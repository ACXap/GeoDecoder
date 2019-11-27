using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    public static class StatGeoCodingService
    {
        private const string url = "https://api-developer.tech.yandex.net";
        public static int GetStat(string keyDevelop, string keyStat)
        {
            HttpWebRequest request = WebRequest.CreateHttp($"{url}/projects/{keyStat}//services/apimaps/limits");
            request.Host = @"api-developer.tech.yandex.net";
            request.Headers.Add($"X-Auth-Key:{keyDevelop}");
            request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream != null)
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            var json = reader.ReadToEnd();
                            var lim = JsonConvert.DeserializeObject<RootObject>(json);
                            return lim.limits.apimaps_total_daily.value;
                        }
                    }
                }
            }
            return -1;
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