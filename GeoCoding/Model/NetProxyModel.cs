using GeoCoding.FileService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GeoCoding
{
    public class NetProxyModel
    {
        private readonly IFileService _fileService = new FileService.FileService();
        private readonly string _fileNameProxyList = "ProxyList.csv";
        private readonly string _urlTest = "https://google.ru";

        private void TestProxy(ProxyEntity proxy)
        {
            string data = string.Empty;
            proxy.Error = string.Empty;
            Stopwatch sw = new Stopwatch();
            proxy.Status = StatusConnect.ConnectNow;
            try
            {
                WebRequest request = WebRequest.Create(_urlTest);
                request.Proxy = new WebProxy(proxy.Address, proxy.Port);

                sw.Start();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                data = reader.ReadToEnd();
                                sw.Stop();
                                proxy.Delay = (int)sw.ElapsedMilliseconds;
                            }
                        }
                    }
                }
                proxy.IsActive = !string.IsNullOrEmpty(data);
                proxy.Status = StatusConnect.OK;
            }
            catch (Exception ex)
            {
                proxy.Error = ex.Message;
                proxy.Status = StatusConnect.Error;
                proxy.IsActive = false;
                sw?.Stop();
            }
            finally
            {
                sw?.Stop();
            }
        }

        public Task TestProxyAsync(ProxyEntity proxy)
        {
            return Task.Factory.StartNew(() =>
            {
                TestProxy(proxy);
            });
        }

        public Task TestListProxyAsync(IEnumerable<ProxyEntity> data)
        {
            return Task.Factory.StartNew(() =>
            {
                ParallelOptions po = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 50
                };
                var a = Parallel.ForEach(data, po, (item) =>
                {
                    TestProxy(item);
                });
            });
        }

        public void GetProxyList(Action<IEnumerable<ProxyEntity>, Exception> callback)
        {
            Exception error = null;
            List<ProxyEntity> data = null;

            _fileService.GetData((d, e) =>
            {
                if (e == null)
                {
                    try
                    {
                        data = new List<ProxyEntity>(d.Count());

                        foreach (var item in d)
                        {
                            var str = item.Split(';');

                            var proxy = new ProxyEntity()
                            {
                                Address = str[0]
                            };
                            int.TryParse(str[1], out int port);
                            proxy.Port = port;

                            if (str.Length > 2)
                            {
                                if (!string.IsNullOrEmpty(str[2]))
                                {
                                    int.TryParse(str[2].Split(' ')[0], out int delay);
                                    proxy.Delay = delay;
                                }
                            }

                            if (!string.IsNullOrEmpty(proxy.Address) && proxy.Port != 0)
                            {
                                data.Add(proxy);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                }
                else
                {
                    error = e;
                }

            }, _fileNameProxyList);

            callback(data, error);
        }
    }
}