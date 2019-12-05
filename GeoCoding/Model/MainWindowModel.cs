// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.BDService;
using GeoCoding.Entities;
using GeoCoding.FileService;
using GeoCoding.FTPService;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string _errorFileNotHaveData = "Файл не содержит обрабатываемых данных";
        private const string _errorIsFormatIDWrong = "Формат значения GlobalId неверный";
        private const string _errorIsAddressEmpty = "Значение адреса пусто";
        private const string _errorIsFormatStringWrong = "Неверный формат строки";
        private const string _globalIDColumnNameLoadFile = "globalid";
        private const string _fiasGuidColumnNameLoadFile = "fiasguid";
        private const string _addressColumnNameLoadFile = "address";

        private const int _globalIDColumnIndexLoadFile = 0;
        private const int _fiasGuidColumnIndexLoadFile = 2;
        private const int _addressColumnIndexLoadFile = 1;
        private const int _maxCountError = 100;

        #endregion PrivateConst

        private readonly IFileService _fileService = new FileService.FileService();
        private readonly IBDService _bdService = new BDPostgresql();
        private readonly IFtpService _ftpService = new FtpService();

        //globalid;Latitude;Longitude;Qcode
        private readonly string _nameColumnOutputFile = $"{_globalIDColumnNameLoadFile}{_charSplit}Latitude{_charSplit}Longitude{_charSplit}Qcode";

        //globalid;address;error
        private readonly string _nameColumnErrorFile = $"{_globalIDColumnNameLoadFile}{_charSplit}{_addressColumnNameLoadFile}{_charSplit}error";

        //globalid;address;fiasguid;AddressWeb;Latitude;Longitude;Qcode;Error;Status;DateTimeGeoCod;Kind;Precision;CountResult;Proxy;GeoCoder
        private readonly string _nameColumnTempFile = $"{_globalIDColumnNameLoadFile}{_charSplit}{_addressColumnNameLoadFile}{_charSplit}{_fiasGuidColumnNameLoadFile}{_charSplit}AddressWeb{_charSplit}" +
                                                        $"Latitude{_charSplit}Longitude{_charSplit}Qcode{_charSplit}Error{_charSplit}Status{_charSplit}DateTimeGeoCod{_charSplit}Kind{_charSplit}" +
                                                         $"Precision{_charSplit}CountResult{_charSplit}Proxy{_charSplit}GeoCoder";

        private readonly string _nameColumnStatisticsFile = $"DateTime{_charSplit}User{_charSplit}System{_charSplit}FileInput{_charSplit}FileOutput{_charSplit}FileError{_charSplit}AllEntity" +
                                                             $"{_charSplit}OK{_charSplit}Error{_charSplit}NotGeoCoding{_charSplit}GeoCodingNow{_charSplit}House" +
                                                                $"{_charSplit}Exact{_charSplit}NotFound{_charSplit}TimeGeoCod";

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowModel()
        {
            CreateFolderStructure();
        }

        /// <summary>
        /// Метод для формирования структуры папок
        /// </summary>
        private void CreateFolderStructure()
        {
            var p = Properties.Settings.Default;
            string[] nameFolders = new string[] { p.FolderTemp, p.FolderInput, p.FolderOutput, p.FolderErrors, p.FolderStatistics };
            string path = Environment.CurrentDirectory;
            foreach (var item in nameFolders)
            {
                _fileService.CreateFolder(e =>
                { }, $"{path}/{item}");
            }
        }

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
        /// Метод для получения списка файлов
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром набор файлов, ошибка</param>
        /// <param name="defaultFolder">Имя папки по умолчанию</param>
        public void GetFiles(Action<IEnumerable<string>, Exception> callback, string defaultFolder = "")
        {
            Exception error = null;
            string[] data = null;

            _fileService.GetFiles((d, e) =>
            {
                data = d?.ToArray();
                error = e;
            }, defaultFolder);

            callback(data, error);
        }

        /// <summary>
        /// Метод для получения данных о файле
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Набор файлов</param>
        public async void GetDataAboutFiles(Action<Exception> callback, IEnumerable<EntityFile> data, bool canUseAnsi)
        {
            Exception error = null;
            string firstString = string.Empty;

            await Task.Factory.StartNew(() =>
            {
                foreach (var item in data)
                {
                    _fileService.GetCountRecord((i, e) =>
                    {
                        if (e == null)
                        {
                            item.Count = i;
                        }
                    }, item.NameFile);

                    _fileService.GetData((d, e) =>
                    {
                        if (e == null)
                        {
                            firstString = d.First();
                            if (IsFirstStringNameColumnTempFile(firstString))
                            {
                                item.FileType = FileType.Temp;
                            }
                            else if (IsFirstStringNameColumnErrorFile(firstString))
                            {
                                item.FileType = FileType.Error;
                            }
                            else if (IsFirstStringNameColumn(firstString))
                            {
                                item.FileType = FileType.Data;
                            }
                        }
                    }, item.NameFile, canUseAnsi);
                }

                callback(error);
            });

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
        public void GetDataFromFile(Action<IEnumerable<EntityGeoCod>, Exception> callback, string file, bool canUseAnsi)
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
            }, file, canUseAnsi);

            if (error == null && data == null)
            {
                error = new Exception(_errorFileNotHaveData);
            }

            callback(data, error);
        }

        /// <summary>
        /// Метод для сохранения данных в файл
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Множество данных для записи</param>
        /// <param name="file">Файл куда записывать</param>
        public void SaveData(Action<Exception> callback, IEnumerable<EntityGeoCod> data, string file, int maxSizePart, bool canCopyToFtp, FTPSettings ftps)
        {
            Exception error = null;
            List<string> list = new List<string>();

            if (maxSizePart == 0)
            {
                list.Add(_nameColumnOutputFile);

                list.AddRange(data.Where(y => y.Status == StatusType.OK).Select(x =>
                  {
                      return $"{x.GlobalID}{_charSplit}{x.MainGeoCod?.Latitude}{_charSplit}{x.MainGeoCod?.Longitude}{_charSplit}{x.MainGeoCod?.Qcode}";
                  }));

                _fileService.SaveData(er =>
                {
                    error = er;
                }, list, file);

                if (error == null && canCopyToFtp)
                {
                    CopyFileOnFtp(e =>
                    {
                        error = e;
                    }, ftps, file);
                }
            }
            else
            {
                var i = 1;

                var a = data.Partition(maxSizePart);
                try
                {
                    foreach (var item in a)
                    {
                        string nameFile = System.IO.Path.GetFileNameWithoutExtension(file);
                        string nameFolder = System.IO.Path.GetDirectoryName(file);
                        string ex = System.IO.Path.GetExtension(file);
                        string newNameFile = $"{nameFolder}\\{nameFile}_{i}{ex}";

                        list = new List<string>()
                        {
                            _nameColumnOutputFile
                        };

                        list.AddRange(item.Where(y => y.Status == StatusType.OK).Select(x =>
                        {
                            return $"{x.GlobalID}{_charSplit}{x.MainGeoCod?.Latitude}{_charSplit}{x.MainGeoCod?.Longitude}{_charSplit}{x.MainGeoCod?.Qcode}";
                        }));

                        _fileService.SaveData(er =>
                        {
                            error = er;
                        }, list, newNameFile);

                        if (error == null && canCopyToFtp)
                        {
                            CopyFileOnFtp(e =>
                            {
                                error = e;
                            }, ftps, newNameFile);
                        }

                        i++;
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            callback(error);
        }

        /// <summary>
        /// Метод для сохранения файла с ошибками
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Данные для записи</param>
        /// <param name="file">Имя файла</param>
        public void SaveError(Action<Exception> callback, IEnumerable<EntityGeoCod> data, string file)
        {
            if (data == null)
            {
                callback(null);
                return;
            }
            if (string.IsNullOrEmpty(file))
            {
                callback(new ArgumentNullException("Для сохранения файла с ошибками нужно имя файла"));
                return;
            }

            Exception error = null;
            List<string> list = null;
            var d = data.Where(x => x.Status == StatusType.Error);

            if (d.Count() > 0)
            {
                list = new List<string>(data.Count())
                {
                    _nameColumnErrorFile
                };

                list.AddRange(d.Select(x =>
                {
                    return $"{x.GlobalID}{_charSplit}{x.Address}{_charSplit}{x.Error}";
                }));

                _fileService.SaveData(er =>
                {
                    error = er;
                    callback(error);
                }, list, file);
            }
            else
            {
                callback(null);
                return;
            }
        }

        /// <summary>
        /// Метод для сохранения файла с временными данными
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="data">Данные для записи</param>
        /// <param name="file">Имя файла</param>
        public void SaveTemp(Action<Exception> callback, IEnumerable<EntityGeoCod> data, string file)
        {
            Exception error = null;
            List<string> list = null;

            if (!string.IsNullOrEmpty(file) && data != null && data.Count() > 0)
            {
                list = new List<string>(data.Count())
                {
                    _nameColumnTempFile
                };

                list.AddRange(data.Select(x =>
                {
                    return $"{x.GlobalID}{_charSplit}{x.Address}{_charSplit}{x.FiasGuid}{_charSplit}{x.MainGeoCod?.AddressWeb}{_charSplit}{x.MainGeoCod?.Latitude}{_charSplit}{x.MainGeoCod?.Longitude}" +
                    $"{_charSplit}{x.MainGeoCod?.Qcode}{_charSplit}{x.Error}{_charSplit}{x.Status}{_charSplit}{x.DateTimeGeoCod}{_charSplit}{x.MainGeoCod?.Kind}" +
                    $"{_charSplit}{x.MainGeoCod?.Precision}{_charSplit}{x.CountResult}{_charSplit}{x.Proxy}{_charSplit}{x.GeoCoder}";
                }));

                _fileService.SaveData(er =>
                {
                    error = er;
                }, list, file);
            }
            else
            {
                error = new ArgumentNullException();
            }

            callback(error);
        }

        /// <summary>
        /// Метод для сохранения статистики
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="stat">Статистика</param>
        /// <param name="files">Настройки файлов</param>
        /// <param name="file">Имя файла со статистикой</param>
        public void SaveStatistics(Action<Exception> callback, Statistics stat, FilesSettings files, string file)
        {
            Exception error = null;
            string[] data = null;
            string row = $"{DateTime.Now}{_charSplit}{Environment.UserName}{_charSplit}{stat.GeoServiceName}{_charSplit}{files.FileInput}{_charSplit}{files.FileOutput}{_charSplit}{files.FileError}{_charSplit}{stat.AllEntity}" +
                $"{_charSplit}{stat.OK}{_charSplit}{stat.Error}{_charSplit}{stat.NotGeoCoding}{_charSplit}{stat.GeoCodingNow}" +
                $"{_charSplit}{stat.House}{_charSplit}{stat.Exact}{_charSplit}{stat.NotFound}{_charSplit}{stat.TimeGeoCod}";

            _fileService.FileExists((exists, er) =>
           {
               if (er == null)
               {
                   if (exists)
                   {
                       data = new string[]
                       {
                            row
                       };
                   }
                   else
                   {
                       data = new string[]
                       {
                            _nameColumnStatisticsFile,
                            row
                       };
                   }

                   _fileService.AppendData(e =>
                   {
                       if (e != null)
                       {
                           error = e;
                       }
                   }, data, file);
               }
               else
               {
                   error = er;
               }
           }, file);

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
        /// Метод для получения настроек из файла
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="file">Имя файла</param>
        /// <param name="ftp">Настройки фтп-сервера</param>
        /// <param name="bd">Настройки подключения к базе данных</param>
        public void GetSettingsFromFile(Action<Exception> callback, string file, FTPSettings ftp, BDSettings bd)
        {
            Exception error = null;
            List<string> data = null;
            _fileService.GetData((d, e) =>
            {
                error = e;
                if (e == null && d != null)
                {
                    data = d.ToList();
                }
            }, file, false);

            foreach (var str in data)
            {
                if (str[0] == '#')
                {
                    continue;
                }
                var s = str.Split('=');
                switch (s[0])
                {
                    case "FtpServer":
                        ftp.Server = s[1];
                        break;

                    case "FtpPort":
                        int.TryParse(s[1], out int i);
                        ftp.Port = i;
                        break;

                    case "FtpOutput":
                        ftp.FolderOutput = s[1];
                        break;

                    case "FtpInput":
                        ftp.FolderInput = s[1];
                        break;

                    case "BdServer":
                        bd.Server = s[1];
                        break;

                    case "BdName":
                        bd.BDName = s[1];
                        break;

                    case "BdPort":
                        int.TryParse(s[1], out int k);
                        bd.Port = k;
                        break;

                    default:
                        break;
                }
            }

            callback(error);
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

                            if (d.First().Split(_charSplit).Length > 2 && d.First().Split(_charSplit)[2].ToLower() == _fiasGuidColumnNameLoadFile && Guid.TryParse(s[_fiasGuidColumnIndexLoadFile], out Guid guid))
                            {
                                geocod.FiasGuid = guid;
                            }

                            //    0        1       2        3         4       5          6    7      8        9           10    11      12           13     14
                            //globalid;address;fiasguid;AddressWeb;Latitude;Longitude;Qcode;Error;Status;DateTimeGeoCod;Kind;Precision;CountResult;Proxy;GeoCoder
                            if (IsFirstStringNameColumnTempFile(d.First()))
                            {
                                if (!string.IsNullOrEmpty(s[3]) && !string.IsNullOrEmpty(s[4]) && !string.IsNullOrEmpty(s[5]))
                                {
                                    geocod.MainGeoCod = new GeoCod()
                                    {
                                        AddressWeb = s[3],
                                        Longitude = s[5],
                                        Latitude = s[4],
                                    };
                                    byte.TryParse(s[6], out byte qcode);
                                    geocod.MainGeoCod.Qcode = qcode;
                                    Enum.TryParse(s[10], out KindType kt);
                                    geocod.MainGeoCod.Kind = kt;
                                    Enum.TryParse(s[11], out PrecisionType pt);
                                    geocod.MainGeoCod.Precision = pt;
                                }

                                geocod.Error = s[7];
                                Enum.TryParse(s[8], out StatusType a);
                                geocod.Status = a;
                                DateTime.TryParse(s[9], out DateTime dt);
                                geocod.DateTimeGeoCod = dt;
                                byte.TryParse(s[12], out byte c);
                                geocod.CountResult = c;
                                geocod.Proxy = s[13];
                                geocod.GeoCoder = s[14];
                            }
                            //    0        1      2
                            //globalid;address;error
                            else if (IsFirstStringNameColumnErrorFile(d.First()))
                            {
                                geocod.Error = s[2];
                                geocod.Status = StatusType.Error;
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
                if (str[_globalIDColumnIndexLoadFile].ToLower() == _globalIDColumnNameLoadFile
                    && str[_addressColumnIndexLoadFile].ToLower() == _addressColumnNameLoadFile)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Метод для проверки первой строки данных и сравнения со строкой файла с временными данными
        /// </summary>
        /// <param name="fs">Строка с названием столбцов в файле с временными данными</param>
        /// <returns></returns>
        private bool IsFirstStringNameColumnTempFile(string fs)
        {
            return fs.ToLower() == _nameColumnTempFile.ToLower() + ";District|City|Street|HouseNumber".ToLower();
        }

        /// <summary>
        /// Метод для проверки первой строки данных и сравнения со  строкой файла с ошибками
        /// </summary>
        /// <param name="fs">Строка с названием столбцов в файле с ошибками</param>
        /// <returns>Возвращает истину, если первая строка данных файла с ошибками</returns>
        private bool IsFirstStringNameColumnErrorFile(string fs)
        {
            return fs.ToLower() == _nameColumnErrorFile;
        }

        /// <summary>
        /// Метод для подключения к базе данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="bds">Настройки подключения к базе данных</param>
        public async void ConnectBDAsync(Action<Exception> callback, BDSettings bds)
        {
            Exception error = null;
            await Task.Factory.StartNew(() =>
            {
                _bdService.ConnectBD(e =>
                {
                    error = e;
                }, new ConnectionSettingsDb()
                {
                    Server = bds.Server,
                    BDName = bds.BDName,
                    Port = bds.Port,
                    Login = bds.Login,
                    Password = bds.Password
                });

                callback(error);
            });
        }

        /// <summary>
        /// Метод для получения данных из базы данных
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром, коллекция данных, ошибка</param>
        /// <param name="bds">Настройки подключения к базе данных</param>
        /// <param name="query">Sql-запрос к базе данных</param>
        public async void GetDataFromBDAsync(Action<IEnumerable<EntityGeoCod>, Exception> callback, BDSettings bds, string query)
        {
            Exception error = null;
            List<EntityGeoCod> data = new List<EntityGeoCod>();

            await Task.Factory.StartNew(() =>
            {
                _bdService.ExecuteUserQuery((d, e) =>
                {
                    if (e == null)
                    {
                        foreach (var item in d)
                        {
                            var a = new EntityGeoCod();

                            if (item.OrponId == 0)
                            {
                                SetError(a, _errorIsFormatIDWrong);
                            }
                            else
                            {
                                a.GlobalID = item.OrponId;
                            }

                            if (string.IsNullOrEmpty(item.Address))
                            {
                                SetError(a, _errorIsAddressEmpty);
                            }
                            else
                            {
                                a.Address = item.Address;
                            }
                            a.FiasGuid = item.FiasGuid;

                            data.Add(a);
                        }
                    }
                    else
                    {
                        error = e;
                    }
                }, new ConnectionSettingsDb()
                {
                    Server = bds.Server,
                    BDName = bds.BDName,
                    Port = bds.Port,
                    Login = bds.Login,
                    Password = bds.Password
                }, query);

                callback(data, error);
            });
        }

        /// <summary>
        /// Метод для тестового подключения к фтп-серверу
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="ftps">Настройки фтп-сервера</param>
        public async void ConnectFTPAsync(Action<Exception> callback, FTPSettings ftps)
        {
            Exception error = null;
            await Task.Factory.StartNew(() =>
            {
                _ftpService.ConnectFtp(e =>
                {
                    error = e;
                }, GetConSettings(ftps));

                callback(error);
            });
        }

        /// <summary>
        /// Метод для копирования файла на фтп-сервер
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром ошибка</param>
        /// <param name="ftps">Настройки фтп-сервера</param>
        /// <param name="path">Путь к файлу</param>
        public void CopyFileOnFtp(Action<Exception> callback, FTPSettings ftps, string path)
        {
            Exception error = null;

            _ftpService.CopyFileOnFtp(e =>
            {
                error = e;
            }, GetConSettings(ftps), path);

            callback(error);
        }

        /// <summary>
        /// Метод для формирования фтп-настроек в формате
        /// </summary>
        /// <param name="ftps">Настройки фтп-сервера</param>
        /// <returns>Возвращает настройки фтп-сервера в формате</returns>
        private ConnectionSettings GetConSettings(FTPSettings ftps)
        {
            return new ConnectionSettings()
            {
                Server = ftps.Server,
                Port = ftps.Port,
                Login = ftps.User,
                Password = ftps.Password,
                FolderInput = ftps.FolderInput,
                FolderOutput = ftps.FolderOutput
            };
        }

        public void SaveFile(Action<Exception> callback, string[] data, string file)
        {
            _fileService.SaveData(er =>
            {
                callback(er);
            }, data, file);
        }

        public void ReadFile(Action<Exception, IEnumerable<string>> callback, string file)
        {
            _fileService.GetData((data, er) =>
            {
                callback(er, data);
            }, file, false);
        }
    }
}