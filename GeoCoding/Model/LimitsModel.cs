﻿using GeoCoding.Entities;
using GeoCoding.GeoCodingLimitsService;
using GeoCoding.GeoCodingLimitsService.Data;
using GeoCoding.GeoCodingLimitsService.Data.Model;
using GeoCoding.Helpers;
using GeoCoding.Model.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCoding.Model
{
    public class LimitsModel
    {
        public LimitsModel(ILimitsRepository limitsRepository, IEnumerable<EntityApiKey> collectionKey)
        {
            _limitsRepository = limitsRepository;
            _collectionKey = collectionKey;
        }

        #region PrivateField

        private readonly ILimitsRepository _limitsRepository;
        private readonly IEnumerable<EntityApiKey> _collectionKey;

        #endregion PrivateField

        #region PublicProperties
        #endregion PublicProperties

        #region PrivateMethod
        #endregion PrivateMethod

        private EntityApiKey _currentApiKey;
        private object _lock = new object();

        #region PublicMethod

        public Task<EntityResult<bool>> InitApiKey(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                EntityResult<bool> result = new EntityResult<bool>();

                _currentApiKey = _collectionKey.Single(x => x.ApiKey == key);

                var lastDb = GetLastUseLimits(_currentApiKey.ApiKey);
                var lastServ = GetLastUseLimitsServer(_currentApiKey);

                if (!lastDb.Successfully || !lastServ.Successfully)
                {
                    result.Successfully = false;
                    result.Error = lastDb.Error ?? lastServ.Error;
                    return result;
                }

                var dateNow = DateTime.Now;
                if (dateNow.Date > _currentApiKey.DateCurrentSpent.Date && dateNow.Date > lastDb.Entity.DateTime.Date)
                {
                    _currentApiKey.DateCurrentSpent = dateNow;
                    _currentApiKey.CurrentSpent = 0;
                }

                result.Successfully = true;
                return result;
            });
        }

        public bool CanGeo(string key)
        {
            var result = false;
            var apiKey = _collectionKey.Single(x => x.ApiKey == key);
            if (apiKey != _currentApiKey) throw new Exception("Key Error");

            lock (_lock)
            {
                _currentApiKey.CurrentSpent += 1;
                _currentApiKey.DateCurrentSpent = DateTime.Now;
                if (_currentApiKey.CurrentSpent < _currentApiKey.CurrentLimit)
                {
                    result = true;
                }
            }

            return result;
        }

        public EntityResult<bool> CheckRepo()
        {
            var result = _limitsRepository.CheckRepository();
            return result;
        }

        public EntityResult<bool> CheckSyncApiKey(string apiKey)
        {
            EntityResult<bool> result = new EntityResult<bool>();

            var r = _limitsRepository.GetApiKeyByKey(HashHelper.HashString(apiKey));
            if (r.Successfully)
            {
                result.Successfully = true;
            }
            else
            {
                result.Successfully = false;
                result.Error = r.Error;
            }
            return result;
        }

        public void SetStatusSyncApiKey()
        {
            Task.Factory.StartNew(() =>
            {
                ParallelOptions po = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 5
                };
                Parallel.ForEach(_collectionKey, po, (k) =>
                {
                    k.StatusSync = StatusSyncType.SyncProcessed;
                    var result = CheckSyncApiKey(k.ApiKey);
                    if (result.Successfully)
                    {
                        k.StatusSync = StatusSyncType.Sync;
                        k.Error = string.Empty;
                        var r = GetLastUseLimits(k.ApiKey);
                        if (r.Successfully)
                        {
                            k.CurrentSpent = r.Entity.Value;
                            k.DateCurrentSpent = r.Entity.DateTime;
                        }
                    }
                    else
                    {
                        k.StatusSync = StatusSyncType.Error;
                        k.Error = result.Error.Message;
                    }
                });
            });
        }

        public Task<EntityResult<bool>> SyncApiKey(EntityApiKey key)
        {
            return Task.Factory.StartNew(() =>
            {
                EntityResult<bool> result = new EntityResult<bool>();

                key.StatusSync = StatusSyncType.SyncProcessed;
                var k = new ApiKey()
                {
                    Key = HashHelper.HashString(key.ApiKey),
                    Description = key.Description
                };
                var r = _limitsRepository.AddApiKey(k);
                if (r.Successfully)
                {
                    key.StatusSync = StatusSyncType.Sync;
                    key.Error = string.Empty;
                }
                else
                {
                    var er = r.Error.Message.Split(' ');
                    if (er[1] == "duplicate")
                    {
                        key.StatusSync = StatusSyncType.Sync;
                        key.Error = string.Empty;
                    }
                    else
                    {
                        key.StatusSync = StatusSyncType.Error;
                        key.Error = r.Error.Message;
                    }
                }

                return result;
            });
        }

        public EntityResult<bool> SetLastUseLimits(UseLimits useLimits)
        {
            EntityResult<bool> result = new EntityResult<bool>();
            var r = _limitsRepository.AddUseUpLimits(useLimits);
            if (r.Successfully)
            {
                result.Successfully = true;
            }
            else
            {
                result.Error = r.Error;
            }
            return result;
        }

        public EntityResult<UseLimits> GetLastUseLimits(string apiKey)
        {
            EntityResult<UseLimits> result;
            result = _limitsRepository.GetLastUseUpLimits(HashHelper.HashString(apiKey));
            return result;
        }

        public EntityResult<UseLimits> GetLastUseLimitsServer(EntityApiKey apiKey)
        {
            EntityResult<UseLimits> result = new EntityResult<UseLimits>();
            var a = StatGeoCodingService.GetStat(apiKey.ApiKeyDevelop, apiKey.ApiKeyStat);
            if (a.Successfully)
            {
                result.Entity = new UseLimits()
                {
                    Key = apiKey.ApiKey,
                    DateTime = DateTime.Now,
                    Value = a.Entity
                };
                result.Successfully = true;
            }
            else
            {
                result.Error = a.Error;
            }

            return result;
        }

        #endregion PublicMethod
    }
}