// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;
using GeoCoding.GeoCodingService;
using GeoCoding.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoding
{
    /// <summary>
    /// Класс для работы с геокодированием
    /// </summary>
    public class GeoCodingModel
    {
        public GeoCodingModel(NetSettings netSettings, GeoCodSettings geoCodSettings, LimitsModel limitsModel)
        {
            _netSettings = netSettings;
            _geoCodSettings = geoCodSettings;
            _limitsModel = limitsModel;
            //_polygon = polygon;
        }

        #region PrivateField

        private const string _errorTimeIsOver = "Время вышло";
        private const string _errorGeoCodNotFound = "Адрес не найден";
        private const string _errorGeoCodFoundResultMoreOne = "Количество результатов больше 1. Нужны уточнения";
        private const string _errorLimit = "Ваш лимит исчерпан";
        private const string _errorTimeIsUp = "Скорее всего упал сайт";
        private const string _errorLotOfMistakes = "Очень много ошибок при обработке данных. Обработка прекращена";
        private IGeoCodingService _geoCodingService;
        private CancellationTokenSource _cts;

        private readonly NetSettings _netSettings;
        private readonly GeoCodSettings _geoCodSettings;
        private readonly LimitsModel _limitsModel;
        //private readonly PolygonViewModel _polygon;
        private readonly object _lock = new object();
        private readonly DateTime _timeStopSpendingAllLimits = new DateTime(1, 1, 1, 3, 59, 0);
        private bool _isGoodGeoCoder;
        private bool _isStartSaveLimit;

        #endregion PrivateField

        #region PrivateMethod

        /// <summary>
        /// Метод для проверки можно ли геокодировать и не вышли ли за лимит
        /// </summary>
        /// <returns></returns>
        private Exception CheckCanGeo()
        {
            var key = _geoCodingService.GetKeyApi();
            var canGeo = _limitsModel.CanGeo(key);

            if (!canGeo)
            {
                var e = new Exception(_errorLimit);
                return e;
            }

            return null;
        }

        /// <summary>
        /// Функция для получения координат для объекта
        /// </summary>
        /// <param name="data">Объект</param>
        /// <returns>Возвращает ошибку</returns>
        private Exception SetGeoCod(EntityGeoCod data, ConnectSettings cs)
        {
            Exception error = null;
            string errorMsg = string.Empty;

            data.Status = StatusType.Processed;
            data.GeoCoder = _geoCodSettings.CurrentGeoCoder.GeoCoder;

            if (!string.IsNullOrEmpty(cs.ProxyAddress))
            {
                data.Proxy = $"{cs.ProxyAddress}:{cs.ProxyPort}";
            }

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
                        if (data.CountResult > 1)
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

            }, data.Address, cs, GetPolygon(data.Address));

            return error;
        }

        /// <summary>
        /// Метод для выбора полигона по адресу
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <returns>Координаты полигона</returns>
        private List<double> GetPolygon(string address)
        {
            //if(_geoCodSettings.CanUsePolygon)
            //{
            //    return _polygon.GetPolygon(address);
            //}

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
                        Longitude = g.Longitude.Replace(',', '.'),
                        MatchQuality = g.MatchQuality
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
                    else if (g.Precision.ToLower() == "true")
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

        /// <summary>
        /// Метод для сохранения текущего лимита
        /// </summary>
        private void SaveLimit()
        {
            if (_isStartSaveLimit) return;

            Task.Factory.StartNew(() =>
            {
                _isStartSaveLimit = true;
                var result = _limitsModel.SetLastUseLimits();
                _isStartSaveLimit = false;
            });
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
        /// Метод для получения координат для множества объектов
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="collectionGeoCod">Множество объектов</param>
        public async void GetAllGeoCod(Action<Exception> callback, IEnumerable<EntityGeoCod> collectionGeoCod)
        {
            if (!_isGoodGeoCoder)
            {
                callback(new Exception("Key is error"));
                return;
            }

            Exception error = null;

            var r = await _limitsModel.InitApiKey(_geoCodingService.GetKeyApi());
            if (!r.Successfully)
            {
                callback(r.Error);
                return;
            }

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
                            var e = CheckCanGeo();

                            if (e == null)
                            {
                                var errorSetGeo = SetGeoCod(data, connect);
                                if (errorSetGeo != null)
                                {
                                    if (errorSetGeo.Message == _errorLimit || errorSetGeo.Message == _errorTimeIsUp)
                                    {
                                        error = errorSetGeo;
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
                            }
                            else
                            {
                                error = e;
                                pl.Break();
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

                                    var e = CheckCanGeo();

                                    if (e == null)
                                    {
                                        e = SetGeoCod(geo, connect);
                                    }
                                    else
                                    {
                                        SetError(geo, e.Message);
                                        geo.CountResult = 0;
                                    }

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
            if (!_isGoodGeoCoder)
            {
                callback(new Exception("Key is error"));
                return;
            }

            Exception error = null;
            data.MainGeoCod = null;
            data.CountResult = 0;

            await _limitsModel.InitApiKey(_geoCodingService.GetKeyApi());

            await Task.Factory.StartNew(() =>
            {
                var error = CheckCanGeo();

                if (error == null)
                {
                    error = SetGeoCod(data, GetConnect());
                }
                else
                {
                    SetError(data, error.Message);
                    data.CountResult = 0;
                }
            });

            SaveLimit();

            callback(error);
        }

        /// <summary>
        /// Метод для получения урл-адреса запроса
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <returns>Возвращает ссылку на запрос</returns>
        public string GetUrlRequest(string address)
        {
            if (!_isGoodGeoCoder) return "Key is error";
            var key = _geoCodingService.GetKeyApi();
            var canGeo = _limitsModel.CanGeo(key);
            if (canGeo)
            {
                SaveLimit();
                return _geoCodingService.GetUrlRequest(address, GetPolygon(address));
            }

            return _errorLimit;
        }

        /// <summary>
        /// Метод установки текущего геосериса
        /// </summary>
        /// <param name="geoService">Ссылка на геосервис</param>
        public async Task<EntityResult<bool>> SetGeoService()
        {
            EntityResult<bool> result = new EntityResult<bool>();

            var g = _geoCodSettings.CurrentGeoCoder;
            var r = await _limitsModel.InitApiKey(g.Key);
            if (r.Successfully)
            {
                _geoCodingService = MainGeoService.GetServiceByName(g.GeoCoder, g.Key);
                result.Successfully = true;
                _isGoodGeoCoder = true;
            }
            else
            {
                _isGoodGeoCoder = false;
                result.Error = r.Error;
            }

            return result;
        }

        /// <summary>
        /// Метод для получения текущего лимита
        /// </summary>
        /// <returns>Возвращает задачу с подсчетом текущего лимита</returns>
        public async Task<EntityResult<int>> GetCurrentLimit()
        {
            var key = _geoCodingService.GetKeyApi();
            EntityResult<int> result;

            result = await _limitsModel.GetCurrentLimit(key);
            return result;
        }

        /// <summary>
        /// Метод для получения максимально возможного лимита
        /// </summary>
        /// <returns>Возвращает задачу с подсчетом максимально возможного лимита</returns>
        public async Task<EntityResult<int>> GetMaxLimit()
        {
            var key = _geoCodingService.GetKeyApi();
            EntityResult<int> result;

            result = await _limitsModel.GetMaxLimit(key);
            return result;
        }

        /// <summary>
        /// Метод для получения координат для множества объектов с максимальным лимитом по ключу
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="collectionGeoCod">Множество объектов</param>
        public async void GetAllGeoCodMaxLimit(Action<Exception> callback, IEnumerable<EntityGeoCod> collectionGeoCod)
        {
            if (!_isGoodGeoCoder)
            {
                callback(new Exception("Key is error"));
                return;
            }

            Exception error = null;

            var r = await _limitsModel.GetMaxLimit(_geoCodingService.GetKeyApi());
            if (!r.Successfully)
            {
                callback(r.Error);
                return;
            }
            else if (r.Entity < 1)
            {
                callback(new Exception(_errorLimit));
                return;
            }

            _cts = new CancellationTokenSource();

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
                        var isStop = CheckTimeStop();
                        if (isStop != null)
                        {
                            error = isStop;
                            pl.Break();
                        }
                        else
                        {
                            var e = CheckCanGeoMaxLimit();

                            if (e == null)
                            {
                                var errorSetGeo = SetGeoCod(data, connect);
                                if (errorSetGeo != null)
                                {
                                    if (errorSetGeo.Message == _errorLimit || errorSetGeo.Message == _errorTimeIsUp)
                                    {
                                        error = errorSetGeo;
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
                            }
                            else
                            {
                                error = e;
                                pl.Break();
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

            callback(error);
        }
        
        #endregion PublicMethod

        /// <summary>
        /// Метод проверки времени останова
        /// </summary>
        /// <returns>Возвращает ошибку если нужно остановится</returns>
        private Exception CheckTimeStop()
        {
            var dateTimeNow = DateTime.Now.TimeOfDay;
            if (dateTimeNow > _timeStopSpendingAllLimits.TimeOfDay)
            {
                return new Exception(_errorTimeIsOver);
            }

            return null;
        }

        private Exception CheckCanGeoMaxLimit()
        {
            throw new NotImplementedException();
        }
    }
}