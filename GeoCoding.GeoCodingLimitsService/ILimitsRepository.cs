// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;
using GeoCoding.GeoCodingLimitsService.Data;
using GeoCoding.GeoCodingLimitsService.Data.Model;

namespace GeoCoding.GeoCodingLimitsService
{
    public interface ILimitsRepository
    {
        EntityResult<int> AddApiKey(ApiKey key);
        EntityResult<int> RemoveApiKey(string key);
        EntityResult<ApiKey> GetApiKeyByKey(string key);

        EntityResult<int> AddUseUpLimits(UseLimits useLimits);
        EntityResult<UseLimits> GetLastUseUpLimits(string key);
        EntityResult<UseLimits> GetAllUseUpLimits(string key);

        EntityResult<bool> CheckRepository();
    }
}