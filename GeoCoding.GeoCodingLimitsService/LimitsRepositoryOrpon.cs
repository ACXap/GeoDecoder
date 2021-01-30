// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;
using GeoCoding.GeoCodingLimitsService.Data;
using GeoCoding.GeoCodingLimitsService.Data.Model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace GeoCoding.GeoCodingLimitsService
{
    public class LimitsRepositoryOrpon : ILimitsRepository
    {
        public LimitsRepositoryOrpon(ConnectionSettingsDb conSettings)
        {
            if (conSettings == null) throw GetArgumentNullException(nameof(conSettings));

            _connectString = $"Server={conSettings.Server};" +
                                $"Port={conSettings.Port};" +
                                $"User Id={conSettings.Login};" +
                                $"Password={conSettings.Password};" +
                                $"Database={conSettings.BDName};" +
                                $"Timeout=600;" +
                                $"CommandTimeout=600;";
        }

        private const string TABLE_KEY = "public.nsi_spr_geokey";
        private const string TABLE_LIMITS = "public.nsi_geokey_limits";
        private const string KEY_NOT_FOUND = "Key not found";
        private const string ARGUMENT_CANNOT_NULL = "cannot be null";

        private readonly string _connectString;

        #region PrivateMethod
        private ArgumentNullException GetArgumentNullException(string paramName)
        {
            return new ArgumentNullException(paramName, $"{paramName} {ARGUMENT_CANNOT_NULL}");
        }

        private EntityResult<T> GetEntityResultErrorArrgument<T>(string paramName)
        {
            return new EntityResult<T>() { Error = GetArgumentNullException(paramName) };
        }

        #endregion PrivateMethod

        public EntityResult<int> AddApiKey(ApiKey key)
        {
            if (key == null) return GetEntityResultErrorArrgument<int>(nameof(key));

            EntityResult<int> result = new EntityResult<int>();

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string insertApiKey = $"INSERT INTO {TABLE_KEY} (key, description) VALUES(@key, @desc)";
                using NpgsqlCommand cmd = new NpgsqlCommand(insertApiKey, conn);
                cmd.Parameters.AddWithValue("key", key.Key);
                cmd.Parameters.AddWithValue("desc", key.Description);
                result.Entity = cmd.ExecuteNonQuery();

                result.Successfully = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<int> AddUseUpLimits(UseLimits useLimits)
        {
            if (useLimits == null) return GetEntityResultErrorArrgument<int>(nameof(useLimits));
            

            EntityResult<int> result = new EntityResult<int>();

            if (useLimits.Value == 0)
            {
                result.Successfully = true;
                return result;
            }

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string insertUseLimits = $"INSERT INTO {TABLE_LIMITS} (id_key, value, name_user, date_time) " +
                    $"VALUES((Select id from {TABLE_KEY} where key=@key), @value, @name_user, @date)";
                using NpgsqlCommand cmd = new NpgsqlCommand(insertUseLimits, conn);
                cmd.Parameters.AddWithValue("key", useLimits.Key);
                cmd.Parameters.AddWithValue("value", useLimits.Value);
                cmd.Parameters.AddWithValue("name_user", useLimits.User);
                cmd.Parameters.AddWithValue("date", useLimits.DateTime);
                result.Entity = cmd.ExecuteNonQuery();
                result.Successfully = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<bool> CheckRepository()
        {
            EntityResult<bool> result = new EntityResult<bool>();

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string existsTable = $"SELECT '{TABLE_KEY}'::regclass";
                using NpgsqlCommand cmd = new NpgsqlCommand(existsTable, conn);
                cmd.ExecuteNonQuery();
                result.Successfully = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<UseLimits> GetAllUseUpLimits(string key)
        {
            if (string.IsNullOrEmpty(key)) return GetEntityResultErrorArrgument<UseLimits>(nameof(key));

            EntityResult<UseLimits> result = new EntityResult<UseLimits>();

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string selectApiKey = $"Select k.key, l.value, l.date_time From {TABLE_KEY} k" +
                    $" left join {TABLE_LIMITS} l on l.id_key=k.id Where k.key=@key";
                using NpgsqlCommand cmd = new NpgsqlCommand(selectApiKey, conn);
                cmd.Parameters.AddWithValue("key", key);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    var list = new List<UseLimits>();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(1))
                        {
                            list.Add(new UseLimits()
                            {
                                Key = reader.GetString(0),
                                Value = reader.GetInt32(1),
                                DateTime = reader.GetDateTime(2)
                            });
                        }
                    }

                    result.Entities = list;
                    result.Successfully = true;
                }
                else
                {
                    result.Error = new Exception(KEY_NOT_FOUND);
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<ApiKey> GetApiKeyByKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return GetEntityResultErrorArrgument<ApiKey>(nameof(key));

            EntityResult<ApiKey> result = new EntityResult<ApiKey>();
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string selectApiKey = $"Select id, key, description From {TABLE_KEY} Where key=@key";
                using NpgsqlCommand cmd = new NpgsqlCommand(selectApiKey, conn);
                cmd.Parameters.AddWithValue("key", key);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    result.Entity = new ApiKey()
                    {
                        Id = reader.GetInt32(0),
                        Key = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? reader.GetString(2) : string.Empty
                    };
                    result.Successfully = true;
                }
                else
                {
                    result.Error = new Exception(KEY_NOT_FOUND);
                }

            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<UseLimits> GetLastUseUpLimits(string key)
        {
            if (string.IsNullOrEmpty(key)) return GetEntityResultErrorArrgument<UseLimits>(nameof(key));

            EntityResult<UseLimits> result = new EntityResult<UseLimits>();

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string selectApiKey = $"Select k.key, l.value, l.date_time From {TABLE_KEY} k" +
                    $" left join {TABLE_LIMITS} l on l.id_key=k.id Where k.key=@key order by l.date_time desc limit 1";
                using NpgsqlCommand cmd = new NpgsqlCommand(selectApiKey, conn);
                cmd.Parameters.AddWithValue("key", key);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (!reader.IsDBNull(1))
                    {
                        result.Entity = new UseLimits()
                        {
                            Key = reader.GetString(0),
                            Value = reader.GetInt32(1),
                            DateTime = reader.GetDateTime(2)
                        };
                    }
                    else
                    {
                        result.Entity = new UseLimits() { Key = key, Value = 0 };
                    }
                    result.Successfully = true;
                }
                else
                {
                    result.Error = new Exception(KEY_NOT_FOUND);
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        public EntityResult<int> RemoveApiKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return GetEntityResultErrorArrgument<int>(nameof(key));

            EntityResult<int> result = new EntityResult<int>();

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectString);
                conn.Open();

                string removeApiKey = $"DELETE FROM {TABLE_KEY} WHERE key=@key";
                using NpgsqlCommand cmd = new NpgsqlCommand(removeApiKey, conn);
                cmd.Parameters.AddWithValue("key", key);
                result.Entity = cmd.ExecuteNonQuery();
                result.Successfully = result.Entity > 0;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }
    }
}