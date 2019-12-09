// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;
using Npgsql;
using System;
using System.Collections.Generic;

namespace GeoCoding.BDService
{
    public class BDPostgresql : IBDService
    {
        /// <summary>
        /// Метод для выполнения пользовательского запроса к базе данных
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="query">Пользовательский запрос</param>
        public EntityResult<EntityAddress> ExecuteUserQuery(ConnectionSettingsDb conSettings, string query)
        {
            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                using NpgsqlCommand com = new NpgsqlCommand(query, con);
                using NpgsqlDataReader reader = com.ExecuteReader();

                result.Entities = GetData(reader);
                result.Successfully = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// Метод для получения адресов без координат
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="limitRow">Лимит строк</param>
        public EntityResult<EntityAddress> GetNewAddress(ConnectionSettingsDb conSettings, int limitRow)
        {
            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                string query = "select eah.orponid as globalid, eah.adr_adm_ter as address, eah.fiasguid as fiasguid" +
                    " from public.ent_as_house eah" +
                    " where 1=1 and eah.livestatus = 1" +
                    " and exists (select 1 from public.ent_id_vs_o_add," +
                    " public.ent_rf_rtk b, public.orpon_sys_rtk_mrb c," +
                    " public.ent_sp_sys_rtk d" +
                    " where house_id = eah.id" +
                    " and c.mrb_id = eah.mrf_id" +
                    " and d.id = c.rtk_sys_id" +
                    " and d.is_master_system" +
                    " and system_id = d.id)" +
                    " and eah.coordinates_id is null and eah.adr_adm_ter is not null limit " + limitRow;
                using NpgsqlCommand com = new NpgsqlCommand(query, con);
                using NpgsqlDataReader reader = com.ExecuteReader();

                result.Entities = GetData(reader);
                result.Successfully = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }
        
        /// <summary>
        /// Метод для получения адресов с плохими координатами
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="limitRow">Лимит строк</param>
        public EntityResult<EntityAddress> GetBadAddress(ConnectionSettingsDb conSettings, int limitRow)
        {
            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                string query = "select eah.orponid as globalid, eah.adr_adm_ter as address, eah.fiasguid as fiasguid" +
                    " from public.ent_as_house eah, public.house_coordinates hc" +
                    " where 1=1 and eah.coordinates_id=hc.id" +
                    " and eah.livestatus=1" +
                    " and hc.quality_code<>1" +
                    " and eah.adr_adm_ter is not null order by hc.change_date NULLS first limit " + limitRow;

                using NpgsqlCommand com = new NpgsqlCommand(query, con);
                using NpgsqlDataReader reader = com.ExecuteReader();

                result.Entities = GetData(reader);
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// Метод для проверки соединения с базой данных 
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        public EntityResult<bool> ConnectBD(ConnectionSettingsDb conSettings)
        {
            EntityResult<bool> result = new EntityResult<bool>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();
                string existsTable = $"SELECT 'public.ent_as_house'::regclass";
                using NpgsqlCommand cmd = new NpgsqlCommand(existsTable, con);
                cmd.ExecuteNonQuery();
                result.Successfully = true;
                result.Entity = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// Метод для формирования строки подключения
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <returns>Строка подключения</returns>
        private string GetConnectionString(ConnectionSettingsDb conSettings)
        {
            return $"Server={conSettings.Server};" +
                $"Port={conSettings.Port};" +
                $"User Id={conSettings.Login};" +
                $"Password={conSettings.Password};" +
                $"Database={conSettings.BDName};" +
                $"Timeout=300;" +
                $"CommandTimeout=300;";
        }

        /// <summary>
        /// метод для получения объекта адреса из ридера потока из базы
        /// </summary>
        /// <param name="reader">Ридер потока из базы</param>
        /// <returns>Коллекция адресов</returns>
        private List<EntityAddress> GetData(NpgsqlDataReader reader)
        {
            List<EntityAddress> data = new List<EntityAddress>();

            while (reader.Read())
            {
                var entity = new EntityAddress();

                if (!reader.IsDBNull(0))
                {
                    entity.OrponId = reader.GetInt32(0);
                }
                if (!reader.IsDBNull(1))
                {
                    entity.Address = reader.GetString(1);
                }
                if (reader.FieldCount > 2 && reader.GetName(2).ToLower() == "fiasguid" && !reader.IsDBNull(2))
                {
                    Guid.TryParse(reader.GetString(2), out Guid guid);
                    entity.FiasGuid = guid;
                }
                data.Add(entity);
            }

            return data;
        }
    }
}