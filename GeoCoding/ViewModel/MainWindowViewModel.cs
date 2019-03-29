using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GeoCoding
{
    /// <summary>
    /// Класс ViewModel для основного окна
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region PrivateConst

        /// <summary>
        /// Сообщение об отмене операции
        /// </summary>
        private const string _errorCancel = "Операция была отменена.";

        /// <summary>
        /// Текст сообщения если адрес пустой при геокодировании
        /// </summary>
        private const string _errorAddressEmpty = "Адрес пуст";

        /// <summary>
        /// Сообщение о завершении обработки объектов
        /// </summary>
        private const string _processedcompleted = "Обработка завершилась. Всего обработано:";

        /// <summary>
        /// Сообщение о успешном применении настроек из файла
        /// </summary>
        private const string _settingsGood = "Настройки применены успешно";

        #endregion PrivateConst

        #region PrivateFields

        /// <summary>
        /// Поле для хранения ссылки на модель
        /// </summary>
        private readonly MainWindowModel _model;

        private INotifications _notifications;

        /// <summary>
        /// Поле для хранения ссылки на модельгеокодирования
        /// </summary>
        private GeoCodingModel _geoCodingModel;

        /// <summary>
        /// Поле для хранения ссылки на модель работы с прокси
        /// </summary>
        private NetProxyModel _netProxyModel;

        /// <summary>
        /// Поле для хранения ссылки на коллекцию с данными
        /// </summary>
        private ObservableCollection<EntityGeoCod> _collectionGeoCod;

        /// <summary>
        /// Поле для хранения ссылки на представления коллекции
        /// </summary>
        private ICollectionView _customerView;

        /// <summary>
        /// Поле для хранения выделенного адреса
        /// </summary>
        private EntityGeoCod _currentGeoCod;

        /// <summary>
        /// Поле для хранения объекта для одиночного геокодирования
        /// </summary>
        private EntityGeoCod _singlGeoCod = new EntityGeoCod();

        /// <summary>
        /// Поле для хранения настроек входного и выходного файла
        /// </summary>
        private FilesSettings _filesSettings;

        /// <summary>
        /// Поле для хранения настроек оповещений
        /// </summary>
        private NotificationSettings _notificationSettings;

        /// <summary>
        /// Поле для хранения ссылки на настройки модуля проверки данных
        /// </summary>
        private VerificationSettings _verificationSettings;

        /// <summary>
        /// Поле для хранения запущена ли процедура декодирования
        /// </summary>
        private bool _isStartGeoCoding = false;

        /// <summary>
        /// Поле для хранения запрошена ли остановка процесса геокодирования
        /// </summary>
        private bool _isRequestedStop = false;

        /// <summary>
        /// Поле для хранения запущено ли получение данных из БД
        /// </summary>
        private bool _isStartGetDataFromBD = false;

        ///// <summary>
        ///// Поле для хранения ссылки на текущий выбранный геосервис
        ///// </summary>
        //private string _currentGeoService;

        /// <summary>
        /// Поле для хранения ссылки на настройки геокодирования
        /// </summary>
        private GeoCodSettings _geoCodSettings;

        /// <summary>
        /// Поле для хранения ссылки на настройки фтп сервера
        /// </summary>
        private FTPSettings _ftpSettings;

        /// <summary>
        /// Поле для хранения ссылки на настройки БД
        /// </summary>
        private BDSettings _bdSettings;

        /// <summary>
        /// Поле для хранения ссылки на основные настройки приложения
        /// </summary>
        private GeneralSettings _generalSettings;

        /// <summary>
        /// Поле для хранения статистики
        /// </summary>
        private StatisticsViewModel _stat;

        /// <summary>
        /// Поле для хранения ссылки на команду получения полного имени файла
        /// </summary>
        private RelayCommand _commandGetFile;

        /// <summary>
        /// Поле для хранения ссылки на команду получения полного имени файла для сохранения
        /// </summary>
        private RelayCommand _commandSetFileOutput;

        /// <summary>
        /// Поле для хранения ссылки на команду сохранения данных
        /// </summary>
        private RelayCommand _commandSaveData;

        /// <summary>
        /// Поле для хранения ссылки на команду получения данных из файла
        /// </summary>
        private RelayCommand _commandGetDataFromFile;

        /// <summary>
        /// Поле для хранения ссылки на команду для геокодирования объекта
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandGetGeoCod;

        /// <summary>
        /// Поле для хранения ссылки на команду открытия папки
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;

        /// <summary>
        /// Поле для хранения ссылки на команду для получения координат для множества объектов
        /// </summary>
        private RelayCommand _commandGetAllGeoCod;

        /// <summary>
        /// Поле для хранения ссылки на команду остановки геокодирования
        /// </summary>
        private RelayCommand _commandStopGeoCoding;

        /// <summary>
        /// Поле для хранения ссылки на команду копирования адреса объекта
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandCopyAddress;

        /// <summary>
        /// Поле для хранения ссылки на команду открытия(геокодирования) адреса в браузере
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandOpenInBrowser;

        /// <summary>
        /// Поле для хранения ссылки на команду перетаскивания файлов на окно программы
        /// </summary>
        private RelayCommand<DragEventArgs> _commandDragDrop;

        /// <summary>
        /// Поле для хранения ссылки на команду драг-дроп файлов в список файлов
        /// </summary>
        private RelayCommand<DragEventArgs> _commandDragDropFileForList;

        /// <summary>
        /// Поле для хранения ссылки на команду сохранения настроек
        /// </summary>
        private RelayCommand _commandSaveSettings;

        /// <summary>
        /// Поле для хранения ссылки на команду сохранение статистики
        /// </summary>
        private RelayCommand _commandSaveStatistics;

        /// <summary>
        /// Поле для хранения ссылки на команду копирования ссылки на запрос
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandCopyRequest;

        /// <summary>
        /// Поле для хранения ссылки на команду копирования глобалАйДи в буфер
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandCopyGlobalId;

        /// <summary>
        /// Поле для хранения ссылки на команду проверки соединения с фтп-сервером
        /// </summary>
        private RelayCommand _commandCheckConnectFtp;

        /// <summary>
        /// Поле для хранения ссылки на команду проверки соединения с базой
        /// </summary>
        private RelayCommand _commandCheckConnect;

        /// <summary>
        /// Поле для хранения ссылки на команду получения данных из базы данных
        /// </summary>
        private RelayCommand _commandGetDataFromBD;

        /// <summary>
        /// Поле для хранения ссылки на команду очистки коллекции
        /// </summary>
        private RelayCommand _commandClearCollection;

        /// <summary>
        /// Поле для хранения команды обработки выбора варианта геокодирования
        /// </summary>
        private RelayCommand<SelectionChangedEventArgs> _commandSelectMainGeo;

        /// <summary>
        /// Поле для хранения команды обработки закрытия программы
        /// </summary>
        private RelayCommand<CancelEventArgs> _commandClosing;

        /// <summary>
        /// Поле для хранения команды для геокодирования группы
        /// </summary>
        private RelayCommand<object> _commandGetGeoCodGroup;

        /// <summary>
        /// Поле для хранения команды сохранения временных данных
        /// </summary>
        private RelayCommand _commandSaveTempData;

        #endregion PrivateFields

        #region PublicPropertys

        /// <summary>
        /// Модель работы с оповещениями
        /// </summary>
        public INotifications Notifications => _notifications;

        /// <summary>
        /// Коллекция с данными
        /// </summary>
        public ObservableCollection<EntityGeoCod> CollectionGeoCod
        {
            get => _collectionGeoCod;
            set
            {
                Set(ref _collectionGeoCod, value);
                CommandGetAllGeoCod.RaiseCanExecuteChanged();
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    _stat.Init(value);
                    //Customers = CollectionViewSource.GetDefaultView(_collectionGeoCod); потому что так будет пустая основная коллекция
                    Customers = new CollectionViewSource { Source = _collectionGeoCod }.View;
                    DispatcherHelper.CheckBeginInvokeOnUI(() => Customers.GroupDescriptions.Add(new PropertyGroupDescription("Error")));
                    _customerView.Filter = CustomerFilter;
                    _ver.SetCollection(value);
                });
            }
        }

        /// <summary>
        /// Настройки входного и выходного файла
        /// </summary>
        public FilesSettings FilesSettings
        {
            get => _filesSettings;
            set => Set(ref _filesSettings, value);
        }

        /// <summary>
        /// Настройки оповещений
        /// </summary>
        public NotificationSettings NotificationSettings
        {
            get => _notificationSettings;
            set => Set(ref _notificationSettings, value);
        }

        /// <summary>
        /// Запущена ли процедура декодирования
        /// </summary>
        public bool IsStartGeoCoding
        {
            get => _isStartGeoCoding;
            set
            {
                Set(ref _isStartGeoCoding, value);
                IsRequestedStop = false;
            }
        }

        /// <summary>
        /// Запрошена ли отмена процесса геокодирования
        /// </summary>
        public bool IsRequestedStop
        {
            get => _isRequestedStop;
            set => Set(ref _isRequestedStop, value);
        }

        /// <summary>
        /// Запущено ли получение данных из БД
        /// </summary>
        public bool IsStartGetDataFromBD
        {
            get => _isStartGetDataFromBD;
            set => Set(ref _isStartGetDataFromBD, value);
        }

        /// <summary>
        /// Представление коллекции
        /// </summary>
        public ICollectionView Customers
        {
            get => _customerView;
            set => Set(ref _customerView, value);
        }

        /// <summary>
        /// Текущий выделенный адрес
        /// </summary>
        public EntityGeoCod CurrentGeoCod
        {
            get => _currentGeoCod;
            set => Set(ref _currentGeoCod, value);
        }

        /// <summary>
        /// Объект для одиночного геокодирования
        /// </summary>
        public EntityGeoCod SingleGeoCod
        {
            get => _singlGeoCod;
            set => Set(ref _singlGeoCod, value);
        }

        /// <summary>
        /// Настройки для работы с ФТП-сервером
        /// </summary>
        public FTPSettings FTPSettings
        {
            get => _ftpSettings;
            set => Set(ref _ftpSettings, value);
        }

        /// <summary>
        /// Настройки геокодирования
        /// </summary>
        public GeoCodSettings GeoCodSettings
        {
            get => _geoCodSettings;
            set => Set(ref _geoCodSettings, value);
        }

        /// <summary>
        /// Настройки БД
        /// </summary>
        public BDSettings BDSettings
        {
            get => _bdSettings;
            set => Set(ref _bdSettings, value);
        }

        /// <summary>
        /// Настройки модуля проверки данных
        /// </summary>
        public VerificationSettings VerificationSettings
        {
            get => _verificationSettings;
            set => Set(ref _verificationSettings, value);
        }

        /// <summary>
        /// основные настройки приложения
        /// </summary>
        public GeneralSettings GeneralSettings
        {
            get => _generalSettings;
            set => Set(ref _generalSettings, value);
        }

        /// <summary>
        /// Статистика выполнения геокодирования
        /// </summary>
        public StatisticsViewModel Stat
        {
            get => _stat;
            set => Set(ref _stat, value);
        }

        #endregion PublicPropertys

        #region PublicCommands

        /// <summary>
        /// Команда для выбора файла (получения полного имени файла с данными)
        /// </summary>
        public RelayCommand CommandGetFile =>
        _commandGetFile ?? (_commandGetFile = new RelayCommand(
                    () =>
                    {
                        string defName = string.Empty;

                        if (_filesSettings.IsFileInputOnFTP)
                        {
                            defName = _ftpSettings.Server + _ftpSettings.FolderOutput;
                        }
                        _model.GetFile((f, er) =>
                        {
                            // Оповещаем если ошибка
                            _notifications.Notification(NotificationType.Error, er);

                            if (er == null)
                            {
                                if (!string.IsNullOrEmpty(f))
                                {
                                    SetFileInput(f);
                                }
                            }
                        }, defName);
                    }, () => !_isStartGeoCoding));

        /// <summary>
        /// Команда для выбора файла для сохранения (получения полного имени файла для сохранения)
        /// </summary>
        public RelayCommand CommandSetFileOutput =>
        _commandSetFileOutput ?? (_commandSetFileOutput = new RelayCommand(
                    () =>
                    {
                        string defName = string.Empty;

                        if (string.IsNullOrEmpty(_filesSettings.FileOutput))
                        {
                            defName = SetDefNameFileOutput();
                        }
                        else
                        {
                            defName = _filesSettings.FileOutput;
                        }

                        _model.SetFileForSave((file, error) =>
                        {
                            _notifications.Notification(NotificationType.Error, error);

                            if (error == null)
                            {
                                if (!string.IsNullOrEmpty(file))
                                {
                                    // Сохраняем полное имя файла в свойстве FileOutput
                                    FilesSettings.FileOutput = file;
                                }
                            }
                        }, defName);
                    }));

        /// <summary>
        /// Команда для сохранения данных в файл
        /// </summary>
        public RelayCommand CommandSaveData =>
        _commandSaveData ?? (_commandSaveData = new RelayCommand(
                    () =>
                    {
                        SaveData();
                        SaveErrors();

                        if (_geoCodSettings.CanSaveStatistics)
                        {
                            SaveStatistics();
                        }
                        if (_geoCodSettings.CanSaveDataAsTemp)
                        {
                            SaveTemp();
                        }
                    }, () => !string.IsNullOrEmpty(_filesSettings.FileOutput) && _collectionGeoCod != null && _collectionGeoCod.Any()));

        /// <summary>
        /// Команда получения данных из файла
        /// </summary>
        public RelayCommand CommandGetDataFromFile =>
        _commandGetDataFromFile ?? (_commandGetDataFromFile = new RelayCommand(
                    () =>
                    {
                        // Получаем данные из файла
                        GetDataFromFile();
                    }, () => !string.IsNullOrEmpty(_filesSettings.FileInput) && !_isStartGeoCoding));

        /// <summary>
        /// Команда для геокодирования объекта
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandGetGeoCod =>
        _commandGetGeoCod ?? (_commandGetGeoCod = new RelayCommand<EntityGeoCod>(
                    geocod =>
                    {
                        if (!string.IsNullOrEmpty(geocod.Address))
                        {
                            _geoCodingModel.GetGeoCod(er =>
                            {
                                _notifications.Notification(NotificationType.Error, er);

                                if (er == null)
                                {
                                    _customerView?.Refresh();
                                    _stat.UpdateStatisticsCollection();
                                }
                            }, geocod);
                        }
                        else
                        {
                            _notifications.Notification(NotificationType.Error, _errorAddressEmpty);
                        }
                    }));

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
        /// Команда для получения координат для множества объектов
        /// </summary>
        public RelayCommand CommandGetAllGeoCod =>
        _commandGetAllGeoCod ?? (_commandGetAllGeoCod = new RelayCommand(
                    () =>
                    {
                        if(_collectionFiles!=null && _collectionFiles.Any())
                        {
                            GetAllGeooForFiles();
                        }
                        else
                        {
                            GetAllGeoCod(_collectionGeoCod);
                        }
                    }, () => (_collectionGeoCod != null && _collectionGeoCod.Any()) || (_collectionFiles!=null && _collectionFiles.Any())));

        /// <summary>
        /// Команда остановки геокодирования
        /// </summary>
        public RelayCommand CommandStopGeoCoding =>
        _commandStopGeoCoding ?? (_commandStopGeoCoding = new RelayCommand(
                    () =>
                    {
                        // Останавливаем процесс
                        _geoCodingModel.StopGet();
                        IsRequestedStop = true;
                    }));

        /// <summary>
        /// Команда копирования адреса в буфер
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandCopyAddress =>
        _commandCopyAddress ?? (_commandCopyAddress = new RelayCommand<EntityGeoCod>(
                obj =>
                {
                    try
                    {
                        Clipboard.SetText(obj.Address, TextDataFormat.UnicodeText);
                    }
                    catch (Exception ex)
                    {
                        _notifications.Notification(NotificationType.Error, ex.Message);
                    }
                }));

        /// <summary>
        /// Команда копирования глобалАйДи в буфер
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandCopyGlpobalId =>
        _commandCopyGlobalId ?? (_commandCopyGlobalId = new RelayCommand<EntityGeoCod>(
                 obj =>
                 {
                     try
                     {
                         Clipboard.SetText(obj.GlobalID.ToString(), TextDataFormat.UnicodeText);
                     }
                     catch (Exception ex)
                     {
                         _notifications.Notification(NotificationType.Error, ex.Message);
                     }
                 }));

        /// <summary>
        /// Команда открыть адрес в браузере
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandOpenInBrowser =>
        _commandOpenInBrowser ?? (_commandOpenInBrowser = new RelayCommand<EntityGeoCod>(
                  obj =>
                  {
                      try
                      {
                          System.Diagnostics.Process.Start(_geoCodingModel.GetUrlRequest(obj.Address));
                      }
                      catch (Exception ex)
                      {
                          _notifications.Notification(NotificationType.Error, ex.Message);
                      }
                  }));

        /// <summary>
        /// Команда обработки перетаскивания файлов на окно программы
        /// </summary>
        public RelayCommand<DragEventArgs> CommandDragDrop =>
        _commandDragDrop ?? (_commandDragDrop = new RelayCommand<DragEventArgs>(
                    obj =>
                    {
                        if (obj.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
                        {
                            var a = (string[])obj.Data.GetData(DataFormats.FileDrop, true);
                            if (a.Length == 1)
                            {
                                if (a[0].Contains(".ini"))
                                {
                                    GetSettingsFromFile(a[0], _ftpSettings, _bdSettings);
                                }
                                else
                                {
                                    SetFileInput(a[0]);
                                }
                            }
                            else if (a.Length > 1)
                            {
                                SetCollectionFiles(a);
                            }
                        }
                    }, obj => !_isStartGeoCoding));

        /// <summary>
        /// Команда обработки перетаскивания файлов в список файлов
        /// </summary>
        public RelayCommand<DragEventArgs> CommandDragDropFileForList =>
        _commandDragDropFileForList ?? (_commandDragDropFileForList = new RelayCommand<DragEventArgs>(
                    obj =>
                    {
                        if (obj.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
                        {
                            var a = (string[])obj.Data.GetData(DataFormats.FileDrop, true);
                            SetCollectionFiles(a);
                        }
                    }));

        /// <summary>
        /// Команда для сохранения настроек
        /// </summary>
        public RelayCommand CommandSaveSettings =>
        _commandSaveSettings ?? (_commandSaveSettings = new RelayCommand(
                    () =>
                    {
                        _model.SaveSettings(e =>
                        {
                            _notifications.Notification(NotificationType.SettingsSave, e, true);
                        }, _filesSettings, _ftpSettings, _geoCodSettings, _bdSettings, _notificationSettings, _netSettings, _verificationSettings, _generalSettings);
                    }));

        /// <summary>
        /// Команда для сохранения статистики
        /// </summary>
        public RelayCommand CommandSaveStatistics =>
        _commandSaveStatistics ?? (_commandSaveStatistics = new RelayCommand(
                    () =>
                    {
                        SaveStatistics();
                    }, () => _stat != null && _stat.Statistics != null));

        /// <summary>
        /// Команда для копирования ссылки на запрос
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandCopyRequest =>
        _commandCopyRequest ?? (_commandCopyRequest = new RelayCommand<EntityGeoCod>(
                    obj =>
                    {
                        try
                        {
                            Clipboard.SetText(_geoCodingModel.GetUrlRequest(obj.Address), TextDataFormat.UnicodeText);
                        }
                        catch (Exception ex)
                        {
                            _notifications.Notification(NotificationType.Error, ex);
                        }
                    }));

        /// <summary>
        /// Команда для проверки соединения с базой
        /// </summary>
        public RelayCommand CommandCheckConnect =>
        _commandCheckConnect ?? (_commandCheckConnect = new RelayCommand(
                    () =>
                    {
                        _bdSettings.StatusConnect = StatusType.Processed;
                        _bdSettings.Error = string.Empty;
                        _model.ConnectBDAsync(e =>
                        {
                            if (e != null)
                            {
                                _notifications.Notification(NotificationType.Error, e);
                                _bdSettings.StatusConnect = StatusType.Error;
                                _bdSettings.Error = e.Message;
                            }
                            else
                            {
                                _bdSettings.StatusConnect = StatusType.OK;
                                _bdSettings.Error = string.Empty;
                            }
                        }, _bdSettings);
                    }, () => !string.IsNullOrEmpty(_bdSettings.Server) || !string.IsNullOrEmpty(_bdSettings.BDName) || _bdSettings.StatusConnect == StatusType.Processed));

        /// <summary>
        /// Команда для проверки соединения с фтп-сервером
        /// </summary>
        public RelayCommand CommandCheckConnectFtp =>
        _commandCheckConnectFtp ?? (_commandCheckConnectFtp = new RelayCommand(
                    () =>
                    {
                        _ftpSettings.StatusConnect = StatusType.Processed;
                        _ftpSettings.Error = string.Empty;
                        _model.ConnectFTPAsync(e =>
                        {
                            if (e != null)
                            {
                                _notifications.Notification(NotificationType.Error, e);
                                _ftpSettings.StatusConnect = StatusType.Error;
                                _ftpSettings.Error = e.Message;
                            }
                            else
                            {
                                FTPSettings.StatusConnect = StatusType.OK;
                                _ftpSettings.Error = string.Empty;
                            }
                        }, _ftpSettings);
                    }, () => !string.IsNullOrEmpty(_ftpSettings.Server) || _ftpSettings.StatusConnect == StatusType.Processed));

        /// <summary>
        /// Команда для получения данных из базы данных
        /// </summary>
        public RelayCommand CommandGetDataFromBD =>
        _commandGetDataFromBD ?? (_commandGetDataFromBD = new RelayCommand(
                    () =>
                    {
                        IsStartGetDataFromBD = true;
                        _model.GetDataFromBDAsync((data, error) =>
                        {
                            if (error == null)
                            {
                                _filesSettings.FileOutput = SetDefNameFileOutput();
                            }

                            CreateCollection(data, error);
                            IsStartGetDataFromBD = false;

                        }, _bdSettings, _bdSettings.SQLQuery);
                    }, () => !string.IsNullOrEmpty(_bdSettings.SQLQuery) && !_isStartGetDataFromBD && !_isStartGeoCoding));

        /// <summary>
        /// Команда очистки коллекции
        /// </summary>
        public RelayCommand CommandClearCollection =>
        _commandClearCollection ?? (_commandClearCollection = new RelayCommand(() =>
                    {
                        if (_collectionGeoCod != null && _collectionGeoCod.Any())
                        {
                            _collectionGeoCod.Clear();
                        }
                    }, () => _collectionGeoCod != null && _collectionGeoCod.Any() && !_isStartGeoCoding));

        /// <summary>
        /// Команда обработки выбора варианта геокодирования
        /// </summary>
        public RelayCommand<SelectionChangedEventArgs> CommandSelectMainGeo =>
        _commandSelectMainGeo ?? (_commandSelectMainGeo = new RelayCommand<SelectionChangedEventArgs>(
                    obj =>
                    {
                        if (obj != null && obj.AddedItems != null && obj.AddedItems.Count > 0)
                        {
                            if (obj.AddedItems[0] is GeoCod item)
                            {
                                _currentGeoCod.MainGeoCod = item;
                                _currentGeoCod.Error = string.Empty;
                                _currentGeoCod.Status = StatusType.OK;
                                _stat.UpdateStatisticsCollection();
                            }
                        }
                    }));

        /// <summary>
        /// Команда для обработки закрытия программы
        /// </summary>
        public RelayCommand<CancelEventArgs> CommandClosing =>
        _commandClosing ?? (_commandClosing = new RelayCommand<CancelEventArgs>(
                    obj =>
                    {
                        if (_isStartGeoCoding)
                        {
                            if (obj != null)
                            {
                                obj.Cancel = true;
                                _notifications.Notification(NotificationType.Close, "Идет процесс геокодирования. Закрытие невозможно!");
                            }
                        }
                        else
                        {
                            if (obj != null)
                            {
                                obj.Cancel = !_notifications.NotificationWithConfirmation(NotificationType.Close, "Вы уверены?");
                            }
                        }
                    }));

        /// <summary>
        /// Команда для запуска геокодирования для группы
        /// </summary>
        public RelayCommand<object> CommandGetGeoCodGroup =>
            _commandGetGeoCodGroup ?? (_commandGetGeoCodGroup = new RelayCommand<object>(
                obj =>
                {
                    if (obj is CollectionViewGroup a)
                    {
                        var b = a.Items;
                        GetAllGeoCod(b.Select(x => (EntityGeoCod)x), false);
                    }
                }, obj => !_isStartGeoCoding));

        /// <summary>
        /// Команда для сохранения временных данных
        /// </summary>
        public RelayCommand CommandSaveTempData =>
        _commandSaveTempData ?? (_commandSaveTempData = new RelayCommand(
                    () =>
                    {
                        SaveTemp();
                    }, () => _collectionGeoCod != null && _collectionGeoCod.Any()));

        #endregion PublicCommands

        #region PrivateMethod

        /// <summary>
        /// Метод для получения данных из файла
        /// </summary>
        private void GetDataFromFile()
        {
            _model.GetDataFromFile((list, error) =>
            {
                CreateCollection(list, error);
            }, _filesSettings.FileInput);
        }

        /// <summary>
        /// Фильтрация для представления коллекции
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns>Возвращает истину. если объект по критериям проходит</returns>
        private bool CustomerFilter(object item)
        {
            EntityGeoCod customer = item as EntityGeoCod;
            return customer.Status == StatusType.Error;
        }

        /// <summary>
        /// Сохраняем данные
        /// </summary>
        private void SaveData()
        {
            int parSize = _filesSettings.CanBreakFileOutput ? _filesSettings.MaxSizePart : 0;

            _model.SaveData(error =>
            {
                // Оповещаем о записи
                _notifications.Notification(NotificationType.SaveData, error, true);
                if (error == null)
                {
                    if (_geoCodSettings.CanOpenFolderAfter)
                    {
                        OpenFolder(_filesSettings.FileOutput);
                    }
                }
            }, _collectionGeoCod, _filesSettings.FileOutput, parSize, _filesSettings.CanCopyFileOutputToFtp, _ftpSettings);
        }

        /// <summary>
        /// Открыть папку
        /// </summary>
        /// <param name="str">Путь к файлу или папке</param>
        private void OpenFolder(string str)
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

        /// <summary>
        /// Метод установки файла с данными
        /// </summary>
        /// <param name="nameFile"></param>
        private void SetFileInput(string nameFile)
        {
            // Сохраняем полное имя файла в свойство FilesInput
            FilesSettings.FileInput = nameFile;
            // Если данные получать сразу, то получаем
            if (_filesSettings.CanGetDataOnce && !string.IsNullOrEmpty(_filesSettings.FileInput))
            {
                // Получаем данные из файла
                GetDataFromFile();
            }

            FilesSettings.FileOutput = SetDefNameFileOutput();

            if (_collectionGeoCod != null && _collectionGeoCod.Any() && _geoCodSettings.CanGeoCodAfterGetFile)
            {
                GetAllGeoCod(_collectionGeoCod);
            }
        }

        /// <summary>
        /// Метод задания имени файла для сохранения по умолчанию
        /// </summary>
        /// <returns>Возвращает полное имя файла</returns>
        private string SetDefNameFileOutput()
        {
            string defName = string.Empty;

            if (_collectionGeoCod != null && _collectionGeoCod.Any())
            {
                defName = $"{NameFile()}_UpLoad_{_collectionGeoCod.Count}.csv";
            }
            else
            {
                defName = $"{NameFile()}_UpLoad.csv";
            }

            return $"{_filesSettings.FolderOutput}\\{defName}";
        }

        /// <summary>
        /// Метод для получения имени файла с ошибками
        /// </summary>
        /// <returns>Имя файла</returns>
        private string SetDefNameFileErrors()
        {
            string defName = string.Empty;

            if (_collectionGeoCod != null && _collectionGeoCod.Any())
            {
                int countError = _collectionGeoCod.Count(x => x.Status == StatusType.Error);
                defName = $"{_filesSettings.FolderErrors}\\{NameFile()}_Errors_{countError}.csv";
            }

            return defName;
        }

        /// <summary>
        /// Метод для получения имени файла для временных данных
        /// </summary>
        /// <returns>Имя файла</returns>
        private string SetDefNameFileTemp()
        {
            string defName = string.Empty;

            if (_collectionGeoCod != null && _collectionGeoCod.Any())
            {
                defName = $"{_filesSettings.FolderTemp}\\{NameFile()}_Temp_{_collectionGeoCod.Count}.csv";
            }

            return defName;
        }

        private string NameFile() => $"{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}_{System.IO.Path.GetFileNameWithoutExtension(_filesSettings.FileInput)}";

        /// <summary>
        /// Метод для получения имени файла статистики по умолчанию
        /// </summary>
        /// <returns>Имя файла</returns>
        private string SetDefNameFileStatistics()
        {
            return $"{_filesSettings.FolderStatistics}\\{DateTime.Now.ToString("yyyy_MM_dd")}_Statistics.csv";
        }

        /// <summary>
        /// Метод для сохранения файла с ошибками
        /// </summary>
        private void SaveErrors()
        {
            if (_collectionGeoCod != null && _collectionGeoCod.Any(x => x.Status == StatusType.Error))
            {
                var nameFile = SetDefNameFileErrors();
                _filesSettings.FileError = nameFile;
                _model.SaveError(error =>
                {
                    _notifications.Notification(NotificationType.Error, error);
                }, _collectionGeoCod.Where(x => x.Status == StatusType.Error), nameFile);
            }
        }

        /// <summary>
        /// Метод для сохранения временных файлов
        /// </summary>
        private void SaveTemp()
        {
            if (_collectionGeoCod != null && _collectionGeoCod.Any())
            {
                var nameFile = SetDefNameFileTemp();
                _model.SaveTemp(error =>
                {
                    _notifications.Notification(NotificationType.Error, error);
                }, _collectionGeoCod, nameFile);
            }
        }

        /// <summary>
        /// Метод для сохранения статистики
        /// </summary>
        private void SaveStatistics()
        {
            if (_stat.IsSave)
            {
                _notifications.Notification(NotificationType.StatAlreadySave);
                return;
            }

            var nameFile = SetDefNameFileStatistics();
            _model.SaveStatistics(e =>
            {
                _notifications.Notification(NotificationType.Error, e);
                if (e == null)
                {
                    _stat.IsSave = true;
                }
            }, _stat.Statistics, _filesSettings, nameFile);
        }

        /// <summary>
        /// Метод для получения координат для всей коллекции
        /// </summary>
        private void GetAllGeoCod(IEnumerable<EntityGeoCod> collectionItem, bool canFilter = true)
        {
            IEnumerable<EntityGeoCod> data = null;

            if (canFilter)
            {
                data = GetCollectionWithFilter(collectionItem);
            }
            else
            {
                data = collectionItem;
            }

            data = data.Where(x => !string.IsNullOrEmpty(x.Address));

            int countData = data.Count();
            if (countData > 0)
            {
                // Отображаем индикацию работы процесса
                IsStartGeoCoding = true;
                _stat.Start(_geoCodSettings.GeoService);

                _geoCodingModel.GetAllGeoCod(e =>
               {
                   IsStartGeoCoding = false;
                   _stat.Stop();
                   _customerView?.Refresh();

                   if (e == null)
                   {
                       // Оповещаем о завершении получении координат
                       _notifications.Notification(NotificationType.DataProcessed, $"{_processedcompleted} {countData}");
                   }
                   else if (e.Message == _errorCancel)
                   {
                       // Оповещаем если сами отменили
                       _notifications.Notification(NotificationType.Cancel);
                   }
                   else
                   {
                       // Оповещаем если были ошибки
                       _notifications.Notification(NotificationType.Error, e.Message);
                   }

                   if (_geoCodSettings.CanSaveDataAsFinished && !string.IsNullOrEmpty(_filesSettings.FileOutput))
                   {
                       SaveData();
                       SaveErrors();
                   }

                   if (_geoCodSettings.CanSaveDataAsTemp)
                   {
                       SaveTemp();
                   }
                   if (_geoCodSettings.CanSaveStatistics)
                   {
                       SaveStatistics();
                   }

                   _ver.SetCollection(_collectionGeoCod);
               }, data);
            }
            else
            {
                _notifications.Notification(NotificationType.DataEmpty);
            }
        }

        /// <summary>
        /// Метод фильтрации коллекции
        /// </summary>
        /// <param name="collectionItem">Коллекция объектов для фильтрации</param>
        /// <returns>Возвращаем отфильтрованную коллекцию</returns>
        private IEnumerable<EntityGeoCod> GetCollectionWithFilter(IEnumerable<EntityGeoCod> collectionItem)
        {
            IEnumerable<EntityGeoCod> data = null;

            if (_geoCodSettings.CanGeoCodGetAll)
            {
                data = collectionItem;
            }
            else if (_geoCodSettings.CanGeoCodGetError)
            {
                data = collectionItem.Where(x => x.Status == StatusType.Error);
            }
            else if (_geoCodSettings.CanGeoCodGetNotGeo)
            {
                data = collectionItem.Where(x => x.Status == StatusType.NotProcessed);
            }

            return data;
        }

        /// <summary>
        /// Метод для настройки программ из файла настроек
        /// </summary>
        /// <param name="file">Имя файла с настройками</param>
        /// <param name="ftp">Ссылка на настройки фтп</param>
        /// <param name="bd">Ссылка на настройки БД</param>
        private void GetSettingsFromFile(string file, FTPSettings ftp, BDSettings bd)
        {
            _model.GetSettingsFromFile(e =>
            {
                _notifications.Notification(NotificationType.DataProcessed, _settingsGood, e);
            }, file, ftp, bd);
        }

        /// <summary>
        /// Метод для создание коллекции из листа данных из файла или БД
        /// </summary>
        /// <param name="data">Лист данных</param>
        /// <param name="error">Ошибки</param>
        private void CreateCollection(IEnumerable<EntityGeoCod> data, Exception error)
        {
            _notifications.Notification(NotificationType.Error, error);

            if (error == null)
            {
                if (data.Any())
                {
                    // Создаем коллекцию с данными
                    CollectionGeoCod = new ObservableCollection<EntityGeoCod>(data);
                }
                else
                {
                    _notifications.Notification(NotificationType.DataEmpty);
                }
            }
        }

        #endregion PrivateMethod

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
        /// Поле для хранения ссылки на сетевые настройки
        /// </summary>
        private NetSettings _netSettings;
        /// <summary>
        /// Сетевые настройки
        /// </summary>
        public NetSettings NetSettings
        {
            get => _netSettings;
            set => Set(ref _netSettings, value);
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
                    _netSettings.CollectionListProxy = new ObservableCollection<ProxyEntity>(d);
                }
                else
                {
                    _notifications.Notification("ProxyList", e.Message);
                }
            });
        }

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
                        _netSettings.Status = StatusType.Processed;
                        await _netProxyModel.TestListProxyAsync(_netSettings.CollectionListProxy);
                        _netSettings.Status = StatusType.OK;
                    }));

        private VerificationViewModel _ver;
        /// <summary>
        /// 
        /// </summary>
        public VerificationViewModel Ver
        {
            get => _ver;
            set => Set(ref _ver, value);
        }

        private int _tabIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        public int TabIndex
        {
            get => _tabIndex;
            set
            {
                Set(ref _tabIndex, value);
                if (value == 2)
                {
                    _ver?.Update();
                }
                else if (value == 1)
                {
                    _customerView?.Refresh();
                }
            }
        }

        private ObservableCollection<EntityFile> _collectionFiles;
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<EntityFile> CollectionFiles
        {
            get => _collectionFiles;
            set => Set(ref _collectionFiles, value);
        }

        private RelayCommand _commandGetFiles;
        public RelayCommand CommandGetFiles =>
        _commandGetFiles ?? (_commandGetFiles = new RelayCommand(
                    () =>
                    {
                        _model.GetFiles((d, e) =>
                        {
                            // Оповещаем если ошибка
                            _notifications.Notification(NotificationType.Error, e);

                            if (e == null)
                            {
                                SetCollectionFiles(d);
                            }
                        });
                    }, () => !_isStartGeoCoding));

        private RelayCommand _commandClearCollectionFiles;
        public RelayCommand CommandClearCollectionFiles =>
        _commandClearCollectionFiles ?? (_commandClearCollectionFiles = new RelayCommand(
                    () =>
                    {
                        CollectionFiles?.Clear();
                    }, () => !_isStartGeoCoding && _collectionFiles != null && _collectionFiles.Any()));

        private RelayCommand<EntityFile> _commandRemoveFilesFromCollection;
        public RelayCommand<EntityFile> CommandRemoveFilesFromCollection =>
        _commandRemoveFilesFromCollection ?? (_commandRemoveFilesFromCollection = new RelayCommand<EntityFile>(
                    obj =>
                    {
                        _collectionFiles.Remove(obj);
                    }, !_isStartGeoCoding));

        private void SetCollectionFiles(IEnumerable<string> d)
        {
            if (d != null && d.Any())
            {
                if (_collectionFiles == null || !_collectionFiles.Any())
                {
                    CollectionFiles = new ObservableCollection<EntityFile>(d.Select(x =>
                    {
                        return new EntityFile() { NameFile = x };
                    }));
                }
                else
                {
                    foreach (var item in d)
                    {
                        _collectionFiles.Add(new EntityFile() { NameFile = item });
                    }
                }

                GetDataAboutFiles();
            }
        }

        private void GetDataAboutFiles()
        {
            IsStartGetDataAboutFiles = true;

            _model.GetDataAboutFiles(e =>
            {
                if (e == null)
                {
                    IsStartGetDataAboutFiles = false;
                }
            }, _collectionFiles);

        }

        private bool _isStartGetDataAboutFiles = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsStartGetDataAboutFiles
        {
            get => _isStartGetDataAboutFiles;
            set => Set(ref _isStartGetDataAboutFiles, value);
        }

        private RelayCommand _commandGetGeoForListFile;
        public RelayCommand CommandGetGeoForListFile =>
        _commandGetGeoForListFile ?? (_commandGetGeoForListFile = new RelayCommand(
                    () =>
                    {
                        GetAllGeooForFiles();
                    }));

        private async void GetAllGeooForFiles()
        {
            IsStartGeoCoding = true;

            foreach (var item in _collectionFiles)
            {
                if (item.FileType == FileType.Other)
                {
                    continue;
                }

                item.Status = StatusType.Processed;

                FilesSettings.FileInput = item.NameFile;

                _model.GetDataFromFile((list, e) =>
                {
                    CreateCollection(list, e);
                }, item.NameFile);

                FilesSettings.FileOutput = SetDefNameFileOutput();

                IEnumerable<EntityGeoCod> data = null;

                data = GetCollectionWithFilter(_collectionGeoCod).Where(x => !string.IsNullOrEmpty(x.Address));

                int countData = data.Count();

                _stat.Start(_geoCodSettings.GeoService);

                var error = await _geoCodingModel.GetAllGeoCod(data);

                if (_geoCodSettings.CanSaveDataAsFinished && !string.IsNullOrEmpty(_filesSettings.FileOutput))
                {
                    SaveData();
                    SaveErrors();
                }
                if (_geoCodSettings.CanSaveDataAsTemp)
                {
                    SaveTemp();
                }
                if (_geoCodSettings.CanSaveStatistics)
                {
                    SaveStatistics();
                }

                item.Collection = _collectionGeoCod.ToList();
                _stat.Stop();

                if (error == null)
                {
                    item.Status = StatusType.OK;
                }
                else if (error.Message == _errorCancel)
                {
                    item.Status = StatusType.Error;
                    item.Error = error.Message;
                    _notifications.Notification(NotificationType.Cancel);
                    break;
                }
                else
                {
                    item.Status = StatusType.Error;
                    item.Error = error.Message;
                }
            }

            _notifications.Notification(NotificationType.Ok, "Обработка файлов завершена");
            IsStartGeoCoding = false;
        }

        private RelayCommand _myCommand;
        public RelayCommand MyCommand =>
        _myCommand ?? (_myCommand = new RelayCommand(
                    () =>
                    {
                        CheckAllGeoFromFiles();
                    }));

        private async void CheckAllGeoFromFiles()
        {
            foreach(var item in _collectionFiles)
            {
                if (item.FileType == FileType.Other)
                {
                    continue;
                }

                item.Status = StatusType.Processed;
                FilesSettings.FileInput = item.NameFile;

                if(item.FileType == FileType.Temp)
                {
                    _model.GetDataFromFile((list, e) =>
                    {
                        CreateCollection(list, e);
                    }, item.NameFile);
                }
    
                FilesSettings.FileOutput = SetDefNameFileOutput();



                var error = await _ver.CheckAll();
                if(error==null)
                {
                    item.Status = StatusType.OK;
                }
                else
                {
                    item.Error = error.Message;
                    item.Status = StatusType.Error;
                }

            }
            
        }

        private RelayCommand<EntityFile> _commandOpenFile;
        public RelayCommand<EntityFile> CommandOpenFile =>
        _commandOpenFile ?? (_commandOpenFile = new RelayCommand<EntityFile>(
                    obj =>
                    {
                        FilesSettings.FileInput = obj.NameFile;

                        if (obj.Collection != null && obj.Collection.Any())
                        {
                            CreateCollection(obj.Collection, null);
                        }
                        else
                        {
                            _model.GetDataFromFile((list, e) =>
                            {
                                CreateCollection(list, e);
                            }, obj.NameFile);
                        }

                        FilesSettings.FileOutput = SetDefNameFileOutput();
                    }, obj => obj != null && obj.FileType != FileType.Other));

        //private RelayCommand<EntityFile> _commandGetAllGeoCodFile;
        //public RelayCommand<EntityFile> CommandGetAllGeoCodFile =>
        //_commandGetAllGeoCodFile ?? (_commandGetAllGeoCodFile = new RelayCommand<EntityFile>(
        //            obj =>
        //            {
        //                FilesSettings.FileInput = obj.NameFile;

        //                if (obj.Collection != null && obj.Collection.Any())
        //                {
        //                    CreateCollection(obj.Collection, null);
        //                }
        //                else
        //                {
        //                    _model.GetDataFromFile((list, e) =>
        //                    {
        //                        CreateCollection(list, e);
        //                    }, obj.NameFile);
        //                }

        //                FilesSettings.FileOutput = SetDefNameFileOutput();

        //                GetAllGeoCod(_collectionGeoCod);

        //            }));

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
            Stat = new StatisticsViewModel();

            _netProxyModel = new NetProxyModel();

            _model.GetSettings((e, f, g, ftp, bds, ns, nset, vset, gset) =>
            {
                if (e == null)
                {
                    FilesSettings = f;
                    GeoCodSettings = g;
                    FTPSettings = ftp;
                    BDSettings = bds;
                    GeneralSettings = gset;
                    NotificationSettings = ns;
                    NetSettings = nset;
                    VerificationSettings = vset;

                    _notifications = new NotificationsModel(ns);
                    _geoCodingModel = new GeoCodingModel(_netSettings, _geoCodSettings);
                    Ver = new VerificationViewModel(_verificationSettings);
                    if (_netSettings.IsListProxy)
                    {
                        GetProxyList();
                    }
                }
                else
                {
                    _notifications = new NotificationsModel(new NotificationSettings());
                    _notifications.Notification(NotificationType.Error, e);
                }
            });

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, data =>
            {
                if ((data.PropertyName == "IsNotProxy" || data.PropertyName == "IsSystemProxy" || data.PropertyName == "IsManualProxy") && data.NewValue)
                {
                    _geoCodSettings.IsMultipleRequests = true;
                }
                if(data.PropertyName == "IsStartCompare" && !data.NewValue)
                {
                    CommandSaveData.Execute(true);
                }
            });
        }
    }
}