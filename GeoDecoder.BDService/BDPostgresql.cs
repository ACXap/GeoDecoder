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
        /// <param name="callback">Функция обратного вызова,с параметрами: множество объектов, ошибка</param>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="query">Пользовательский запрос</param>
        public void ExecuteUserQuery(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettings conSettings, string query)
        {
            Exception error = null;
            List<Entity> data = new List<Entity>();
            string connectionString = SetConnectionString(conSettings);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        using (NpgsqlCommand com = new NpgsqlCommand(query, con))
                        {
                            using (NpgsqlDataReader reader = com.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var entity = new Entity();

                                    if (!reader.IsDBNull(0))
                                    {
                                        entity.OrponId = reader.GetInt32(0);
                                    }
                                    if (!reader.IsDBNull(1))
                                    {
                                        entity.Address = reader.GetString(1);
                                    }

                                    data.Add(entity);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(data, error);
        }

        /// <summary>
        /// Метод для проверки соединения с базой данных 
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: ошибка</param>
        /// <param name="conSettings">Свойства подключения</param>
        public void ConnectBD(Action<Exception> callback, ConnectionSettings conSettings)
        {
            Exception error = null;
            string connectionString = SetConnectionString(conSettings);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    //finally
                    //{
                    //    con.Close();
                    //    con.Dispose();
                    //}
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        /// <summary>
        /// метод для формирования строки подключения
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <returns>Строка подключения</returns>
        private string SetConnectionString(ConnectionSettings conSettings)
        {
            return $"Server={conSettings.Server};Port={conSettings.Port};User Id={conSettings.Login};Password={conSettings.Password};Database={conSettings.BDName};Timeout=0";
        }
    }
}