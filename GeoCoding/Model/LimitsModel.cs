// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;
using GeoCoding.GeoCodingLimitsService;
using GeoCoding.GeoCodingLimitsService.Data;
using GeoCoding.GeoCodingLimitsService.Data.Model;
using GeoCoding.Helpers;
using GeoCoding.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private EntityApiKey _currentApiKey;
        private readonly object _lock = new object();

        #endregion PrivateField

        public async Task<EntityResult<int>> GetCurrentLimit(string key)
        {
            EntityResult<int> result = new EntityResult<int>();

            var r = await InitApiKey(key);

            if (r.Successfully)
            {
                result.Successfully = true;
                result.Entity = _currentApiKey.CurrentLimit - _currentApiKey.CurrentSpent;
            }
            else
            {
                result.Error = r.Error;
            }

            return result;
        }

        public Task<EntityResult<int>> GetMaxLimit(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                EntityResult<int> result = new EntityResult<int>();

                _currentApiKey = _collectionKey.Single(x => x.ApiKey == key);
                var lastServ = GetLastUseLimitsServer(_currentApiKey);
                System.Diagnostics.Debug.WriteLine(lastServ.Entity);

                if (!lastServ.Successfully)
                {
                    result.Error = lastServ.Error;
                    _currentApiKey.Error = result.Error.Message;
                    _currentApiKey.StatusSync = StatusSyncType.Error;
                    return result;
                }

                _currentApiKey.Error = string.Empty;
                _currentApiKey.StatusSync = StatusSyncType.Sync;

                result.Entity = _currentApiKey.MaxLimit - lastServ.Entity.Value;
                _countSpentFirst = lastServ.Entity.Value;
                _countLimit = result.Entity;
                result.Successfully = true;

                return result;
            });
        }

        private int _countLimit;
        private int _countSpentFirst;

        public bool CanGeoMaxLimit(string key)
        {
            var result = false;
            var apiKey = _collectionKey.Single(x => x.ApiKey == key);
            if (apiKey != _currentApiKey) throw new Exception("Key Error");
            if (apiKey.StatusSync != StatusSyncType.Sync) throw new Exception("Key Not Sync with BD");

            lock (_lock)
            {
                if (_countLimit > 0)
                {
                    _countLimit -= 1;
                    result = true;
                }
            }

            return result;
        }

        private bool _isStartGetLimit;

        public void StartGetLimit(string key)
        {
            Task.Factory.StartNew(() =>
            {
                _currentApiKey = _collectionKey.Single(x => x.ApiKey == key);
                while (_isStartGetLimit)
                {
                    var countLimit = _countLimit;

                    var lastServ = GetLastUseLimitsServer(_currentApiKey);

                    if (lastServ.Successfully)
                    {
                        lock (_lock)
                        {
                            if (lastServ.Entity.Value > _currentApiKey.MaxLimit)
                            {
                                _countLimit = 0;
                            }
                            else
                            {
                                if (lastServ.Entity.Value > _countSpentFirst)
                                {
                                    var a = lastServ.Entity.Value - _countSpentFirst - countLimit;
                                    _countLimit -= a;
                                }
                            }
                        }
                    }
                    Thread.Sleep(60000);
                }
            });
        }

        public void StopGetLimit()
        {
            _isStartGetLimit = false;
        }

        #region PublicMethod

        public Task<EntityResult<bool>> InitApiKey(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                EntityResult<bool> result = new EntityResult<bool>();

                _currentApiKey = _collectionKey.Single(x => x.ApiKey == key);

                var lastDb = GetLastUseLimits(_currentApiKey.ApiKey);
               // var lastServ = GetLastUseLimitsServer(_currentApiKey);

                if (!lastDb.Successfully)// || !lastServ.Successfully)
                {
                    result.Successfully = false;
                    result.Error = lastDb.Error;// ?? lastServ.Error;
                    _currentApiKey.Error = result.Error.Message;
                    _currentApiKey.StatusSync = StatusSyncType.Error;
                    return result;
                }



                _currentApiKey.Error = string.Empty;
                _currentApiKey.StatusSync = StatusSyncType.Sync;

                var dateNow = DateTime.Now;
                if (dateNow.Date > _currentApiKey.DateCurrentSpent.Date && dateNow.Date > lastDb.Entity.DateTime.Date)
                {
                    _currentApiKey.DateCurrentSpent = dateNow;
                    _currentApiKey.CurrentSpent = 0;
                }
                else
                {
                    if (_currentApiKey.DateCurrentSpent > lastDb.Entity.DateTime)
                    {
                        var r = SetLastUseLimits();
                    }

                    if (_currentApiKey.DateCurrentSpent < lastDb.Entity.DateTime)
                    {
                        _currentApiKey.DateCurrentSpent = lastDb.Entity.DateTime;
                        _currentApiKey.CurrentSpent = lastDb.Entity.Value;
                    }
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
            if (apiKey.StatusSync != StatusSyncType.Sync) throw new Exception("Key Not Sync with BD");

            lock (_lock)
            {
                if (_currentApiKey.CurrentSpent < _currentApiKey.CurrentLimit)
                {
                    _currentApiKey.CurrentSpent += 1;
                    _currentApiKey.DateCurrentSpent = DateTime.Now;
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
                    MaxDegreeOfParallelism = 2
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


        private string GetNameUser()
        {
            return Environment.UserName;
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

        public EntityResult<bool> SetLastUseLimits()
        {
            var useLimits = new UseLimits()
            {
                Key = HashHelper.HashString(_currentApiKey.ApiKey),
                DateTime = _currentApiKey.DateCurrentSpent,
                Value = _currentApiKey.CurrentSpent,
                User = GetNameUser()
            };

            return SetLastUseLimits(useLimits);
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

            System.Diagnostics.Debug.WriteLine(a);

            return result;
        }

        #endregion PublicMethod
    }
}