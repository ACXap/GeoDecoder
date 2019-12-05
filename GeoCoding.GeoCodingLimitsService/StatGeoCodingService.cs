// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GeoCoding.GeoCodingLimitsService
{
    public static class StatGeoCodingService
    {
        private const string url = "https://api-developer.tech.yandex.net";
        public static EntityResult<int> GetStat(string keyDevelop, string keyStat)
        {
            EntityResult<int> result = new EntityResult<int>();

            try
            {
                HttpWebRequest request = WebRequest.CreateHttp($"{url}/projects/{keyStat}//services/apimaps/limits");
                request.Host = @"api-developer.tech.yandex.net";
                request.Headers.Add($"X-Auth-Key:{keyDevelop}");
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            var json = reader.ReadToEnd();
                            var lim = JsonConvert.DeserializeObject<RootObject>(json);
                            result.Entity = lim.limits.apimaps_total_daily.value;
                            result.Successfully = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }
    }
    public class ApimapsTotalDaily
    {
        public int limit { get; set; }
        public DateTime reset_date { get; set; }
        public int value { get; set; }
    }

    public class Limits
    {
        public ApimapsTotalDaily apimaps_total_daily { get; set; }
    }

    public class RootObject
    {
        public Limits limits { get; set; }
    }
}