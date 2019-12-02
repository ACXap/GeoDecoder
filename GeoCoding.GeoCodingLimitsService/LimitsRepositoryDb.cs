using GeoCoding.Entities;
using GeoCoding.GeoCodingLimitsService.Data;
using GeoCoding.GeoCodingLimitsService.Data.Model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace GeoCoding.GeoCodingLimitsService
{
    public class LimitsRepositoryDb : ILimitsRepository
    {
        private const string TABLE_KEY = "public.t_000148_ent_spr_geokey";
        private const string TABLE_LIMITS = "public.t_000148_ent_geokey_limits";

        private readonly string _connectString;

        public LimitsRepositoryDb(ConnectionSettingsDb conSettings)
        {
            _connectString = $"Server={conSettings.Server};Port={conSettings.Port};User Id={conSettings.Login};Password={conSettings.Password};Database={conSettings.BDName};Timeout=300;CommandTimeout=300;";
        }

        public EntityResult<int> AddApiKey(ApiKey key)
        {
            EntityResult<int> result = new EntityResult<int>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();
                    string insertApiKey = $"INSERT INTO {TABLE_KEY} (key, description) VALUES(@key, @desc)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertApiKey, conn))
                    {
                        cmd.Parameters.AddWithValue("key", key.Key);
                        cmd.Parameters.AddWithValue("desc", key.Description);
                        result.Entity = cmd.ExecuteNonQuery();
                        result.Successfully = true;
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successfully = false;
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<ApiKey> GetApiKeyByKey(string key)
        {
            EntityResult<ApiKey> result = new EntityResult<ApiKey>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();
                    string selectApiKey = $"Select id, key, description From {TABLE_KEY} Where key=@key";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectApiKey, conn))
                    {
                        cmd.Parameters.AddWithValue("key", key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var k = new ApiKey();
                            while (reader.Read())
                            {
                                k.Id = reader.GetInt32(0);
                                k.Key = reader.GetString(1);
                                k.Description = reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                            }
                            result.Entity = k;
                            result.Successfully = true;

                            reader.Close();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successfully = false;
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<int> RemoveApiKey(string key)
        {
            EntityResult<int> result = new EntityResult<int>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();
                    string removeApiKey = $"DELETE FROM {TABLE_KEY} WHERE key=@key";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(removeApiKey, conn))
                    {
                        cmd.Parameters.AddWithValue("key", key);
                        result.Entity = cmd.ExecuteNonQuery();
                        result.Successfully = true;
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successfully = false;
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<int> AddUseUpLimits(UseLimits useLimits)
        {
            EntityResult<int> result = new EntityResult<int>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();
                    string insertUseLimits = $"INSERT INTO {TABLE_LIMITS} (id_key, value, date_time) " +
                        $"VALUES((Select id from {TABLE_KEY} where key=@key), @value, @date)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertUseLimits, conn))
                    {
                        cmd.Parameters.AddWithValue("key", useLimits.Key);
                        cmd.Parameters.AddWithValue("value", useLimits.Value);
                        cmd.Parameters.AddWithValue("date", useLimits.DateTime);
                        result.Entity = cmd.ExecuteNonQuery();
                        result.Successfully = true;
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successfully = false;
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<UseLimits> GetAllUseUpLimits(string key)
        {
            EntityResult<UseLimits> result = new EntityResult<UseLimits>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();
                    string selectApiKey = $"Select k.key, l.value, l.date_time From {TABLE_KEY} k" +
                        $"left join {TABLE_LIMITS} l on l.id=k.id Where k.key=@key";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectApiKey, conn))
                    {
                        cmd.Parameters.AddWithValue("key", key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<UseLimits>();
                            while (reader.Read())
                            {
                                var k = reader.IsDBNull(0) ? reader.GetString(0) : string.Empty;
                                if (!string.IsNullOrEmpty(k))
                                {
                                    var isExistValue = reader.IsDBNull(1);
                                    if (isExistValue)
                                    {
                                        var value = reader.GetInt32(1);
                                        var date = reader.GetDateTime(2);
                                        var u = new UseLimits()
                                        {
                                            Key = k,
                                            Value = value,
                                            DateTime = date
                                        };
                                        list.Add(u);
                                    }
                                }
                                else
                                {
                                    result.Successfully = false;
                                    result.Error = new Exception("Key not found");
                                }
                            }

                            result.Entities = list;
                            result.Successfully = true;

                            reader.Close();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successfully = false;
                result.Error = ex;
            }

            return result;
        }
        public EntityResult<UseLimits> GetLastUseUpLimits(string key)
        {
            EntityResult<UseLimits> result = new EntityResult<UseLimits>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
                {
                    conn.Open();
                    string selectApiKey = $"Select k.key, l.value, l.date_time From {TABLE_KEY} k" +
                        $"left join {TABLE_LIMITS} l on l.id=k.id Where k.key=@key order by l.date_time desc limit 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectApiKey, conn))
                    {
                        cmd.Parameters.AddWithValue("key", key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var k = reader.IsDBNull(0) ? reader.GetString(0) : string.Empty;
                                if (!string.IsNullOrEmpty(k))
                                {
                                    var isExistValue = reader.IsDBNull(1);
                                    if (isExistValue)
                                    {
                                        var value = reader.GetInt32(1);
                                        var date = reader.GetDateTime(2);
                                        var u = new UseLimits()
                                        {
                                            Key = k,
                                            Value = value,
                                            DateTime = date
                                        };
                                        result.Entity = u;
                                        result.Successfully = true;
                                    }
                                }
                                else
                                {
                                    result.Successfully = false;
                                    result.Error = new Exception("Key not found");
                                }
                            }

                            reader.Close();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successfully = false;
                result.Error = ex;
            }

            return result;
        }
    }
}