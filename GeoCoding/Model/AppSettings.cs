using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GeoCoding.GeoCodingService;
using GeoCoding.Helpers;
using GeoCoding.Model.Data;
using GeoCoding.Model.Data.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GeoCoding.Model
{
    public class AppSettings : ViewModelBase
    {
        public AppSettings(MainWindowModel model)
        {
            _model = model;
            GetSettings();

            _notifications = new NotificationsModel(NotificationSettings);
            _modelVer = new VerificationModel(_verificationSettings.VerificationServer);

            GetApiKey();

            if (_netSettings.IsListProxy)
            {
                GetProxyList();
            }

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, data =>
            {
                if ((data.PropertyName == "IsNotProxy" || data.PropertyName == "IsSystemProxy" || data.PropertyName == "IsManualProxy") && data.NewValue)
                {
                    GeoCodSettings.IsMultipleRequests = true;
                }
            });
        }

        #region PrivateField
        MainWindowModel _model;
        VerificationModel _modelVer;
        INotifications _notifications;
        NetProxyModel _netProxyModel = new NetProxyModel();

        private ApiKeySettings _apiKeySettings;
        private FilesSettings _filesSettings;
        private GeoCodSettings _geoCodSettings;
        private FTPSettings _ftpSettings;
        private BDSettings _BDSettings;
        private GeneralSettings _generalSettings;
        private NotificationSettings _notificationSettings;
        private NetSettings _netSettings;
        private VerificationSettings _verificationSettings;
        #endregion PrivateField

        #region PublicProperties

        /// <summary>
        /// Настройки апи-ключей
        /// </summary>
        public ApiKeySettings ApiKeySettings
        {
            get => _apiKeySettings;
            set => Set(ref _apiKeySettings, value);
        }

        /// <summary>
        /// Настройки по работе с файлами
        /// </summary>
        public FilesSettings FilesSettings
        {
            get => _filesSettings;
            set => Set(ref _filesSettings, value);
        }

        /// <summary>
        /// Настройки по работе с геокодером
        /// </summary>
        public GeoCodSettings GeoCodSettings
        {
            get => _geoCodSettings;
            set => Set(ref _geoCodSettings, value);
        }

        /// <summary>
        /// настройки по работе с фтп-сервером
        /// </summary>
        public FTPSettings FTPSettings
        {
            get => _ftpSettings;
            set => Set(ref _ftpSettings, value);
        }

        /// <summary>
        /// Настройки по работе с базой данных
        /// </summary>
        public BDSettings BDSettings
        {
            get => _BDSettings;
            set => Set(ref _BDSettings, value);
        }

        /// <summary>
        /// Основные настройки приложения
        /// </summary>
        public GeneralSettings GeneralSettings
        {
            get => _generalSettings;
            set => Set(ref _generalSettings, value);
        }

        /// <summary>
        /// Настройки по оповещению
        /// </summary>
        public NotificationSettings NotificationSettings
        {
            get => _notificationSettings;
            set => Set(ref _notificationSettings, value);
        }

        /// <summary>
        /// Настройки по работе с сетью
        /// </summary>
        public NetSettings NetSettings
        {
            get => _netSettings;
            set => Set(ref _netSettings, value);
        }

        /// <summary>
        /// Настройки сервера проверки данных
        /// </summary>
        public VerificationSettings VerificationSettings
        {
            get => _verificationSettings;
            set => Set(ref _verificationSettings, value);
        }

        #endregion PublicProperties

        #region Command

        /// <summary>
        /// Поле для хранения ссылки на команду сохранения настроек
        /// </summary>
        private RelayCommand _commandSaveSettings;
        /// <summary>
        /// Команда для сохранения настроек
        /// </summary>
        public RelayCommand CommandSaveSettings =>
        _commandSaveSettings ?? (_commandSaveSettings = new RelayCommand(
                    () =>
                    {
                        SaveSettings();
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки соединения с базой
        /// </summary>
        private RelayCommand _commandCheckConnect;
        /// <summary>
        /// Команда для проверки соединения с базой
        /// </summary>
        public RelayCommand CommandCheckConnect =>
        _commandCheckConnect ?? (_commandCheckConnect = new RelayCommand(
                    () =>
                    {
                        BDSettings.StatusConnect = StatusType.Processed;
                        BDSettings.Error = string.Empty;
                        _model.ConnectBDAsync(e =>
                        {
                            if (e != null)
                            {
                                _notifications.Notification(NotificationType.Error, e);
                                BDSettings.StatusConnect = StatusType.Error;
                                BDSettings.Error = e.Message;
                            }
                            else
                            {
                                BDSettings.StatusConnect = StatusType.OK;
                                BDSettings.Error = string.Empty;
                            }
                        }, BDSettings);
                    }, () => !string.IsNullOrEmpty(_BDSettings.Server) || !string.IsNullOrEmpty(_BDSettings.BDName) || _BDSettings.StatusConnect == StatusType.Processed));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки соединения с фтп-сервером
        /// </summary>
        private RelayCommand _commandCheckConnectFtp;
        /// <summary>
        /// Команда для проверки соединения с фтп-сервером
        /// </summary>
        public RelayCommand CommandCheckConnectFtp =>
        _commandCheckConnectFtp ?? (_commandCheckConnectFtp = new RelayCommand(
                    () =>
                    {
                        FTPSettings.StatusConnect = StatusType.Processed;
                        FTPSettings.Error = string.Empty;
                        _model.ConnectFTPAsync(e =>
                        {
                            if (e != null)
                            {
                                _notifications.Notification(NotificationType.Error, e);
                                FTPSettings.StatusConnect = StatusType.Error;
                                FTPSettings.Error = e.Message;
                            }
                            else
                            {
                                FTPSettings.StatusConnect = StatusType.OK;
                                FTPSettings.Error = string.Empty;
                            }
                        }, FTPSettings);
                    }, () => !string.IsNullOrEmpty(_ftpSettings.Server) || _ftpSettings.StatusConnect == StatusType.Processed));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки подключения к сереру
        /// </summary>
        private RelayCommand _commandCheckServer;
        /// <summary>
        /// Команда проверки подключения к серверу
        /// </summary>
        public RelayCommand CommandCheckServer =>
            _commandCheckServer ?? (_commandCheckServer = new RelayCommand(
            () =>
            {
                _modelVer.SettingsService(_verificationSettings.VerificationServer);

                _verificationSettings.StatusConnect = StatusType.Processed;

                _modelVer.CheckServerAsync(e =>
                {
                    if (e != null)
                    {
                        _verificationSettings.Error = e.Message;
                        _verificationSettings.StatusConnect = StatusType.Error;
                    }
                    else
                    {
                        _verificationSettings.Error = string.Empty;
                        _verificationSettings.StatusConnect = StatusType.OK;
                    }
                });
            }, () => !string.IsNullOrEmpty(_verificationSettings.VerificationServer)));

        /// <summary>
        /// Поле для хранения команды получения списка прокси
        /// </summary>
        private RelayCommand _commandGetProxyList;
        /// <summary>
        /// Команда для получения списка прокси
        /// </summary>
        public RelayCommand CommandGetProxyList =>
        _commandGetProxyList ?? (_commandGetProxyList = new RelayCommand(
                    () =>
                    {
                        GetProxyList();
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки прокси
        /// </summary>
        private RelayCommand _commandTestProxy;
        /// <summary>
        /// Команда для проверки прокси
        /// </summary>
        public RelayCommand CommandTestProxy =>
        _commandTestProxy ?? (_commandTestProxy = new RelayCommand(
                    async () =>
                    {
                        await _netProxyModel.TestProxyAsync(_netSettings.Proxy);
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки списка прокси
        /// </summary>
        private RelayCommand _commandTestListProxy;
        /// <summary>
        /// Команда для проверки списка прокси
        /// </summary>
        public RelayCommand CommandTestListProxy =>
        _commandTestListProxy ?? (_commandTestListProxy = new RelayCommand(
                    async () =>
                    {
                        NetSettings.Status = StatusType.Processed;
                        await _netProxyModel.TestListProxyAsync(_netSettings.CollectionListProxy);
                        NetSettings.Status = StatusType.OK;
                    }, () => _netSettings.CollectionListProxy != null && _netSettings.CollectionListProxy.Any()));

        /// <summary>
        /// Поле для хранения ссылки на команду открытия папки
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;
        /// <summary>
        /// Команда открытия папки
        /// </summary>
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
                    str =>
                    {
                        OpenFolder(str);
                    }, str => !string.IsNullOrEmpty(str)));

        /// <summary>
        /// Поле для хранения ссылки на команду сохранения основных настроек
        /// </summary>
        private RelayCommand _commandSaveGeneralSettings;
        /// <summary>
        /// Команда для сохранения основных настроек приложения
        /// </summary>
        public RelayCommand CommandSaveGeneralSettings =>
        _commandSaveGeneralSettings ?? (_commandSaveGeneralSettings = new RelayCommand(
                    () =>
                    {
                        SaveGeneralSettings();
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду сохранения апи-ключей
        /// </summary>
        private RelayCommand _commandSaveApiKeys;
        /// <summary>
        /// Команда для сохранения апи ключей
        /// </summary>
        public RelayCommand CommandSaveApiKeys =>
        _commandSaveApiKeys ?? (_commandSaveApiKeys = new RelayCommand(
                    () =>
                    {
                        var c = _apiKeySettings.CollectionApiKeys.FirstOrDefault(k => k.ApiKey == _apiKeySettings.CurrentKey.ApiKey);
                        if (c == null)
                        {
                            _apiKeySettings.CollectionApiKeys.Add(_apiKeySettings.CurrentKey);
                        }
                        SaveApiKey();
                    }));

        /// <summary>
        /// Поле для хранения команды для добавления апи-ключа 
        /// </summary>
        private RelayCommand _commandAddApiKey;
        /// <summary>
        /// Команда для добавления апи-ключа
        /// </summary>
        public RelayCommand CommandAddApiKey =>
        _commandAddApiKey ?? (_commandAddApiKey = new RelayCommand(
                    () =>
                    {
                        _apiKeySettings.CurrentKey = new EntityApiKey();
                        new List<DayWeek>();

                        var listDayWeek = new List<DayWeek>();
                        foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                        {
                            listDayWeek.Add(new DayWeek() { Day = d });
                        }
                        var s = listDayWeek.First(x => x.Day == 0);
                        listDayWeek.Remove(s);
                        listDayWeek.Insert(listDayWeek.Count, s);

                        _apiKeySettings.CurrentKey.CollectionDayWeekSettings = listDayWeek;
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду удаления апи-ключа
        /// </summary>
        private RelayCommand _commandRemoveApiKey;
        /// <summary>
        /// Команда для удаления апи-ключа
        /// </summary>
        public RelayCommand CommandRemoveApiKey =>
        _commandRemoveApiKey ?? (_commandRemoveApiKey = new RelayCommand(
                    () =>
                    {
                        _apiKeySettings.CollectionApiKeys.Remove(_apiKeySettings.CurrentKey);
                        _apiKeySettings.CurrentKey = _apiKeySettings.CollectionApiKeys.FirstOrDefault();
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки потраченного лимита на сервере
        /// </summary>
        private RelayCommand _commandUpdateCurrentSpentServer;
        /// <summary>
        /// Команда проверки потраченного лимита на сервере
        /// </summary>
        public RelayCommand CommandUpdateCurrentSpentServer =>
        _commandUpdateCurrentSpentServer ?? (_commandUpdateCurrentSpentServer = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            var a = StatGeoCodingService.GetStat(_apiKeySettings.CurrentKey.ApiKeyDevelop, _apiKeySettings.CurrentKey.ApiKeyStat);
                            if (a != -1)
                            {
                                _apiKeySettings.CurrentKey.CurrentSpentServer = a;
                            }
                        }
                        catch(Exception ex)
                        {
                            _notifications.Notification(NotificationType.Error, ex.Message);
                        }
                        
                        
                    }));

        #endregion Command

        #region PrivateMethod

        /// <summary>
        /// Метод для получения настроек приложения
        /// </summary>
        private void GetSettings()
        {
            var p = Properties.Settings.Default;
            var curDir = Environment.CurrentDirectory;

            FilesSettings = new FilesSettings()
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
                CanGetDataOnce = p.CanGetDataOnce,
                CanUseANSI = p.CanUseANSI
            };

            GeoCodSettings = new GeoCodSettings()
            {
                CanGeoCodGetAll = p.CanGeoCodGetAll,
                CanGeoCodGetError = p.CanGeoCodGetError,
                CanGeoCodGetNotGeo = p.CanGeoCodGetNotGeo,
                CanSaveDataAsFinished = p.CanSaveDataAsFinished,
                CanSaveDataAsTemp = p.CanSaveDataAsTemp,
                CanOpenFolderAfter = p.CanOpenFolderAfter,
                CanGeoCodAfterGetFile = p.CanGeoCodAfterGetFile,
                CanSaveStatistics = p.CanSaveStatistics,
                GeoService = p.GeoService,
                IsMultipleProxy = p.IsMultipleProxy,
                IsMultipleRequests = p.IsMultipleRequests,
                CountProxy = p.CountProxy,
                CountRequests = p.CountRequests,
                MaxCountError = p.MaxCountError,
                CanUsePolygon = p.CanUsePolygon
            };

            FTPSettings = new FTPSettings()
            {
                Port = p.FtpPort,
                User = p.FtpUser,
                FolderInput = p.FtpFolderInput,
                FolderOutput = p.FtpFolderOutput
            };

            var res = ProtectedDataDPAPI.DecryptData(p.FtpServer);
            if (res.Successfully) FTPSettings.Server = res.Object;

            res = ProtectedDataDPAPI.DecryptData(p.FtpPassword);
            if (res.Successfully) FTPSettings.Password = res.Object;

            BDSettings = new BDSettings()
            {
                BDName = p.BDName,
                Port = p.BDPort,
                Login = p.BDLogin,
                Password = p.BDPassword
            };

            res = ProtectedDataDPAPI.DecryptData(p.BDServer);
            if (res.Successfully) BDSettings.Server = res.Object;

            res = ProtectedDataDPAPI.DecryptData(p.BDPassword);
            if (res.Successfully) BDSettings.Password = res.Object;

            NotificationSettings = new NotificationSettings()
            {
                CanNotificationDataEmpty = p.CanNotificationDataEmpty,
                CanNotificationDataProcessed = p.CanNotificationDataProcessed,
                CanNotificationOnlyError = p.CanNotificationOnlyError,
                CanNotificationProcessCancel = p.CanNotificationProcessCancel,
                CanNotificationSaveData = p.CanNotificationSaveData,
                CanNotificationSaveSettings = p.CanNotificationSaveSettings,
                CanNotificationStatAlreadySave = p.CanNotificationStatAlreadySave,
                CanNotificationExit = p.CanNotificationExit
            };

            NetSettings = new NetSettings()
            {
                IsNotProxy = p.IsNotProxy,
                IsSystemProxy = p.IsSystemProxy,
                IsManualProxy = p.IsManualProxy,
                IsListProxy = p.IsListProxy,
                Proxy = new ProxyEntity()
                {
                    Address = p.ProxyAddress,
                    Port = p.ProxyPort
                }
            };

            GeneralSettings = new GeneralSettings()
            {
                CanStartCompact = p.CanStartCompact,
                CanUseBdModule = p.CanUseBdModule,
                CanUseFtpModule = p.CanUseFtpModule,
                CanUseVerificationModule = p.CanUseVerificationModule,
                ColorTheme = p.ColorTheme,
                BackgroundGeo = p.BackgroundGeo,
                ScpriptBackgroundGeo = p.ScpriptBackgroundGeo,
                UseScriptBackGeo = p.UseScriptBackGeo
            };

            var listDayWeek = ObjectToStringJson.GetObjectOfstring<List<DayWeekWithTime>>(p.ListDayWeekMode);
            if (listDayWeek == null || !listDayWeek.Any())
            {
                listDayWeek = new List<DayWeekWithTime>();
                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                {
                    listDayWeek.Add(new DayWeekWithTime() { Day = d });
                }
            }
            var s = listDayWeek.First(x => x.Day == 0);
            listDayWeek.Remove(s);
            listDayWeek.Insert(listDayWeek.Count, s);

            GeneralSettings.ListDayWeek = listDayWeek;

            VerificationSettings = new VerificationSettings();

            res = ProtectedDataDPAPI.DecryptData(p.VerificationServer);
            if (res.Successfully) VerificationSettings.VerificationServer = res.Object;

            //res = ProtectedDataDPAPI.DecryptData(p.VerificationServerFactor);
            //if (res.Successfully) VerificationSettings.VerificationServerFactor = res.Object;
        }

        private void GetApiKey()
        {
            ApiKeySettings = new ApiKeySettings();

            _model.ReadFile((er, data) =>
            {
                if (er != null)
                {
                    ApiKeySettings.CollectionApiKeys = new ObservableCollection<EntityApiKey>();
                }
                else
                {
                    if (data != null && data.Any())
                    {
                        var a = ObjectToStringJson.GetObjectOfstring<IEnumerable<EntityApiKey>>(data.First());
                        if (a != null && a.Any())
                        {
                            ApiKeySettings.CollectionApiKeys = new ObservableCollection<EntityApiKey>(a);
                        }
                    }
                }
            }, _apiKeySettings.File);
        }

        /// <summary>
        /// Метод для получения списка прокси
        /// </summary>
        private void GetProxyList()
        {
            _netProxyModel.GetProxyList((d, e) =>
            {
                if (e == null)
                {
                    NetSettings.CollectionListProxy = new ObservableCollection<ProxyEntity>(d);
                }
                else
                {
                    _notifications.Notification("ProxyList", e.Message);
                }
            });
        }

        /// <summary>
        /// Метод для сохранения всех настроек
        /// </summary>
        private void SaveSettings()
        {
            var p = Properties.Settings.Default;

            if (_filesSettings != null)
            {
                p.CanBreakFileOutput = _filesSettings.CanBreakFileOutput;
                p.CanCopyFileOutputToFtp = _filesSettings.CanCopyFileOutputToFtp;
                p.IsFileInputOnFTP = _filesSettings.IsFileInputOnFTP;
                p.MaxSizePart = _filesSettings.MaxSizePart;
                p.CanGetDataOnce = _filesSettings.CanGetDataOnce;
                p.CanUseANSI = _filesSettings.CanUseANSI;
            }

            if (_ftpSettings != null)
            {
                p.FtpPort = _ftpSettings.Port;
                p.FtpUser = _ftpSettings.User;
                p.FtpFolderInput = _ftpSettings.FolderInput;

                var res = ProtectedDataDPAPI.EncryptData(_ftpSettings.Password);
                if (res.Successfully) p.FtpPassword = res.Object;

                res = ProtectedDataDPAPI.EncryptData(_ftpSettings.Server);
                if (res.Successfully) p.FtpServer = res.Object;
            }

            if (_geoCodSettings != null)
            {
                p.CanGeoCodGetAll = _geoCodSettings.CanGeoCodGetAll;
                p.CanGeoCodGetError = _geoCodSettings.CanGeoCodGetError;
                p.CanGeoCodGetNotGeo = _geoCodSettings.CanGeoCodGetNotGeo;
                p.CanOpenFolderAfter = _geoCodSettings.CanOpenFolderAfter;
                p.CanSaveDataAsFinished = _geoCodSettings.CanSaveDataAsFinished;
                p.CanSaveDataAsTemp = _geoCodSettings.CanSaveDataAsTemp;
                p.GeoService = _geoCodSettings.GeoService;
                p.IsMultipleProxy = _geoCodSettings.IsMultipleProxy;
                p.IsMultipleRequests = _geoCodSettings.IsMultipleRequests;
                p.CountProxy = _geoCodSettings.CountProxy;
                p.CountRequests = _geoCodSettings.CountRequests;
                p.MaxCountError = _geoCodSettings.MaxCountError;
                p.CanGeoCodAfterGetFile = _geoCodSettings.CanGeoCodAfterGetFile;
                p.CanSaveStatistics = _geoCodSettings.CanSaveStatistics;
                p.CanUsePolygon = _geoCodSettings.CanUsePolygon;
            }

            if (_generalSettings != null)
            {
                p.CanStartCompact = _generalSettings.CanStartCompact;
                p.CanUseVerificationModule = _generalSettings.CanUseVerificationModule;
                p.CanUseBdModule = _generalSettings.CanUseBdModule;
                p.CanUseFtpModule = _generalSettings.CanUseFtpModule;
                p.BackgroundGeo = _generalSettings.BackgroundGeo;
                p.UseScriptBackGeo = _generalSettings.UseScriptBackGeo;
                p.ScpriptBackgroundGeo = _generalSettings.ScpriptBackgroundGeo;
                p.ColorTheme = _generalSettings.ColorTheme;
                p.ListDayWeekMode = ObjectToStringJson.GetStringOfObject(_generalSettings.ListDayWeek);
            }

            if (_netSettings != null)
            {
                p.IsNotProxy = _netSettings.IsNotProxy;
                p.IsManualProxy = _netSettings.IsManualProxy;
                p.IsSystemProxy = _netSettings.IsSystemProxy;
                p.IsListProxy = _netSettings.IsListProxy;
                p.ProxyPort = _netSettings.Proxy.Port;
                p.ProxyAddress = _netSettings.Proxy.Address;
            }

            if (_BDSettings != null)
            {
                p.BDPort = _BDSettings.Port;
                p.BDName = _BDSettings.BDName;
                p.BDLogin = _BDSettings.Login;

                var res = ProtectedDataDPAPI.EncryptData(_BDSettings.Password);
                if (res.Successfully) p.BDPassword = res.Object;

                res = ProtectedDataDPAPI.EncryptData(_BDSettings.Server);
                if (res.Successfully) p.BDServer = res.Object;
            }

            if (_verificationSettings != null)
            {
                var res = ProtectedDataDPAPI.EncryptData(_verificationSettings.VerificationServer);
                if (res.Successfully) p.VerificationServer = res.Object;

                //res = ProtectedDataDPAPI.EncryptData(_verificationSettings.VerificationServerFactor);
                //if (res.Successfully) p.VerificationServerFactor = res.Object;
            }

            if (_notificationSettings != null)
            {
                p.CanNotificationProcessCancel = _notificationSettings.CanNotificationProcessCancel;
                p.CanNotificationDataEmpty = _notificationSettings.CanNotificationDataEmpty;
                p.CanNotificationDataProcessed = _notificationSettings.CanNotificationDataProcessed;
                p.CanNotificationSaveData = _notificationSettings.CanNotificationSaveData;
                p.CanNotificationSaveSettings = _notificationSettings.CanNotificationSaveSettings;
                p.CanNotificationStatAlreadySave = _notificationSettings.CanNotificationStatAlreadySave;
                p.CanNotificationOnlyError = _notificationSettings.CanNotificationOnlyError;
                p.CanNotificationExit = _notificationSettings.CanNotificationExit;
            }

            p.Save();
        }

        /// <summary>
        /// Метод для сохранения только основных настроек
        /// </summary>
        public void SaveGeneralSettings()
        {
            var p = Properties.Settings.Default;

            if (_generalSettings != null)
            {
                p.CanStartCompact = _generalSettings.CanStartCompact;
                p.CanUseVerificationModule = _generalSettings.CanUseVerificationModule;
                p.CanUseBdModule = _generalSettings.CanUseBdModule;
                p.CanUseFtpModule = _generalSettings.CanUseFtpModule;
                p.BackgroundGeo = _generalSettings.BackgroundGeo;
                p.UseScriptBackGeo = _generalSettings.UseScriptBackGeo;
                p.ScpriptBackgroundGeo = _generalSettings.ScpriptBackgroundGeo;
                p.ColorTheme = _generalSettings.ColorTheme;
                p.ListDayWeekMode = ObjectToStringJson.GetStringOfObject(_generalSettings.ListDayWeek);
            }

            p.Save();
        }

        /// <summary>
        /// Метод сохранения апи-ключей
        /// </summary>
        private void SaveApiKey()
        {
            var s = ObjectToStringJson.GetStringOfObject(_apiKeySettings.CollectionApiKeys);
            _model.SaveFile((e) =>
            {
                if (e != null) _notifications.Notification(NotificationType.Error, e.Message);
            }, new string[] { s }, _apiKeySettings.File);
        }

        /// <summary>
        /// Открыть папку
        /// </summary>
        /// <param name="str">Путь к файлу или папке</param>
        public void OpenFolder(string str)
        {
            if (str == "AppFolder")
            {
                str = Environment.CurrentDirectory;
            }
            if (str == "StatFolder")
            {
                str = _filesSettings.FolderStatistics;
            }
            _model.OpenFolder(e =>
            {
                _notifications.Notification(NotificationType.Error, e);
            }, str);
        }

        #endregion PrivateMethod

        #region PublicMethod
        /// <summary>
        /// Метод для получения модели информирования
        /// </summary>
        /// <returns></returns>
        public INotifications GetNotification()
        {
            return _notifications;
        }

        #endregion PublicMethod
    }
}