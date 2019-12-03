using GeoCoding.Entities;
using GeoCoding.GeoCodingLimitsService;
using GeoCoding.GeoCodingLimitsService.Data;
using GeoCoding.GeoCodingLimitsService.Data.Model;
using GeoCoding.Helpers;
using GeoCoding.Model.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GeoCoding.Model
{
    public class LimitsModel
    {
        public LimitsModel(ILimitsRepository limitsRepository)
        {
            _limitsRepository = limitsRepository;
        }

        #region PrivateField

        private readonly ILimitsRepository _limitsRepository;

        #endregion PrivateField

        #region PublicProperties
        #endregion PublicProperties

        #region Command
        #endregion Command

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod
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

        public void SetStatusSyncApiKey(ObservableCollection<EntityApiKey> collectionApiKeys)
        {
            Task.Factory.StartNew(() =>
            {
                ParallelOptions po = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 5
                };
                Parallel.ForEach(collectionApiKeys, po, (k) =>
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
                    if(er[1] == "duplicate")
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
        
        public EntityResult<UseLimits> GetLastUseLimits(string apiKey)
        {
            EntityResult<UseLimits> result;
            result = _limitsRepository.GetLastUseUpLimits(HashHelper.HashString(apiKey));
            return result;
        }
        #endregion PublicMethod
    }
}
