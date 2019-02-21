using GeoCoding.BDService;
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
        private const string _errorGeoCodResponsEmpty = "Запрос к сервису геокодирования вернул пустой ответ";
        private const string _errorGeoCodFoundResultMoreOne = "Количество результатов больше 1. Нужны уточнения";
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
        private readonly IBDService _bdService = new BDPostgresql();
        private readonly IFtpService _ftpService = new FtpService();

        private readonly string _nameColumnOutputFile = $"{_globalIDColumnNameLoadFile}{_charSplit}Latitude{_charSplit}Longitude{_charSplit}Qcode";
        private readonly string _nameColumnErrorFile = $"{_globalIDColumnNameLoadFile}{_charSplit}{_addressColumnNameLoadFile}{_charSplit}error";
        private readonly string _nameColumnTempFile = $"{_globalIDColumnNameLoadFile}{_charSplit}{_addressColumnNameLoadFile}{_charSplit}AddressWeb{_charSplit}Longitude{_charSplit}Latitude" +
                                                         $"{_charSplit}Qcode{_charSplit}Error{_charSplit}Status{_charSplit}DateTimeGeoCod{_charSplit}Kind{_charSplit}Precision{_charSplit}CountResult";
        private readonly string _nameColumnStatisticsFile = $"DateTime{_charSplit}User{_charSplit}System{_charSplit}FileInput{_charSplit}FileOutput{_charSplit}FileError{_charSplit}AllEntity" +
                                                             $"{_charSplit}OK{_charSplit}Error{_charSplit}NotGeoCoding{_charSplit}GeoCodingNow{_charSplit}House" +
                                                                $"{_charSplit}Exact{_charSplit}NotFound{_charSplit}TimeGeoCod";

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowModel()
        {
            var p = Properties.Settings.Default;

            string[] nameFolders = new string[] { p.FolderTemp, p.FolderInput, p.FolderOutput, p.FolderErrors, p.FolderStatistics };
            string path = Environment.CurrentDirectory;
            foreach (var item in nameFolders)
            {
                _fileService.CreateFolder(e =>
                {
                    if (e == null)
                    {

                    }
                }, $"{path}/{item}");
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

        public void SaveError(Action<Exception> callback, IEnumerable<EntityGeoCod> data, string file)
        {
            Exception error = null;
            List<string> list = null;

            if (!string.IsNullOrEmpty(file) && data != null && data.Count() > 0)
            {
                list = new List<string>(data.Count())
                {
                    _nameColumnErrorFile
                };

                list.AddRange(data.Select(x =>
                {
                    return $"{x.GlobalID}{_charSplit}{x.Address}{_charSplit}{x.Error}";
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
                    return $"{x.GlobalID}{_charSplit}{x.Address}{_charSplit}{x.MainGeoCod?.AddressWeb}{_charSplit}{x.MainGeoCod?.Longitude}{_charSplit}{x.MainGeoCod?.Latitude}" +
                    $"{_charSplit}{x.MainGeoCod?.Qcode}{_charSplit}{x.Error}{_charSplit}{x.Status}{_charSplit}{x.DateTimeGeoCod}{_charSplit}{x.MainGeoCod?.Kind}" +
                    $"{_charSplit}{x.MainGeoCod?.Precision}{_charSplit}{x.CountResult}";
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

        public void SaveStatistics(Action<Exception> callback, Statistics stat, FilesSettings files, string file)
        {
            Exception error = null;
            string[] data = null;
            string row = $"{DateTime.Now}{_charSplit}{Environment.UserName}{_charSplit}{stat.GeoServiceName}{_charSplit}{files.FileInput}{_charSplit}{files.FileOutput}{_charSplit}{_charSplit}{stat.AllEntity}" +
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
            }, file);

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
        /// Метод для получения настроек приложения
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: ошибка, настройки файлов, настройки геокодирования, настройки фтп-сервера</param>
        public void GetSettings(Action<Exception, FilesSettings, GeoCodSettings, FTPSettings, BDSettings, string> callback)
        {
            Exception error = null;
            var p = Properties.Settings.Default;
            var curDir = Environment.CurrentDirectory;
            string color = p.ColorTheme;

            FilesSettings f = new FilesSettings()
            {
                CanBreakFileOutput = p.CanBreakFileOutput,
                CanCopyFileOutputToFtp = p.CanCopyFileOutputToFtp,
                FolderInput = $"{curDir}\\{p.FolderInput}",
                FolderOutput = $"{curDir}\\{p.FolderOutput}",
                FolderTemp = $"{curDir}\\{p.FolderTemp}",
                FolderStatistics = $"{curDir}\\{p.FolderStatistics}",
                FolderErrors = $"{curDir}\\{p.FolderErrors}",
                IsFileInputOnFTP = p.IsFileInputOnFTP,
                MaxSizePart = p.MaxSizePart,
                CanGetDataOnce = p.CanGetDataOnce
            };

            GeoCodSettings g = new GeoCodSettings()
            {
                CanGeoCodGetAll = p.CanGeoCodGetAll,
                CanGeoCodGetError = p.CanGeoCodGetError,
                CanGeoCodGetNotGeo = p.CanGeoCodGetNotGeo,
                CanSaveDataAsFinished = p.CanSaveDataAsFinished,
                CanSaveDataAsTemp = p.CanSaveDataAsTemp,
                CanOpenFolderAfter = p.CanOpenFolderAfter,
                CanGeoCodAfterGetFile = p.CanGeoCodAfterGetFile,
                CanSaveStatistics = p.CanSaveStatistics,
                GeoService = p.GeoService
            };

            FTPSettings ftp = new FTPSettings()
            {
                Port = p.FtpPort,
                User = p.FtpUser,
                FolderInput = p.FtpFolderInput,
                FolderOutput = p.FtpFolderOutput
            };

            Helpers.ProtectedDataDPAPI.DecryptData((d, e) =>
            {
                if (e == null)
                {
                    ftp.Server = d;
                }
            }, p.FtpServer);

            Helpers.ProtectedDataDPAPI.DecryptData((d, e) =>
            {
                if (e == null)
                {
                    ftp.Password = d;
                }
            }, p.FtpPassword);

            BDSettings bds = new BDSettings()
            {
                BDName = p.BDName,
                Port = p.BDPort,
                Login = p.BDLogin,
                Password = p.BDPassword
            };

            Helpers.ProtectedDataDPAPI.DecryptData((d, e) =>
            {
                if (e == null)
                {
                    bds.Server = d;
                }
            }, p.BDServer);

            Helpers.ProtectedDataDPAPI.DecryptData((d, e) =>
            {
                if (e == null)
                {
                    bds.Password = d;
                }
            }, p.BDPassword);

            callback(error, f, g, ftp, bds, color);
        }

        /// <summary>
        /// Метод для сохранения настроек
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="filesSettings">Настройки файлов</param>
        /// <param name="ftpSettings">Настройки фтп-сервера</param>
        /// <param name="geoCodSettings">Настройки геокодирования</param>
        public void SaveSettings(Action<Exception> callback, FilesSettings filesSettings, FTPSettings ftpSettings, GeoCodSettings geoCodSettings, BDSettings bdSettings, string color)
        {
            Exception error = null;
            var p = Properties.Settings.Default;

            p.CanBreakFileOutput = filesSettings.CanBreakFileOutput;
            p.CanCopyFileOutputToFtp = filesSettings.CanCopyFileOutputToFtp;
            p.CanGeoCodGetAll = geoCodSettings.CanGeoCodGetAll;
            p.CanGeoCodGetError = geoCodSettings.CanGeoCodGetError;
            p.CanGeoCodGetNotGeo = geoCodSettings.CanGeoCodGetNotGeo;
            p.CanGetDataOnce = filesSettings.CanGetDataOnce;
            p.CanOpenFolderAfter = geoCodSettings.CanOpenFolderAfter;
            p.CanSaveDataAsFinished = geoCodSettings.CanSaveDataAsFinished;
            p.CanSaveDataAsTemp = geoCodSettings.CanSaveDataAsTemp;
            p.GeoService = geoCodSettings.GeoService;
            p.FtpFolderInput = ftpSettings.FolderInput;
            p.FtpFolderOutput = ftpSettings.FolderOutput;
            p.IsFileInputOnFTP = filesSettings.IsFileInputOnFTP;
            p.MaxSizePart = filesSettings.MaxSizePart;

            // ФТП-сервер пароль шифруем
            Helpers.ProtectedDataDPAPI.EncryptData((d, e) =>
            {
                if (e == null)
                {
                    p.FtpPassword = d;
                }
            }, ftpSettings.Password);

            // ФТП-сервер шифруем
            Helpers.ProtectedDataDPAPI.EncryptData((d, e) =>
            {
                if (e == null)
                {
                    p.FtpServer = d;
                }
            }, ftpSettings.Server);

            p.FtpPort = ftpSettings.Port;
            p.FtpUser = ftpSettings.User;
            p.CanGeoCodAfterGetFile = geoCodSettings.CanGeoCodAfterGetFile;
            p.CanSaveStatistics = geoCodSettings.CanSaveStatistics;
            p.ColorTheme = color;

            p.BDPort = bdSettings.Port;
            p.BDName = bdSettings.BDName;
            p.BDLogin = bdSettings.Login;

            // БД сервер шифруем
            Helpers.ProtectedDataDPAPI.EncryptData((d, e) =>
            {
                if (e == null)
                {
                    p.BDServer = d;
                }
            }, bdSettings.Server);

            // БД пароль шифруем
            Helpers.ProtectedDataDPAPI.EncryptData((d, e) =>
            {
                if (e == null)
                {
                    p.BDPassword = d;
                }
            }, bdSettings.Password);

            try
            {
                p.Save();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        public async void ConnectBDAsync(Action<Exception> callback, BDSettings bds)
        {
            Exception error = null;
            await Task.Factory.StartNew(() =>
            {
                _bdService.ConnectBD(e =>
                {
                    error = e;
                }, new BDService.ConnectionSettings()
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

                        data.Add(a);
                    }
                }
                    else
                {
                    error = e;
                }
                }, new BDService.ConnectionSettings()
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

        public void CopyFileOnFtp(Action<Exception> callback, FTPSettings ftps, string path)
        {
            Exception error = null;

            _ftpService.CopyFileOnFtp(e =>
            {
                error = e;
            }, GetConSettings(ftps), path);

            callback(error);
        }

        private FTPService.ConnectionSettings GetConSettings(FTPSettings ftps)
        {
            return new FTPService.ConnectionSettings()
            {
                Server = ftps.Server,
                Port = ftps.Port,
                Login = ftps.User,
                Password = ftps.Password,
                FolderInput = ftps.FolderInput,
                FolderOutput = ftps.FolderOutput
            };
        }
    }
}