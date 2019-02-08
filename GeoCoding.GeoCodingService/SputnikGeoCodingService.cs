using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    internal class SputnikGeoCodingService : IGeoCodingService
    {
        private const string _sputnicUrl = "http://search.maps.sputnik.ru/search/addr?q=";

        public string Name => "Sputnik";

        public void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            Exception error = null;
            GeoCod geocod = null;

            if (!string.IsNullOrEmpty(address))
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
                SputnikJsonRootObject a = JsonConvert.DeserializeObject<SputnikJsonRootObject>(json);
                var countFound = a.result.address[0].features.Count;

                if (countFound == 1)
                {
                    var g = a.result.address[0].features[0];
                    geocod = new GeoCod()
                    {
                        Text = g.properties.display_name,
                        CountResult = (byte)countFound,
                        Latitude = g.geometry.geometries[0].coordinates[0].ToString(),
                        Longitude = g.geometry.geometries[0].coordinates[1].ToString(),
                        Kind = g.properties.type,
                        Precision = g.properties.full_match.ToString()
                    };
                }
                else
                {
                    var list = new List<GeoCod>();
                    var adr = a.result.address[0].features;
                    foreach (var item in adr)
                    {
                        var geo = new GeoCod()
                        {
                            Text = item.properties.display_name,
                            CountResult = (byte)countFound,
                            Latitude = item.geometry.geometries[0].coordinates[0].ToString(),
                            Longitude = item.geometry.geometries[0].coordinates[1].ToString(),
                            Kind = item.properties.type,
                            Precision = item.properties.full_match.ToString()
                        };
                        list.Add(geo);
                    }

                    var e = list.Where(x => x.Precision == "true");
                    if (e != null && e.Count() == 1)
                    {
                        geocod = e.First();
                        geocod.CountResult = 1;
                    }
                    else
                    {
                        geocod = new GeoCod() { CountResult = (byte)countFound };
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }
    }
}