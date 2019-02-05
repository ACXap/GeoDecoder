using Npgsql;
using System;
using System.Collections.Generic;

namespace GeoCoding.BDService
{
    public class BDPostgresql : IBDService
    {
        private string _connectionString;

        public void ExecuteUserQuery(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettings conSettings, string query)
        {
            Exception error = null;
            List<Entity> data = new List<Entity>();
            SetConnectionString(conSettings);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
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

        public void GetAddress(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettings conSettings)
        {
            throw new NotImplementedException();
        }

        public void ConnectBD(Action<Exception> callback, ConnectionSettings conSettings)
        {
            Exception error = null;
            SetConnectionString(conSettings);
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
                {
                    try
                    {
                        con.Open();

                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }


            callback(error);
        }

        private void SetConnectionString(ConnectionSettings conSettings)
        {
            _connectionString = $"Server={conSettings.Server};Port={conSettings.Port};User Id={conSettings.Login};Password={conSettings.Password};Database={conSettings.BDName};Timeout=0";
        }
    }
}