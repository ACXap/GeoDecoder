using System;
using System.IO;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    class SputnikGeoCodingService : IGeoCodingService
    {
        private const string _sputnicUrl = "http://search.maps.sputnik.ru/search?q=";

        public void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            Exception error = null;
            GeoCod geocod = null;

            if(!string.IsNullOrEmpty(address))
            {
                GetJsonString((s, e) =>
                {
                    error = e;
                    if (e == null && !string.IsNullOrEmpty(s))
                    {
                        ParserJson((g, er) =>
                        {
                            error = er;
                            if (error == null)
                            {
                                geocod = g;
                            }
                        }, s);
                    }
                }, address);
            }
            else
            {
                error = new ArgumentNullException();
            }

            callback(geocod, error);
        }

        private void GetJsonString(Action<string, Exception> callback, string address)
        {
            Exception error = null;
            string json = string.Empty;
            string url = $"{_sputnicUrl}{address}";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("Content-Encoding: gzip, deflate, br");
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                json = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(json, error);
        }

        private void ParserJson(Action<GeoCod, Exception> callback, string json)
        {
            Exception error = null;
            GeoCod geocod = null;

            try
            {

            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }
    }
}