using GeoCoding.GeoCodingService;
using GeoCoding.Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoding
{
    /// <summary>
    /// Класс для работы с геокодированием
    /// </summary>
    public class GeoCodingModel
    {
        #region PrivateField
        private const string _limitFile = "limit.dat";
        private const string _errorGeoCodNotFound = "Адрес не найден";
        private const string _errorGeoCodFoundResultMoreOne = "Количество результатов больше 1. Нужны уточнения";
        private const string _errorLimit = "Ваш лимит исчерпан";
        private const string _errorTimeIsUp = "Скорее всего упал сайт";
        private const string _errorLotOfMistakes = "Очень много ошибок при обработке данных. Обработка прекращена";
        private IGeoCodingService _geoCodingService;
        private CancellationTokenSource _cts;

        private Limit _limitGeo;
        private int _countGeo;
        private int _maxCountGeo = 10000;

        private readonly NetSettings _netSettings;
        private readonly GeoCodSettings _geoCodSettings;
        private readonly PolygonViewModel _polygon;
        private readonly object _lock = new object();

        private void GetLimitCount()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (var f = new FileStream(_limitFile, FileMode.OpenOrCreate))
            {
                if (f.Length > 0)
                {
                    _limitGeo = formatter.Deserialize(f) as Limit;
                    if (_limitGeo != null && _limitGeo.Date.Date == DateTime.Now.Date)
                    {
                        _countGeo = _limitGeo.Count;
                    }
                    else
                    {
                        _countGeo = 0;
                    }
                }
                else
                {
                    _countGeo = 0;
                }
            }
        }
        #endregion PrivateField

        #region PrivateMethod

        /// <summary>
        /// Функция для получения координат для объекта
        /// </summary>
        /// <param name="data">Объект</param>
        /// <returns>Возвращает ошибку</returns>
        private Exception SetGeoCod(EntityGeoCod data, ConnectSettings cs)
        {
            if (_geoCodSettings.GeoService.ToLower().Contains("pay"))
            {
                if (_countGeo >= _maxCountGeo)
                {
                    var e = new Exception(_errorLimit);
                    SetError(data, e.Message);
                    data.CountResult = 0;

                    return e;
                }

                Interlocked.Increment(ref _countGeo);
            }

            Exception error = null;
            string errorMsg = string.Empty;

            data.Status = StatusType.Processed;
            data.GeoCoder = _geoCodSettings.GeoService;

            _geoCodingService.GetGeoCod((d, e) =>
            {
                error = e;
                if (e == null)
                {
                    data.ListGeoCod = GetListGeoCod(d);
                    data.CountResult = data.ListGeoCod != null ? data.ListGeoCod.Count : 0;

                    if (data.ListGeoCod != null && data.ListGeoCod.Any())
                    {
                        data.MainGeoCod = GetMainGeoCod(data.ListGeoCod);
                    }
                    else
                    {
                        data.MainGeoCod = null;
                    }

                    if (data.MainGeoCod != null)
                    {
                        data.Status = StatusType.OK;
                        data.Error = string.Empty;
                    }
                    else
                    {
                        if (data.CountResult == 0)
                        {
                            errorMsg = _errorGeoCodNotFound;
                        }
                        if (data.CountResult > 1 && data.MainGeoCod == null)
                        {
                            errorMsg = _errorGeoCodFoundResultMoreOne;
                        }

                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            SetError(data, errorMsg);
                        }
                    }
                }
                else
                {
                    SetError(data, e.Message);
                    data.CountResult = 0;
                }

                data.DateTimeGeoCod = DateTime.Now;

            }, data.Address, cs, GetPolygon());

            return error;
        }

        private List<double> GetPolygon()
        {
            if(_geoCodSettings.CanUsePolygon)
            {
                return _polygon.Polygon;
            }

            return null;
        }

        /// <summary>
        /// Метод заполнения свойств объекта EntityGeoCod при ошибках
        /// </summary>
        /// <param name="geocod">Объект EntityGeoCod</param>
        /// <param name="mes">Сообщение ошибки</param>
        private void SetError(EntityGeoCod geocod, string mes)
        {
            geocod.Error = mes;
            geocod.Status = StatusType.Error;
        }

        /// <summary>
        /// Метод получения основного (точного) геокода для адреса
        /// </summary>
        /// <param name="d">Коллекция всех геокодов для адреса</param>
        /// <returns>Возвращает точный геокод (если он есть)</returns>
        private GeoCod GetMainGeoCod(List<GeoCod> d)
        {
            GeoCod data = null;
            if (d != null && d.Any())
            {
                if (d.Count == 1)
                {
                    data = d.FirstOrDefault();
                }
                else
                {
                    var l = d.Where(x => x.Precision == PrecisionType.Exact);
                    if (l.Count() == 1)
                    {
                        data = l.FirstOrDefault();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Метод получения коллекции геокодов для адреса
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private List<GeoCod> GetListGeoCod(IEnumerable<GeoCodingService.GeoCod> d)
        {
            List<GeoCod> list = null;

            if (d != null && d.Any())
            {
                list = new List<GeoCod>(d.Count());

                foreach (var g in d)
                {
                    GeoCod data = new GeoCod()
                    {
                        AddressWeb = g.Text,
                        Latitude = g.Latitude.Replace(',', '.'),
                        Longitude = g.Longitude.Replace(',', '.')
                    };

                    if (Enum.TryParse(g.Kind?.ToUpperFistChar(), out KindType kind))
                    {
                        data.Kind = kind;
                    }
                    else if (g.Kind == "place" || g.Kind == "suburb")
                    {
                        data.Kind = KindType.Locality;
                    }
                    else if (g.Kind == "houseNumber")
                    {
                        data.Kind = KindType.House;
                    }
                    else if (g.Kind == "street")
                    {
                        data.Kind = KindType.Street;
                    }

                    if (Enum.TryParse(g.Precision?.ToUpperFistChar(), out PrecisionType precision))
                    {
                        data.Precision = precision;
                    }
                    else if (string.IsNullOrEmpty(g.Precision))
                    {
                        data.Precision = PrecisionType.None;
                    }
                    else if (g.Precision?.ToLower() == "true")
                    {
                        data.Precision = PrecisionType.Exact;
                    }
                    else if (g.Precision == "1")
                    {
                        data.Precision = PrecisionType.Exact;
                    }
                    else
                    {
                        if (data.Kind == KindType.Street)
                        {
                            data.Precision = PrecisionType.Street;
                        }
                        else if (data.Kind == KindType.House)
                        {
                            data.Precision = PrecisionType.Near;
                        }
                        else
                        {
                            data.Precision = PrecisionType.Other;
                        }
                    }

                    if (data.Precision == PrecisionType.Exact)
                    {
                        data.Qcode = 1;
                    }
                    else if (data.Precision == PrecisionType.None)
                    {
                        data.Qcode = 0;
                    }
                    else
                    {
                        data.Qcode = 2;
                    }

                    list.Add(data);
                }
            }

            return list;
        }

        /// <summary>
        /// Метод для создания настроек подключения
        /// </summary>
        /// <returns>Возвращает настройки подключения</returns>
        private ConnectSettings GetConnect()
        {
            ConnectSettings cs = new ConnectSettings();

            if (_netSettings.IsNotProxy)
            {
                cs.ProxyType = ProxyType.None;
            }
            else if (_netSettings.IsSystemProxy)
            {
                cs.ProxyType = ProxyType.System;
            }
            else if (_netSettings.IsManualProxy)
            {
                cs.ProxyAddress = _netSettings.Proxy.Address;
                cs.ProxyPort = _netSettings.Proxy.Port;
                cs.ProxyType = ProxyType.Manual;
            }
            else
            {
                var proxy = _netSettings.CollectionListProxy.FirstOrDefault(x => x.IsActive);
                if (proxy != null)
                {
                    cs.ProxyAddress = proxy.Address;
                    cs.ProxyPort = proxy.Port;
                    cs.ProxyType = ProxyType.Manual;
                }
            }

            return cs;
        }

        /// <summary>
        /// Метод для создания настроек подключения с настройками прокси вручную
        /// </summary>
        /// <param name="proxy">Настройки прокси сервера</param>
        /// <returns>Возвращает настройки подключения</returns>
        private ConnectSettings GetConnect(ProxyEntity proxy)
        {
            ConnectSettings cs = new ConnectSettings
            {
                ProxyAddress = proxy.Address,
                ProxyPort = proxy.Port,
                ProxyType = ProxyType.Manual
            };

            return cs;
        }

        #endregion PrivateMethod

        #region PublicMethod

        /// <summary>
        /// Метод остановки процесса
        /// </summary>
        public void StopGet()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// Метод установки текущего геосериса
        /// </summary>
        /// <param name="geoService">Ссылка на геосервис</param>
        private void SetGeoService()
        {
            _geoCodingService = MainGeoService.GetServiceByName(_geoCodSettings.GeoService);
        }

        /// <summary>
        /// Метод для получения координат для множества объектов
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: завершился ли процесс, на каком номере завершился, ошибка</param>
        /// <param name="collectionGeoCod">Множество объектов</param>
        public async void GetAllGeoCod(Action<Exception> callback, IEnumerable<EntityGeoCod> collectionGeoCod)
        {
            Exception error = null;
            SetGeoService();

            _cts = new CancellationTokenSource();

            if (_geoCodSettings.IsMultipleRequests)
            {
                int countError = 0;
                ParallelOptions po = new ParallelOptions
                {
                    CancellationToken = _cts.Token,
                    MaxDegreeOfParallelism = _geoCodSettings.CountRequests
                };
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var connect = GetConnect();
                        var parallelResult = Parallel.ForEach(collectionGeoCod, po, (data, pl) =>
                        {
                            if (!string.IsNullOrEmpty(connect.ProxyAddress))
                            {
                                data.Proxy = $"{connect.ProxyAddress}:{connect.ProxyPort}";
                            }
                            var e = SetGeoCod(data, connect);

                            if (e != null)
                            {
                                if (e.Message == _errorLimit || e.Message == _errorTimeIsUp)
                                {
                                    error = e;
                                    pl.Break();
                                }
                                else
                                {
                                    if (++countError >= _geoCodSettings.MaxCountError)
                                    {
                                        error = new Exception(_errorLotOfMistakes);
                                        pl.Break();
                                    }
                                }
                            }
                            else
                            {
                                countError = 0;
                            }
                        });
                    }
                    catch (OperationCanceledException c)
                    {
                        error = c;
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        _cts?.Dispose();
                    }
                }, _cts.Token);
            }
            else if (_geoCodSettings.IsMultipleProxy)
            {
                ParallelOptions po = new ParallelOptions
                {
                    CancellationToken = _cts.Token,
                    MaxDegreeOfParallelism = _geoCodSettings.CountProxy
                };

                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var parallelResult = Parallel.ForEach(_netSettings.CollectionListProxy, po, (data, pl) =>
                        {
                            EntityGeoCod geo = null;
                            var connect = GetConnect(data);
                            int countError = 0;

                            while (data.IsActive)
                            {
                                if (po.CancellationToken.IsCancellationRequested)
                                {
                                    break;
                                }
                                lock (_lock)
                                {
                                    geo = collectionGeoCod.FirstOrDefault(x => x.Status == StatusType.NotProcessed
                                                                      || (x.Status == StatusType.Error && (x.Error != _errorGeoCodFoundResultMoreOne
                                                                                                        && x.Error != _errorGeoCodNotFound)));
                                    if (geo != null)
                                    {
                                        geo.Status = StatusType.Processed;
                                    }
                                }

                                if (geo != null)
                                {
                                    geo.Proxy = $"{data.Address}:{data.Port}";
                                    var e = SetGeoCod(geo, connect);
                                    if (e != null)
                                    {
                                        if (e.Message == _errorLimit)
                                        {
                                            data.IsActive = false;
                                            data.Error = _errorLimit;
                                        }
                                        else
                                        {
                                            if (++countError >= _geoCodSettings.MaxCountError)
                                            {
                                                data.Error = _errorLotOfMistakes;
                                                data.IsActive = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        countError = 0;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        });
                    }
                    catch (OperationCanceledException c)
                    {
                        error = c;
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        _cts?.Dispose();
                    }
                }, _cts.Token);
            }

            SaveLimit();
            callback(error);
        }

        /// <summary>
        /// Метод для получения координат объекта
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="data">Объект для которого нужны координаты</param>
        public async void GetGeoCod(Action<Exception> callback, EntityGeoCod data)
        {
            Exception error = null;
            data.MainGeoCod = null;
            data.CountResult = 0;
            SetGeoService();

            await Task.Factory.StartNew(() =>
            {
                error = SetGeoCod(data, GetConnect());
            });

            SaveLimit();
            callback(error);
        }

        private void SaveLimit()
        {
            if (_limitGeo == null)
            {
                _limitGeo = new Limit();
            }

            _limitGeo.Date = DateTime.Now;
            _limitGeo.Count = _countGeo;

            BinaryFormatter formatter = new BinaryFormatter();
            using (var f = new FileStream(_limitFile, FileMode.OpenOrCreate))
            {
                formatter.Serialize(f, _limitGeo);
            }
        }

        /// <summary>
        /// Метод для получения урл-адреса запроса
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <returns>Возвращает ссылку на запрос</returns>
        public string GetUrlRequest(string address)
        {
            SetGeoService();
            return _geoCodingService.GetUrlRequest(address, GetPolygon());
        }

        public Task<Exception> GetAllGeoCod(IEnumerable<EntityGeoCod> collectionGeoCod)
        {
            Exception error = null;
            SetGeoService();

            _cts = new CancellationTokenSource();

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (_geoCodSettings.IsMultipleRequests)
                    {
                        int countError = 0;
                        ParallelOptions po = new ParallelOptions
                        {
                            CancellationToken = _cts.Token,
                            MaxDegreeOfParallelism = _geoCodSettings.CountRequests
                        };
                        var connect = GetConnect();
                        var parallelResult = Parallel.ForEach(collectionGeoCod, po, (data, pl) =>
                        {
                            if (!string.IsNullOrEmpty(connect.ProxyAddress))
                            {
                                data.Proxy = $"{connect.ProxyAddress}:{connect.ProxyPort}";
                            }
                            var e = SetGeoCod(data, connect);

                            if (e != null)
                            {
                                if (e.Message == _errorLimit || e.Message == _errorTimeIsUp)
                                {
                                    error = e;
                                    pl.Break();
                                }
                                else
                                {
                                    if (++countError >= _geoCodSettings.MaxCountError)
                                    {
                                        error = new Exception(_errorLotOfMistakes);
                                        pl.Break();
                                    }
                                }
                            }
                            else
                            {
                                countError = 0;
                            }
                        });
                    }
                    else
                    {
                        ParallelOptions po = new ParallelOptions
                        {
                            CancellationToken = _cts.Token,
                            MaxDegreeOfParallelism = _geoCodSettings.CountProxy
                        };

                        var parallelResult = Parallel.ForEach(_netSettings.CollectionListProxy, po, (data, pl) =>
                        {
                            EntityGeoCod geo = null;
                            var connect = GetConnect(data);
                            int countError = 0;

                            while (data.IsActive)
                            {
                                if (po.CancellationToken.IsCancellationRequested)
                                {
                                    break;
                                }
                                lock (_lock)
                                {
                                    geo = collectionGeoCod.FirstOrDefault(x => x.Status == StatusType.NotProcessed
                                                                      || (x.Status == StatusType.Error && (x.Error != _errorGeoCodFoundResultMoreOne
                                                                                                        && x.Error != _errorGeoCodNotFound)));
                                    if (geo != null)
                                    {
                                        geo.Status = StatusType.Processed;
                                    }
                                }

                                if (geo != null)
                                {
                                    geo.Proxy = $"{data.Address}:{data.Port}";
                                    var e = SetGeoCod(geo, connect);
                                    if (e != null)
                                    {
                                        if (e.Message == _errorLimit)
                                        {
                                            data.IsActive = false;
                                            data.Error = _errorLimit;
                                        }
                                        else
                                        {
                                            if (++countError >= _geoCodSettings.MaxCountError)
                                            {
                                                data.Error = _errorLotOfMistakes;
                                                data.IsActive = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        countError = 0;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        });
                    }
                }
                catch (OperationCanceledException c)
                {
                    error = c;
                }
                catch (Exception ex)
                {
                    error = ex;
                }
                finally
                {
                    _cts?.Dispose();
                }

                return error;
            }, _cts.Token);
        }

        #endregion PublicMethod

        public GeoCodingModel(NetSettings netSettings, GeoCodSettings geoCodSettings, PolygonViewModel polygon)
        {
            _netSettings = netSettings;
            _geoCodSettings = geoCodSettings;
            _polygon = polygon;

            GetLimitCount();
        }
    }
}