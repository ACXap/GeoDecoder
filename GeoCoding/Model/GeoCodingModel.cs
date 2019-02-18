﻿using GeoCoding.GeoCodingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoding
{
    public class GeoCodingModel
    {
        private const string _errorGeoCodNotFound = "Адрес не найден";
        private const string _errorGeoCodFoundResultMoreOne = "Количество результатов больше 1. Нужны уточнения";
        private const string _errorLimit = "Ваш лимит исчерпан";
        private const string _errorLotOfMistakes = "Очень много ошибок при обработке данных. Обработка прекращена";
        private const int _maxCountError = 100;

        private IGeoCodingService _geoCodingService;

        private CancellationTokenSource _cts;

        /// <summary>
        /// Метод для получения координат объекта
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="data">Объект для которого нужны координаты</param>
        public async void GetGeoCod(Action<Exception> callback, EntityGeoCod data)
        {
            Exception error = null;
           
            await Task.Factory.StartNew(() =>
            {
                error = SetGeoCod(data);
            });

            callback(error);
        }

        private Exception SetGeoCod(EntityGeoCod data)
        {
            Exception error = null;
            string errorMsg = string.Empty;

            data.Status = StatusType.GeoCodingNow;

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
                    data.Error = e.Message;
                    data.Status = StatusType.Error;
                }

                data.DateTimeGeoCod = DateTime.Now;

            }, data.Address);

            return error;
        }

        public async void GetAllGeoCod(Action<bool, long?, Exception> callback, IEnumerable<EntityGeoCod> collectionGeoCod)
        {
            Exception error = null;
            bool result = false;
            long? indexStop = 0;
            int countError = 0;

            _cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions
            {
                CancellationToken = _cts.Token,
                MaxDegreeOfParallelism = 5
            };

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var parallelResult = Parallel.ForEach(collectionGeoCod, po, (data, pl) =>
                    {
                        po.CancellationToken.ThrowIfCancellationRequested();

                        var e = SetGeoCod(data);

                        if (e != null)
                        {
                            error = e;
                            if (e.Message == _errorLimit)
                            {
                                pl.Break();
                            }
                            else
                            {
                                if (countError++ >= _maxCountError)
                                {
                                    error = new Exception(_errorLotOfMistakes);
                                    pl.Break();
                                }
                            }
                        }
                    });

                    result = parallelResult.IsCompleted;
                    indexStop = parallelResult.LowestBreakIteration;
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
                    _cts.Dispose();
                }
            }, _cts.Token);

            callback(result, indexStop, error);
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
                    else if (g.Kind == "place")
                    {
                        data.Kind = KindType.Locality;
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
                    else
                    {
                        data.Precision = PrecisionType.Other;
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
        public void SetGeoService(IGeoCodingService geoService)
        {
            _geoCodingService = geoService;
        }
    }
}