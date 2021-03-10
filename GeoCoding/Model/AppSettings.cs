// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GeoCoding.GeoCodingLimitsService;
using GeoCoding.Helpers;
using GeoCoding.Model.Data;
using GeoCoding.Model.Data.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCoding.Model
{
    public class AppSettings : ViewModelBase
    {
        public AppSettings(INotifications notifications)
        {
            _model = new MainWindowModel();
            _modelBd = new BdModel();
            GetSettings();

            _notifications = notifications;
            _notifications.SetSettings(_notificationSettings);
            _modelVer = new VerificationModel(_verificationSettings.VerificationServer);

            _mailModel = new MailModel(_notificationSettings);

            GetApiKey();

            _limitsModel = GetLimitsModel();

            GetCollectionGeocoder();
            SetCurrentGeocoder();
            GetProxyList();

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, data =>
            {
                if ((data.PropertyName == "IsNotProxy" || data.PropertyName == "IsSystemProxy" || data.PropertyName == "IsManualProxy") && data.NewValue)
                {
                    GeoCodSettings.IsMultipleRequests = true;
                }
            });
        }

        #region PrivateField
        private readonly MainWindowModel _model;
        private readonly MailModel _mailModel;
        private readonly BdModel _modelBd;
        private readonly VerificationModel _modelVer;
        private readonly INotifications _notifications;
        private readonly NetProxyModel _netProxyModel = new NetProxyModel();
        private readonly LimitsModel _limitsModel;

        private ApiKeySettings _apiKeySettings;
        private FilesSettings _filesSettings;
        private GeoCodSettings _geoCodSettings;
        private FTPSettings _ftpSettings;
        private BDSettings _BDSettings;
        private BDSettings _BDAccessorySettings;
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
        /// Настройки по работе с вспомогательной базой данных
        /// </summary>
        public BDSettings BDAccessorySettings
        {
            get => _BDAccessorySettings;
            set => Set(ref _BDAccessorySettings, value);
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

        private RelayCommand _commandCheckEmail;
        public RelayCommand CommandCheckEmail =>
        _commandCheckEmail ?? (_commandCheckEmail = new RelayCommand(
                    () =>
                    {
                        _mailModel.TestEmail((m)=>
                        {
                            TextStatusEmail = m;
                        });
                    }));


        private string _textStatusEmail = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string TextStatusEmail
        {
            get => _textStatusEmail;
            set => Set(ref _textStatusEmail, value);
        }

        /// <summary>
        /// Поле для хранения ссылки на команду проверки соединения с базой
        /// </summary>
        private RelayCommand _commandCheckConnect;
        /// <summary>
        /// Команда для проверки соединения с базой
        /// </summary>
        public RelayCommand CommandCheckConnect =>
        _commandCheckConnect ?? (_commandCheckConnect = new RelayCommand(
                    async () =>
                    {
                        BDSettings.StatusConnect = StatusType.Processed;
                        BDSettings.Error = string.Empty;

                        var result = await _modelBd.ConnectBDAsync(_BDSettings);

                        if (result.Successfully)
                        {
                            BDSettings.StatusConnect = StatusType.OK;
                            BDSettings.Error = string.Empty;
                        }
                        else
                        {
                            _notifications.Notification(NotificationType.Error, result.Error.Message);
                            BDSettings.StatusConnect = StatusType.Error;
                            BDSettings.Error = result.Error.Message;
                        }
                    }, () => !string.IsNullOrEmpty(_BDSettings.Server) && !string.IsNullOrEmpty(_BDSettings.BDName) && _BDSettings.StatusConnect != StatusType.Processed));

        /// <summary>
        /// Поле для хранения ссылки на команду проверки подключения к вспомогательной базе данных
        /// </summary>
        private RelayCommand _commandCheckConnectAccessoryBD;
        /// <summary>
        /// Команда для проверки подключения к вспомогательной базе данных
        /// </summary>
        public RelayCommand CommandCheckConnectAccessoryBD =>
        _commandCheckConnectAccessoryBD ?? (_commandCheckConnectAccessoryBD = new RelayCommand(
                    () =>
                    {
                        BDAccessorySettings.StatusConnect = StatusType.Processed;
                        BDAccessorySettings.Error = string.Empty;
                        Task.Factory.StartNew(() =>
                        {
                            var result = _limitsModel.CheckRepo();
                            if (result.Successfully)
                            {
                                BDAccessorySettings.StatusConnect = StatusType.OK;
                                BDAccessorySettings.Error = string.Empty;
                            }
                            else
                            {
                                BDAccessorySettings.StatusConnect = StatusType.Error;
                                BDAccessorySettings.Error = result.Error.Message;
                            }
                        });

                    }, () => !string.IsNullOrEmpty(_BDAccessorySettings.Server) && !string.IsNullOrEmpty(_BDAccessorySettings.BDName) && _BDAccessorySettings.StatusConnect != StatusType.Processed));

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
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                var result = _limitsModel.GetLastUseLimitsServer(_apiKeySettings.CurrentKey);
                                if (result.Successfully)
                                {
                                    _apiKeySettings.CurrentKey.CurrentSpentServer = result.Entity.Value;
                                }
                                else
                                {
                                    _notifications.Notification(NotificationType.Error, result.Error.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                _notifications.Notification(NotificationType.Error, ex.Message);
                            }
                        });
                    }, () =>
                    {
                        var a = _apiKeySettings.CurrentKey;
                        return a != null && !string.IsNullOrEmpty(a.ApiKeyDevelop) && !string.IsNullOrEmpty(a.ApiKeyStat);
                    }
                    ));

        /// <summary>
        /// Поле для хранения ссылки на команду сохранения геокодера
        /// </summary>
        private RelayCommand _commandSaveGeoCoder;
        /// <summary>
        /// Команда сохранения геокодеров
        /// </summary>
        public RelayCommand CommandSaveGeoCoder =>
        _commandSaveGeoCoder ?? (_commandSaveGeoCoder = new RelayCommand(
                    () =>
                    {
                        SaveGeoCoder();
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду добавления нового геокодера
        /// </summary>
        private RelayCommand _commandAddGeoCoder;
        /// <summary>
        /// Команда добавления новго геокодера
        /// </summary>
        public RelayCommand CommandAddGeoCoder =>
        _commandAddGeoCoder ?? (_commandAddGeoCoder = new RelayCommand(
                    () =>
                    {
                        if (_geoCodSettings.CollectionGeoCoder == null)
                        {
                            _geoCodSettings.CollectionGeoCoder = new ObservableCollection<EntityGeoCoder>();
                        }
                        int id = 0;
                        if (_geoCodSettings.CollectionGeoCoder.Any())
                        {
                            id = _geoCodSettings.CollectionGeoCoder.Select(x => x.Id).Max() + 1;
                        }
                        _geoCodSettings.CollectionGeoCoder.Add(new EntityGeoCoder() { Id = id, Name = "Новый" });
                    }));

        /// <summary>
        /// Поле для хранения ссылки на команду синхронизации ключа с базой
        /// </summary>
        private RelayCommand _commandSyncApiKey;
        /// <summary>
        /// Команда для синхронизации ключа с базой
        /// </summary>
        public RelayCommand CommandSyncApiKey =>
        _commandSyncApiKey ?? (_commandSyncApiKey = new RelayCommand(
        async () =>
        {
            var result = await _limitsModel.SyncApiKey(_apiKeySettings.CurrentKey);
            if (!result.Successfully && result.Error != null)
            {
                _notifications.Notification(NotificationType.Error, result.Error);
            }
        }));

        #endregion Command

        #region PrivateMethod

        /// <summary>
        /// Метод для получения модуля по работе с лимитами
        /// </summary>
        /// <returns></returns>
        public LimitsModel GetLimitsModel()
        {
            return new LimitsModel(new LimitsRepositoryOrpon(new Entities.ConnectionSettingsDb()
            {
                BDName = _BDSettings.BDName,
                Login = _BDSettings.Login,
                Password = _BDSettings.Password,
                Port = _BDSettings.Port,
                Server = _BDSettings.Server
            }), _apiKeySettings.CollectionApiKeys);
        }

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
                FolderOutput = p.FtpFolderOutput,
                Server = p.FtpServer
            };

            var res = ProtectedDataDPAPI.DecryptData(p.FtpPassword);
            if (res.Successfully) FTPSettings.Password = res.Entity;

            BDSettings = new BDSettings()
            {
                BDName = p.BDName,
                Port = p.BDPort,
                Login = p.BDLogin,
                Server = p.BDServer
            };

            res = ProtectedDataDPAPI.DecryptData(p.BDPassword);
            if (res.Successfully) BDSettings.Password = res.Entity;

            BDAccessorySettings = new BDSettings()
            {
                BDName = p.BDAccessoryName,
                Port = p.BDAccessoryPort,
                Login = p.BDAccessoryLogin,
                Server = p.BDAccessoryServer
            };

            res = ProtectedDataDPAPI.DecryptData(p.BDAccessoryPassword);
            if (res.Successfully) BDAccessorySettings.Password = res.Entity;

            NotificationSettings = new NotificationSettings()
            {
                CanNotificationDataEmpty = p.CanNotificationDataEmpty,
                CanNotificationDataProcessed = p.CanNotificationDataProcessed,
                CanNotificationOnlyError = p.CanNotificationOnlyError,
                CanNotificationProcessCancel = p.CanNotificationProcessCancel,
                CanNotificationSaveData = p.CanNotificationSaveData,
                CanNotificationSaveSettings = p.CanNotificationSaveSettings,
                CanNotificationStatAlreadySave = p.CanNotificationStatAlreadySave,
                CanNotificationExit = p.CanNotificationExit,
                CanSendFileOnMail = p.CanSendFileOnMail,
                RecipientsResultFile = p.RecipientsResultFile,
                MailSender = p.MailSender
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
                ScpriptBackgroundGeo = p.ScpriptBackgroundGeo,
                UseScriptBackGeo = p.UseScriptBackGeo,
                CountNewAddress = p.CountNewAddress,
                UseGetNewAddressBackGeo = p.UseGetNewAddressBackGeo,
                UseGetNewBadAddressBackGeo = p.UseGetNewBadAddressBackGeo
            };

            var listDayWeek = ObjectToStringJson.GetObjectOfstring<List<DayWeekWithTime>>(p.ListDayWeekMode);
            if (listDayWeek == null || !listDayWeek.Any())
            {
                listDayWeek = new List<DayWeekWithTime>();
                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                {
                    listDayWeek.Add(new DayWeekWithTime() { Day = d });
                }
                var s = listDayWeek.First(x => x.Day == 0);
                listDayWeek.Remove(s);
                listDayWeek.Insert(listDayWeek.Count, s);
            }

            GeneralSettings.ListDayWeek = listDayWeek;

            VerificationSettings = new VerificationSettings
            {
                VerificationServer = p.VerificationServer
            };
        }

        /// <summary>
        /// Получение данных о апи-ключах из файла
        /// </summary>
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
                        try
                        {
                            var a = ObjectToStringJson.GetObjectOfstring<IEnumerable<EntityApiKey>>(data.First());
                            if (a != null && a.Any())
                            {
                                ApiKeySettings.CollectionApiKeys = new ObservableCollection<EntityApiKey>(a);

                                foreach (var k in ApiKeySettings.CollectionApiKeys)
                                {
                                    if (k.CollectionDayWeekSettings == null)
                                    {
                                        var listDayWeek = new List<DayWeek>();
                                        foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                                        {
                                            listDayWeek.Add(new DayWeek() { Day = d });
                                        }
                                        var s = listDayWeek.First(x => x.Day == 0);
                                        listDayWeek.Remove(s);
                                        listDayWeek.Insert(listDayWeek.Count, s);
                                        k.CollectionDayWeekSettings = listDayWeek;
                                    }

                                    var l = k.CollectionDayWeekSettings.FirstOrDefault(x => x.Day == DateTime.Now.DayOfWeek && x.Selected)?.MaxCount;
                                    k.CurrentLimit = l != null ? (int)l : 0;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }, _apiKeySettings.File);
        }

        /// <summary>
        /// Получение данных о геокодерах из файла
        /// </summary>
        private void GetCollectionGeocoder()
        {
            _model.ReadFile((er, data) =>
            {
                if (er != null)
                {
                    _geoCodSettings.CollectionGeoCoder = new ObservableCollection<EntityGeoCoder>();
                }
                else
                {
                    if (data != null && data.Any())
                    {
                        try
                        {
                            var a = ObjectToStringJson.GetObjectOfstring<IEnumerable<EntityGeoCoder>>(data.First());
                            if (a != null && a.Any())
                            {
                                _geoCodSettings.CollectionGeoCoder = new ObservableCollection<EntityGeoCoder>(a);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }, _geoCodSettings.File);
        }

        /// <summary>
        /// Метод для получения списка прокси
        /// </summary>
        private void GetProxyList()
        {
            if (!_netSettings.IsListProxy) return;

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
                p.FtpFolderOutput = _ftpSettings.FolderOutput;
                p.FtpServer = _ftpSettings.Server;

                var res = ProtectedDataDPAPI.EncryptData(_ftpSettings.Password);
                if (res.Successfully) p.FtpPassword = res.Entity;
            }

            if (_geoCodSettings != null)
            {
                p.CanGeoCodGetAll = _geoCodSettings.CanGeoCodGetAll;
                p.CanGeoCodGetError = _geoCodSettings.CanGeoCodGetError;
                p.CanGeoCodGetNotGeo = _geoCodSettings.CanGeoCodGetNotGeo;
                p.CanOpenFolderAfter = _geoCodSettings.CanOpenFolderAfter;
                p.CanSaveDataAsFinished = _geoCodSettings.CanSaveDataAsFinished;
                p.CanSaveDataAsTemp = _geoCodSettings.CanSaveDataAsTemp;
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
                p.UseScriptBackGeo = _generalSettings.UseScriptBackGeo;
                p.ScpriptBackgroundGeo = _generalSettings.ScpriptBackgroundGeo;
                p.ColorTheme = _generalSettings.ColorTheme;
                p.CountNewAddress = _generalSettings.CountNewAddress;
                p.UseGetNewAddressBackGeo = _generalSettings.UseGetNewAddressBackGeo;
                p.UseGetNewBadAddressBackGeo = _generalSettings.UseGetNewBadAddressBackGeo;
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
                p.BDServer = _BDSettings.Server;

                var res = ProtectedDataDPAPI.EncryptData(_BDSettings.Password);
                if (res.Successfully) p.BDPassword = res.Entity;
            }

            if (_BDAccessorySettings != null)
            {
                p.BDAccessoryPort = _BDAccessorySettings.Port;
                p.BDAccessoryName = _BDAccessorySettings.BDName;
                p.BDAccessoryLogin = _BDAccessorySettings.Login;
                p.BDAccessoryServer = _BDAccessorySettings.Server;

                var res = ProtectedDataDPAPI.EncryptData(_BDAccessorySettings.Password);
                if (res.Successfully) p.BDAccessoryPassword = res.Entity;
            }

            if (_verificationSettings != null)
            {
                p.VerificationServer = _verificationSettings.VerificationServer;
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
                p.RecipientsResultFile = _notificationSettings.RecipientsResultFile;
                p.CanSendFileOnMail = _notificationSettings.CanSendFileOnMail;
                p.MailSender = _notificationSettings.MailSender;
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
                p.UseScriptBackGeo = _generalSettings.UseScriptBackGeo;
                p.ScpriptBackgroundGeo = _generalSettings.ScpriptBackgroundGeo;
                p.ColorTheme = _generalSettings.ColorTheme;
                p.CountNewAddress = _generalSettings.CountNewAddress;
                p.UseGetNewAddressBackGeo = _generalSettings.UseGetNewAddressBackGeo;
                p.UseGetNewBadAddressBackGeo = _generalSettings.UseGetNewBadAddressBackGeo;
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
                if (e != null)
                {
                    _notifications.Notification(NotificationType.Error, e.Message);
                    _apiKeySettings.CollectionKey = _apiKeySettings.CollectionApiKeys.Select(x => x.ApiKey).ToList();
                }
            }, new string[] { s }, _apiKeySettings.File);

            foreach (var k in ApiKeySettings.CollectionApiKeys)
            {
                var l = k.CollectionDayWeekSettings.FirstOrDefault(x => x.Day == DateTime.Now.DayOfWeek && x.Selected)?.MaxCount;
                k.CurrentLimit = l != null ? (int)l : 0;
            }
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
            _model.OpenFolder(e =>
            {
                _notifications.Notification(NotificationType.Error, e);
            }, str);
        }

        /// <summary>
        /// Метод сохранения геокодеров
        /// </summary>
        private void SaveGeoCoder()
        {
            var s = ObjectToStringJson.GetStringOfObject(_geoCodSettings.CollectionGeoCoder);
            _model.SaveFile((e) =>
            {
                if (e != null) _notifications.Notification(NotificationType.Error, e.Message);
            }, new string[] { s }, _geoCodSettings.File);
        }

        /// <summary>
        /// Установка текущего геокодера
        /// </summary>
        private void SetCurrentGeocoder()
        {
            _geoCodSettings.CurrentGeoCoder = _geoCodSettings.CollectionGeoCoder.First(x => x.IsDefault);
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

        private RelayCommand _commandUpdateCurrentSpentDB;
        public RelayCommand CommandUpdateCurrentSpentDB =>
        _commandUpdateCurrentSpentDB ?? (_commandUpdateCurrentSpentDB = new RelayCommand(
        () =>
        {
            Task.Factory.StartNew(() =>
            {
                if (_apiKeySettings.CurrentKey.StatusSync == StatusSyncType.Sync)
                {
                    _apiKeySettings.CurrentKey.StatusSync = StatusSyncType.SyncProcessed;
                    var r = _limitsModel.GetLastUseLimits(_apiKeySettings.CurrentKey.ApiKey);
                    if (r.Successfully)
                    {
                        _apiKeySettings.CurrentKey.CurrentSpent = r.Entity.Value;
                        _apiKeySettings.CurrentKey.DateCurrentSpent = r.Entity.DateTime;
                    }
                    _apiKeySettings.CurrentKey.StatusSync = StatusSyncType.Sync;
                }
            });
        }));
    }
}