using GeoCoding.FileService;
using GeoCoding.GeoCodingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoding
{
  /// <summary>
  /// Класс модель(работа с модулями файл, геокодирование, база данных) для основного окна
  /// </summary>
    public class MainWindowModel
    {
        #region PrivateConst
        private const char _charSplit = ';';
        private const string _errorIsNotFirstStringNameColumn = "Первая строка данных не название столбцов. Обработка прекращена.";
        private const string _errorLotOfMistakes = "Очень много ошибок при обработке данных. Обработка прекращена";
        private const string _errorGeoCodResponsEmpty = "Запрос к сервису геокодирования вернул пустой ответ";
        private const string _errorGeoCodFoundResultMoreOne = "Количество результатов больше 1. Следует проверить адрес";
        private const string _errorGeoCodNotFound = "Адрес не найден";
        private const string _errorFileNotHaveData = "Файл не содержит обрабатываемых данных";
        private const string _errorIsFormatIDWrong = "Формат значения GlobalId неверный";
        private const string _errorIsAddressEmpty = "Значение адреса пусто";
        private const string _errorIsFormatStringWrong = "Неверный формат строки";
        private const string _globalIDColumnNameLoadFile = "globalid";
        private const string _addressColumnNameLoadFile = "address";
        private const int _globalIDColumnIndexLoadFile = 0;
        private const int _addressColumnIndexLoadFile = 1;
        private const int _maxCountError = 100;
        #endregion PrivateConst

        private readonly IFileService _fileService = new FileService.FileService();

         private readonly IGeoCodingService _geoCodingService = new YandexGeoCodingService();
        //private readonly IGeoCodingService _geoCodingService = new GeoCodingService.Test.GeoCodingTest();

        private readonly string _nameColumnOutputFile = $"{_globalIDColumnNameLoadFile}{_charSplit}Latitude{_charSplit}Longitude{_charSplit}Qcode";
        private CancellationTokenSource _cts;
        private bool _isStartUpdateStatistic = false;

        /// <summary>
        /// Метод для получения(выбора) файла с данными
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: полное имя файла и ошибка</param>
        /// <param name="defaultFolder">Имя папки по умолчанию для открытия</param>
        public void GetFile(Action<string, Exception> callback, string defaultFolder = "")
        {
            Exception error = null;
            string file = string.Empty;

            _fileService.GetFile((f, e) =>
            {
                file = f;
                error = e;
            }, defaultFolder);

            callback(file, error);
        }

        /// <summary>
        /// Метод для получения(выбора) файла для сохранения данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: полное имя файла и ошибка</param>
        /// <param name="defaultName">Имя файла по умолчанию</param>
        public void SetFileForSave(Action<string, Exception> callback, string defaultName = "")
        {
            Exception error = null;
            string file = string.Empty;

            _fileService.SetFileForSave((f, e) =>
            {
                file = f;
                error = e;
            }, defaultName);

            callback(file, error);
        }

        /// <summary>
        /// Метод для получения данных из файла
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: множество объектов и ошибка</param>
        /// <param name="file">Полное имя файла с данными</param>
        public void GetDataFromFile(Action<IEnumerable<EntityGeoCod>, Exception> callback, string file)
        {
            Exception error = null;
            List<EntityGeoCod> data = null;

            _fileService.GetData((d, er) =>
            {
                error = er;
                if (error == null)
                {
                    StringToEntityGeoCod((list, e) =>
                    {
                        error = e;
                        if (list != null && list.Any())
                        {
                            data = list.ToList();
                        }
                    }, d);
                }
            }, file);

            if (error == null && data == null)
            {
                error = new Exception(_errorFileNotHaveData);
            }

            callback(data, error);
        }

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
                error = GetGeo(data);
            });

            callback(error);
        }

        /// <summary>
        /// Метод для получения координат для множеста вобъектов
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: завершился ли процесc, на каком номере завершился, ошибка</param>
        /// <param name="collectionGeoCod">Множество объектов</param>
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

                        var e = GetGeo(data);
                        if (e != null)
                        {
                            error = e;
                            if (countError++ >= _maxCountError)
                            {
                                error = new Exception(_errorLotOfMistakes);
                                pl.Break();
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
        /// Метод для сохранения данных в файл
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Множество данных для записи</param>
        /// <param name="file">Файл куда записывать</param>
        public void SaveData(Action<Exception> callback, IEnumerable<EntityGeoCod> data, string file)
        {
            Exception error = null;

            var list = new List<string>()
            {
                _nameColumnOutputFile
            };

            list.AddRange(data.Select(x =>
            {
                return $"{x.GlobalID}{_charSplit}{x.Latitude}{_charSplit}{x.Longitude}{_charSplit}{x.Qcode}";
            }));

            _fileService.SaveData(er =>
            {
                error = er;
            }, list, file);

            callback(error);
        }

        /// <summary>
        /// Метод для открытия папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="str">Путь к папке/файлу</param>
        public void OpenFolder(Action<Exception> callback, string str)
        {
            Exception error = null;

            _fileService.OpenFolder(e =>
            {
                error = e;
            }, str);

            callback(error);
        }

        /// <summary>
        /// Метод остановки процесса
        /// </summary>
        public void StopGet()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// Метод для открытия папки
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром статистика, ошибка</param>
        /// <param name="data">Множество объектов по которым считается статистика</param>
        public async void UpdateStatistic(Action<Statistics, Exception> callback, IEnumerable<EntityGeoCod> data)
        {
            Exception error = null;
            Statistics statistics = null;

            if(!_isStartUpdateStatistic)
            {
               _isStartUpdateStatistic = true;
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        statistics = new Statistics()
                        {
                            AllEntity = data.Count(),
                            OK = data.Count(x => x.Status == StatusType.OK),
                            NotGeoCoding = data.Count(x => x.Status == StatusType.NotGeoCoding),
                            GeoCodingNow = data.Count(x => x.Status == StatusType.GeoCodingNow),
                            Error = data.Count(x => x.Status == StatusType.Error),
                            House = data.Count(x => x.Kind == KindType.House),
                            Exact = data.Count(x => x.Precision == PrecisionType.Exact)
                        };
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }

                    _isStartUpdateStatistic = false;
                    callback(statistics, error);
                });
            }
        }

        /// <summary>
        /// Метод для установки свойств объекта
        /// </summary>
        /// <param name="data">Объект</param>
        /// <param name="geocod">Свойства объекта</param>
        private void SetDataGeoCod(EntityGeoCod data, GeoCod geocod)
        {
            data.CountResult = geocod.CountResult;

            if (geocod.CountResult == 1)
            {
                if (Enum.TryParse(geocod.Kind.ToUpperFistChar(), out KindType kind))
                {
                    data.Kind = kind;
                }
                if (Enum.TryParse(geocod.Precision.ToUpperFistChar(), out PrecisionType precision))
                {
                    data.Precision = precision;
                }
                data.AddressWeb = geocod.Text;
                data.Latitude = geocod.Latitude;
                data.Longitude = geocod.Longitude;
                if (data.Precision == PrecisionType.Exact)
                {
                    data.Qcode = 1;
                }
                else
                {
                  data.Qcode = 2;
                }
            }
        }

        /// <summary>
        /// Метод преобразования строки в объект EntityGeoCod
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: множество объектов EntityGeoCod и ошибка</param>
        /// <param name="d">Множество строк</param>
        private void StringToEntityGeoCod(Action<IEnumerable<EntityGeoCod>, Exception> callback, IEnumerable<string> d)
        {
            Exception error = null;
            List<EntityGeoCod> data = null;
            int countError = -1;

            if (d != null && d.Any())
            {
                if (IsFirstStringNameColumn(d.First()))
                {
                    data = new List<EntityGeoCod>(d.Count());
                    foreach (var item in d.Skip(1))
                    {
                        if (countError >= _maxCountError)
                        {
                            error = new Exception(_errorLotOfMistakes);
                            data = null;
                            break;
                        }

                        EntityGeoCod geocod = new EntityGeoCod();
                        var s = item.Split(_charSplit);

                        try
                        {
                            if (s.Length >= 2)
                            {
                                if (int.TryParse(s[_globalIDColumnIndexLoadFile].Trim(), out int id))
                                {
                                    geocod.GlobalID = id;
                                }
                                else
                                {
                                    SetError(geocod, _errorIsFormatIDWrong);
                                    countError++;
                                }

                                if (!string.IsNullOrEmpty(s[_addressColumnIndexLoadFile]))
                                {
                                    geocod.Address = s[_addressColumnIndexLoadFile];
                                }
                                else
                                {
                                    SetError(geocod, _errorIsAddressEmpty);
                                    countError++;
                                }
                            }
                            else
                            {
                                SetError(geocod, _errorIsFormatStringWrong);
                                countError++;
                            }
                        }
                        catch (Exception ex)
                        {
                            SetError(geocod, ex.Message);
                            countError++;
                        }

                        data.Add(geocod);
                    }
                }
                else
                {
                    error = new Exception(_errorIsNotFirstStringNameColumn);
                }
            }

            callback(data, error);
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
        /// Функция проверки правильности заголовка у файла с данными
        /// </summary>
        /// <param name="fs">Строка заголовка</param>
        /// <returns>Возвращает true если все верно</returns>
        private bool IsFirstStringNameColumn(string fs)
        {
            bool result = false;

            var str = fs.Split(_charSplit);
            if (str.Length >= 2)
            {
                if (str[_globalIDColumnIndexLoadFile].ToLower() == _globalIDColumnNameLoadFile && str[_addressColumnIndexLoadFile].ToLower() == _addressColumnNameLoadFile)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Функция для получения координат для объекта
        /// </summary>
        /// <param name="data">Объект</param>
        /// <returns>Возвращает ошибку</returns>
        private Exception GetGeo(EntityGeoCod data)
        {
            Exception error = null;

            string errorMsg = string.Empty;
            data.Status = StatusType.GeoCodingNow;

            _geoCodingService.GetGeoCod((geocod, er) =>
            {
                error = er;
                if (error == null)
                {
                    if (geocod != null)
                    {
                        SetDataGeoCod(data, geocod);
                        if (data.CountResult == 1)
                        {
                            data.Status = StatusType.OK;
                        }
                        else if (data.CountResult == 0)
                        {
                            errorMsg = _errorGeoCodNotFound;
                        }
                        else
                        {
                            errorMsg = _errorGeoCodFoundResultMoreOne;
                        }
                    }
                    else
                    {
                        errorMsg = _errorGeoCodResponsEmpty;
                    }
                }
                else
                {
                    errorMsg = er.Message;
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    SetError(data, errorMsg);
                }

            }, data.Address);
            return error;
        }
    }
}
