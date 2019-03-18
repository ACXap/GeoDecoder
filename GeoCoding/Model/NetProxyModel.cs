using GeoCoding.FileService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding
{
    public class NetProxyModel
    {
        private readonly IFileService _fileService = new FileService.FileService();
        private readonly string _fileNameProxyList = "ProxyList.csv";

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

                            if (!string.IsNullOrEmpty(str[2]))
                            {
                                int.TryParse(str[2].Split(' ')[0], out int delay);
                                proxy.Delay = delay;
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