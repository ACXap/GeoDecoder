// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.BDService.Data;
using GeoCoding.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace GeoCoding.BDService
{
    public class BDPostgresql : IBDService
    {
        #region PrivateField

        private const string ARGUMENT_CANNOT_NULL = "cannot be null";
        private const string TABLE_HOUSE = "public.ent_as_house";

        #endregion PrivateField

        /// <summary>
        /// Метод для выполнения пользовательского запроса к базе данных
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="query">Пользовательский запрос</param>
        public EntityResult<EntityAddress> ExecuteUserQuery(ConnectionSettingsDb conSettings, string query, int limitRow)
        {
            if (conSettings == null) return GetEntityResultErrorArrgument<EntityAddress>(nameof(conSettings));
            if (string.IsNullOrEmpty(query)) return GetEntityResultErrorArrgument<EntityAddress>(nameof(query));

            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                if (!query.ToLower().Contains("limit ") && limitRow > 0)
                {
                    query += $" limit {limitRow}";
                }

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
            if (conSettings == null) return GetEntityResultErrorArrgument<EntityAddress>(nameof(conSettings));
            if (limitRow < 1) return GetEntityResultErrorArrgument<EntityAddress>(nameof(limitRow));

            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                string query = GetSqlTempleteNewAddress(limitRow);

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
            if (conSettings == null) return GetEntityResultErrorArrgument<EntityAddress>(nameof(conSettings));
            if (limitRow < 1) return GetEntityResultErrorArrgument<EntityAddress>(nameof(limitRow));

            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                string query = GetSqlTempleteOldBadAddresss(limitRow);

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

        public EntityResult<EntityAddress> ExecuteProcedure(ConnectionSettingsDb conSettings, string scpriptBackgroundGeo, int limitRow)
        {
            if (conSettings == null) return GetEntityResultErrorArrgument<EntityAddress>(nameof(conSettings));
            if (limitRow < 1) return GetEntityResultErrorArrgument<EntityAddress>(nameof(limitRow));

            EntityResult<EntityAddress> result = new EntityResult<EntityAddress>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                string query = $"select * from public.nsi_get_new_bad_coordinates({limitRow});";

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
        /// Метод для проверки соединения с базой данных 
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        public EntityResult<bool> ConnectBD(ConnectionSettingsDb conSettings)
        {
            if (conSettings == null) return GetEntityResultErrorArrgument<bool>(nameof(conSettings));

            EntityResult<bool> result = new EntityResult<bool>();

            try
            {
                using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
                con.Open();

                string existsTable = $"SELECT '{TABLE_HOUSE}'::regclass";
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

        public void SaveData(ConnectionSettingsDb conSettings, IEnumerable<EntityCoordinate> entityCoordinates)
        {
            using NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(conSettings));
            con.Open();

            using var writer = con.BeginBinaryImport($"COPY public.nsi_new_coordinates (orpon_id,latitude,longitude,quality_code) FROM STDIN BINARY");
            foreach (var coordinate in entityCoordinates)
            {
                writer.StartRow();
                writer.Write(coordinate.OrponId, NpgsqlDbType.Bigint);
                writer.Write(coordinate.Latitude, NpgsqlDbType.Double);
                writer.Write(coordinate.Longitude, NpgsqlDbType.Double);
                writer.Write(coordinate.Qcode, NpgsqlDbType.Integer);
            }
            writer.Complete();
        }

        public string GetSqlTempleteNewAddress(int limitRow)
        {
            return "with sys as (select s.house_id from public.ent_sp_sys_rtk d, public.ent_id_vs_o_add s Where d.is_master_system and s.system_id= d.id)" +
                            " SELECT eah.orponid as globalid, eah.adr_adm_ter as address, eah.fiasguid as fiasguid" +
                            $" FROM {TABLE_HOUSE} eah, sys s1" +
                            $" WHERE eah.livestatus = 1" +
                            $" and eah.coordinates_id is null" +
                            $" and eah.id = s1.house_id limit " + limitRow;
        }

        public string GetSqlTempleteOldBadAddresss(int limitRow)
        {
            return "SELECT eah.orponid as globalid, eah.adr_adm_ter as address" + //, eah.fiasguid as fiasguid" +
                    $" FROM {TABLE_HOUSE} eah, public.house_coordinates hc" +
                    " WHERE eah.coordinates_id=hc.id" +
                    " and eah.livestatus=1" +
                    " and hc.quality_code=2" +
                    " and hc.source is null" +
                    //" and (hc.source<>'GUI' or hc.source is null)" +
                    //" order by hc.change_date NULLS first"
                    " and hc.change_date is null" +
                    " limit " + limitRow;
        }

        public string GetSqlTempleteNewOldAddressProcedure(int limitRow)
        {
            return $"select * from public.nsi_get_new_bad_coordinates({limitRow});";
        }

        #region PrivateMethod
        /// <summary>
        /// Метод для получения ошибки отсутсвие аргумента
        /// </summary>
        /// <param name="paramName">Имя аргумента</param>
        /// <returns>Возвращает ошибку</returns>
        private ArgumentNullException GetArgumentNullException(string paramName)
        {
            return new ArgumentNullException(paramName, $"{paramName} {ARGUMENT_CANNOT_NULL}");
        }

        /// <summary>
        /// Метод для получения результата с ошибкой отсутвие аргумента
        /// </summary>
        /// <param name="paramName">Имя аргумента</param>
        /// <returns>Возвращает результат с ошибкой</returns>
        private EntityResult<T> GetEntityResultErrorArrgument<T>(string paramName)
        {
            return new EntityResult<T>() { Error = GetArgumentNullException(paramName) };
        }

        /// <summary>
        /// Метод для формирования строки подключения
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <returns>Строка подключения</returns>
        private string GetConnectionString(ConnectionSettingsDb conSettings)
        {
            var a = $"Server={conSettings.Server};" +
                $"Port={conSettings.Port};" +
                $"User Id={conSettings.Login};" +
                $"Password={conSettings.Password};" +
                $"Database={conSettings.BDName};" +
                "Timeout=1800;" +
                "CommandTimeout=1800;";
            return $"Server={conSettings.Server};" +
                $"Port={conSettings.Port};" +
                $"User Id={conSettings.Login};" +
                $"Password={conSettings.Password};" +
                $"Database={conSettings.BDName};" +
                "Timeout=600;" +
                "CommandTimeout=600;";
        }

        /// <summary>
        /// Метод для получения объекта адреса из ридера потока из базы
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
            reader.Close();

            return data;
        }



        #endregion PrivateMethod
    }
}