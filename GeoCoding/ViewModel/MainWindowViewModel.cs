// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GeoCoding.Model;
using GeoCoding.Model.Data;
using System;
using System.Collections;
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
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
            _modelBd = new BdModel();

            Notifications = new NotificationsModel();
            AppSettings = new AppSettings(_notifications);

            //_polygon = new PolygonViewModel(_notifications);
            _geoCodingModel = new GeoCodingModel(_appSettings.NetSettings, _appSettings.GeoCodSettings, _appSettings.GetLimitsModel());
            SetGeoService();

            Stat = new StatisticsViewModel();
            Ver = new VerificationViewModel(_appSettings.VerificationSettings);

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, data =>
            {
                if (data.PropertyName == "IsStartCompare" && !data.NewValue)
                {
                    CommandSaveData.Execute(true);
                }
                //if (data.PropertyName == "CanUsePolygon" && data.NewValue)
                //{
                //    _polygon.GetAddress();
                //}
            });

            Messenger.Default.Register<PropertyChangedMessage<EntityGeoCoder>>(this, data =>
            {
                if (data.PropertyName == "CurrentGeoCoder")
                {
                    SetGeoService();
                }
            });
        }

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
        /// Настройки приложения
        /// </summary>
        private AppSettings _appSettings;

        /// <summary>
        /// Поле для хранения ссылки на модель
        /// </summary>
        private readonly MainWindowModel _model;

        /// <summary>
        /// Поле для хранения ссылки на модель работы с базой
        /// </summary>
        private readonly BdModel _modelBd;

        private INotifications _notifications;

        /// <summary>
        /// Поле для хранения ссылки на модельгеокодирования
        /// </summary>
        private readonly GeoCodingModel _geoCodingModel;

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

        /// <summary>
        /// Поле для хранения статистики
        /// </summary>
        private StatisticsViewModel _stat;

        /// <summary>
        /// Поле для хранения ссылки на представление работы с полигонами
        /// </summary>
        private PolygonViewModel _polygon;

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
        //private RelayCommand<string> _commandOpenFolder;

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
        //private RelayCommand<DragEventArgs> _commandDragDropFileForList;

        /// <summary>
        /// Поле для хранения ссылки на команду сохранение статистики
        /// </summary>
        private RelayCommand _commandSaveStatistics;

        /// <summary>
        /// Поле для хранения ссылки на команду открытия папки
        /// </summary>
        private RelayCommand<string> _commandOpenFolder;

        /// <summary>
        /// Поле для хранения ссылки на команду копирования ссылки на запрос
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandCopyRequest;

        /// <summary>
        /// Поле для хранения ссылки на команду копирования глобалАйДи в буфер
        /// </summary>
        private RelayCommand<EntityGeoCod> _commandCopyGlobalId;

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
        /// Настройки приложения
        /// </summary>
        public AppSettings AppSettings
        {
            get => _appSettings;
            set => Set(ref _appSettings, value);
        }

        /// <summary>
        /// Модель работы с оповещениями
        /// </summary>
        public INotifications Notifications
        {
            get => _notifications;
            set => Set(ref _notifications, value);
        }

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
                    //_ver.SetCollection(value);
                });
            }
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
        /// Статистика выполнения геокодирования
        /// </summary>
        public StatisticsViewModel Stat
        {
            get => _stat;
            set => Set(ref _stat, value);
        }

        public PolygonViewModel Polygon
        {
            get => _polygon;
            set => Set(ref _polygon, value);
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

                        if (_appSettings.FilesSettings.IsFileInputOnFTP)
                        {
                            defName = _appSettings.FTPSettings.Server + _appSettings.FTPSettings.FolderOutput;
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

                        if (string.IsNullOrEmpty(_appSettings.FilesSettings.FileOutput))
                        {
                            defName = SetDefNameFileOutput();
                        }
                        else
                        {
                            defName = _appSettings.FilesSettings.FileOutput;
                        }

                        _model.SetFileForSave((file, error) =>
                        {
                            _notifications.Notification(NotificationType.Error, error);

                            if (error == null)
                            {
                                if (!string.IsNullOrEmpty(file))
                                {
                                    // Сохраняем полное имя файла в свойстве FileOutput
                                    _appSettings.FilesSettings.FileOutput = file;
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

                        if (_appSettings.GeoCodSettings.CanSaveStatistics)
                        {
                            SaveStatistics();
                        }
                        if (_appSettings.GeoCodSettings.CanSaveDataAsTemp)
                        {
                            SaveTemp();
                        }
                    }, () => !string.IsNullOrEmpty(_appSettings.FilesSettings.FileOutput) && _collectionGeoCod != null && _collectionGeoCod.Any()));

        /// <summary>
        /// Команда получения данных из файла
        /// </summary>
        public RelayCommand CommandGetDataFromFile =>
        _commandGetDataFromFile ?? (_commandGetDataFromFile = new RelayCommand(
                    () =>
                    {
                        // Получаем данные из файла
                        GetDataFromFile();
                    }, () => !string.IsNullOrEmpty(_appSettings.FilesSettings.FileInput) && !_isStartGeoCoding));

        /// <summary>
        /// Команда для геокодирования объекта
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandGetGeoCod =>
        _commandGetGeoCod ?? (_commandGetGeoCod = new RelayCommand<EntityGeoCod>(
                    geocod =>
                    {
                        if (_collectionSelectionItem != null && _collectionSelectionItem.Count > 1)
                        {
                            foreach (var item in _collectionSelectionItem)
                            {
                                if (!string.IsNullOrEmpty(item.Address))
                                {
                                    _geoCodingModel.GetGeoCod(er =>
                                    {
                                        _notifications.Notification(NotificationType.Error, er);

                                        if (er == null)
                                        {
                                            _customerView?.Refresh();
                                            _stat.UpdateStatisticsCollection();
                                            _ver.SetCollection(_collectionGeoCod);
                                        }
                                    }, item);
                                }
                                else
                                {
                                    _notifications.Notification(NotificationType.Error, _errorAddressEmpty);
                                }
                            }
                        }
                        else
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
                                        _ver.SetCollection(_collectionGeoCod);
                                    }
                                }, geocod);
                            }
                            else
                            {
                                _notifications.Notification(NotificationType.Error, _errorAddressEmpty);
                            }
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
                        //if (_collectionFiles != null && _collectionFiles.Any())
                        //{
                        //    GetAllGeooForFiles();
                        //}
                        //else
                        //{
                        GetAllGeoCod(_collectionGeoCod);
                        //}
                    }, () => (_collectionGeoCod != null && _collectionGeoCod.Any()) && !_isGeoCodingModelBusy));

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
                                    GetSettingsFromFile(a[0], _appSettings.FTPSettings, _appSettings.BDSettings);
                                }
                                else
                                {
                                    SetFileInput(a[0]);
                                }
                            }
                            //else if (a.Length > 1)
                            //{
                            //    SetCollectionFiles(a);
                            //}
                        }
                    }, obj => !_isStartGeoCoding));

        ///// <summary>
        ///// Команда обработки перетаскивания файлов в список файлов
        ///// </summary>
        //public RelayCommand<DragEventArgs> CommandDragDropFileForList =>
        //_commandDragDropFileForList ?? (_commandDragDropFileForList = new RelayCommand<DragEventArgs>(
        //            obj =>
        //            {
        //                if (obj.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
        //                {
        //                    var a = (string[])obj.Data.GetData(DataFormats.FileDrop, true);
        //                    SetCollectionFiles(a);
        //                }
        //            }));

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
        /// Команда для получения данных из базы данных
        /// </summary>
        public RelayCommand CommandGetDataFromBD =>
        _commandGetDataFromBD ?? (_commandGetDataFromBD = new RelayCommand(
                    async () =>
                    {
                        IsStartGetDataFromBD = true;
                        
                        
                       var result = await _modelBd.GetDataUserScriptFromBDAsync(_appSettings.BDSettings);
                        
                        CreateCollection(result.Entities, result.Error);
                        IsStartGetDataFromBD = false;

                        if (result.Error == null)
                        {
                            _appSettings.FilesSettings.FileOutput = SetDefNameFileOutput();
                        }
                    }, () => !string.IsNullOrEmpty(_appSettings.BDSettings.SQLQuery) && !_isStartGetDataFromBD && !_isStartGeoCoding));

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
                                var geo = _collectionSelectionItem.FirstOrDefault();
                                if (geo != null)
                                {
                                    geo.MainGeoCod = item;
                                    geo.Error = string.Empty;
                                    geo.Status = StatusType.OK;
                                    _stat.UpdateStatisticsCollection();
                                }

                                //_currentGeoCod.MainGeoCod = item;
                                // _currentGeoCod.Error = string.Empty;
                                //_currentGeoCod.Status = StatusType.OK;
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
        /// Метод для получения данных из файла
        /// </summary>
        private void GetDataFromFile()
        {
            _model.GetDataFromFile((list, error) =>
            {
                CreateCollection(list, error);
            }, _appSettings.FilesSettings.FileInput, _appSettings.FilesSettings.CanUseANSI);
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
            int parSize = _appSettings.FilesSettings.CanBreakFileOutput ? _appSettings.FilesSettings.MaxSizePart : 0;

            _model.SaveData(error =>
            {
                // Оповещаем о записи
                _notifications.Notification(NotificationType.SaveData, error, true);
                if (error == null)
                {
                    if (_appSettings.GeoCodSettings.CanOpenFolderAfter)
                    {
                        AppSettings.OpenFolder(_appSettings.FilesSettings.FileOutput);
                    }
                }
            }, _collectionGeoCod, _appSettings.FilesSettings.FileOutput, parSize, _appSettings.FilesSettings.CanCopyFileOutputToFtp, _appSettings.FTPSettings);
        }

        /// <summary>
        /// Метод установки файла с данными
        /// </summary>
        /// <param name="nameFile"></param>
        private void SetFileInput(string nameFile)
        {
            // Сохраняем полное имя файла в свойство FilesInput
            _appSettings.FilesSettings.FileInput = nameFile;
            // Если данные получать сразу, то получаем
            if (_appSettings.FilesSettings.CanGetDataOnce && !string.IsNullOrEmpty(_appSettings.FilesSettings.FileInput))
            {
                // Получаем данные из файла
                GetDataFromFile();
            }

            _appSettings.FilesSettings.FileOutput = SetDefNameFileOutput();

            if (_collectionGeoCod != null && _collectionGeoCod.Any() && _appSettings.GeoCodSettings.CanGeoCodAfterGetFile)
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
            string defName;
            if (_collectionGeoCod != null && _collectionGeoCod.Any())
            {
                defName = $"{NameFile()}_UpLoad_{_collectionGeoCod.Count}.csv";
            }
            else
            {
                defName = $"{NameFile()}_UpLoad.csv";
            }

            return $"{_appSettings.FilesSettings.FolderOutput}\\{defName}";
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
                defName = $"{_appSettings.FilesSettings.FolderErrors}\\{NameFile()}_Errors_{countError}.csv";
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
                defName = $"{_appSettings.FilesSettings.FolderTemp}\\{NameFile()}_Temp_{_collectionGeoCod.Count}.csv";
            }

            return defName;
        }

        private string NameFile()
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(_appSettings.FilesSettings.FileInput);
            name = string.IsNullOrEmpty(name) ? "data" : name;
            return $"{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}_{name}";
        }

        /// <summary>
        /// Метод для сохранения файла с ошибками
        /// </summary>
        private void SaveErrors()
        {
            var nameFile = SetDefNameFileErrors();
            _model.SaveError(error =>
            {
                _notifications.Notification(NotificationType.Error, error);
            }, _collectionGeoCod, nameFile);
        }

        /// <summary>
        /// Метод для сохранения временных файлов
        /// </summary>
        private void SaveTemp()
        {
            var nameFile = SetDefNameFileTemp();
            _model.SaveTemp(error =>
            {
                _notifications.Notification(NotificationType.Error, error);
            }, _collectionGeoCod, nameFile);
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

            _model.SaveStatistics(e =>
            {
                _notifications.Notification(NotificationType.Error, e);
                if (e == null)
                {
                    _stat.IsSave = true;
                }
            }, _stat.Statistics, _appSettings.FilesSettings);
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

            if(data != null)
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.Address));

                int countData = data.Count();
                if (countData > 0)
                {
                    // Отображаем индикацию работы процесса
                    IsStartGeoCoding = true;
                    //_stat.Start(_appSettings.GeoCodSettings.GeoService);
                    _stat.Start(_appSettings.GeoCodSettings.CurrentGeoCoder.GeoCoder, _geoCodingModel.GetKeyShort());

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

                        if (_appSettings.GeoCodSettings.CanSaveDataAsFinished && !string.IsNullOrEmpty(_appSettings.FilesSettings.FileOutput))
                        {
                            SaveData();
                            SaveErrors();
                        }

                        if (_appSettings.GeoCodSettings.CanSaveDataAsTemp)
                        {
                            SaveTemp();
                        }
                        if (_appSettings.GeoCodSettings.CanSaveStatistics)
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

            if (_appSettings.GeoCodSettings.CanGeoCodGetAll)
            {
                data = collectionItem;
            }
            else if (_appSettings.GeoCodSettings.CanGeoCodGetError)
            {
                data = collectionItem.Where(x => x.Status == StatusType.Error);
            }
            else if (_appSettings.GeoCodSettings.CanGeoCodGetNotGeo)
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
                    _ver.SetCollection(_collectionGeoCod);
                }
                else
                {
                    _notifications.Notification(NotificationType.DataEmpty);
                }
            }
        }

        #endregion PrivateMethod

        #region NeedToMakeOut

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

        //private ObservableCollection<EntityFile> _collectionFiles;
        ///// <summary>
        ///// 
        ///// </summary>
        //public ObservableCollection<EntityFile> CollectionFiles
        //{
        //    get => _collectionFiles;
        //    set => Set(ref _collectionFiles, value);
        //}

        //private RelayCommand _commandGetFiles;
        //public RelayCommand CommandGetFiles =>
        //_commandGetFiles ?? (_commandGetFiles = new RelayCommand(
        //            () =>
        //            {
        //                _model.GetFiles((d, e) =>
        //                {
        //                    // Оповещаем если ошибка
        //                    _notifications.Notification(NotificationType.Error, e);

        //                    if (e == null)
        //                    {
        //                        SetCollectionFiles(d);
        //                    }
        //                });
        //            }, () => !_isStartGeoCoding));

        //private RelayCommand _commandClearCollectionFiles;
        //public RelayCommand CommandClearCollectionFiles =>
        //_commandClearCollectionFiles ?? (_commandClearCollectionFiles = new RelayCommand(
        //            () =>
        //            {
        //                CollectionFiles?.Clear();
        //            }, () => !_isStartGeoCoding && _collectionFiles != null && _collectionFiles.Any()));

        //private RelayCommand<EntityFile> _commandRemoveFilesFromCollection;
        //public RelayCommand<EntityFile> CommandRemoveFilesFromCollection =>
        //_commandRemoveFilesFromCollection ?? (_commandRemoveFilesFromCollection = new RelayCommand<EntityFile>(
        //            obj =>
        //            {
        //                _collectionFiles.Remove(obj);
        //            }, !_isStartGeoCoding));

        //private void SetCollectionFiles(IEnumerable<string> d)
        //{
        //    if (d != null && d.Any())
        //    {
        //        if (_collectionFiles == null || !_collectionFiles.Any())
        //        {
        //            CollectionFiles = new ObservableCollection<EntityFile>(d.Select(x =>
        //            {
        //                return new EntityFile() { NameFile = x };
        //            }));
        //        }
        //        else
        //        {
        //            foreach (var item in d)
        //            {
        //                _collectionFiles.Add(new EntityFile() { NameFile = item });
        //            }
        //        }

        //        GetDataAboutFiles();
        //    }
        //}

        //private void GetDataAboutFiles()
        //{
        //    IsStartGetDataAboutFiles = true;

        //    _model.GetDataAboutFiles(e =>
        //    {
        //        if (e == null)
        //        {
        //            IsStartGetDataAboutFiles = false;
        //        }
        //    }, _collectionFiles, _appSettings.FilesSettings.CanUseANSI);
        //}

        //private bool _isStartGetDataAboutFiles = false;
        ///// <summary>
        ///// 
        ///// </summary>
        //public bool IsStartGetDataAboutFiles
        //{
        //    get => _isStartGetDataAboutFiles;
        //    set => Set(ref _isStartGetDataAboutFiles, value);
        //}

        //private RelayCommand _commandGetGeoForListFile;
        //public RelayCommand CommandGetGeoForListFile =>
        //_commandGetGeoForListFile ?? (_commandGetGeoForListFile = new RelayCommand(
        //            () =>
        //            {
        //                GetAllGeooForFiles();
        //            }));

        //private async void GetAllGeooForFiles()
        //{
        //    IsStartGeoCoding = true;

        //    foreach (var item in _collectionFiles)
        //    {
        //        if (item.FileType == FileType.Other)
        //        {
        //            continue;
        //        }

        //        item.Status = StatusType.Processed;

        //        _appSettings.FilesSettings.FileInput = item.NameFile;

        //        _model.GetDataFromFile((list, e) =>
        //        {
        //            CreateCollection(list, e);
        //        }, item.NameFile, _appSettings.FilesSettings.CanUseANSI);

        //        _appSettings.FilesSettings.FileOutput = SetDefNameFileOutput();

        //        IEnumerable<EntityGeoCod> data = null;

        //        data = GetCollectionWithFilter(_collectionGeoCod).Where(x => !string.IsNullOrEmpty(x.Address));

        //        int countData = data.Count();

        //        //_stat.Start(_appSettings.GeoCodSettings.GeoService);
        //        _stat.Start(_appSettings.GeoCodSettings.CurrentGeoCoder.GeoCoder);

        //        var error = await _geoCodingModel.GetAllGeoCod(data);

        //        if (_appSettings.GeoCodSettings.CanSaveDataAsFinished && !string.IsNullOrEmpty(_appSettings.FilesSettings.FileOutput))
        //        {
        //            SaveData();
        //            SaveErrors();
        //        }
        //        if (_appSettings.GeoCodSettings.CanSaveDataAsTemp)
        //        {
        //            SaveTemp();
        //        }
        //        if (_appSettings.GeoCodSettings.CanSaveStatistics)
        //        {
        //            SaveStatistics();
        //        }

        //        item.Collection = _collectionGeoCod.ToList();
        //        _stat.Stop();

        //        if (error == null)
        //        {
        //            item.Status = StatusType.OK;
        //        }
        //        else if (error.Message == _errorCancel)
        //        {
        //            item.Status = StatusType.Error;
        //            item.Error = error.Message;
        //            _notifications.Notification(NotificationType.Cancel);
        //            break;
        //        }
        //        else
        //        {
        //            item.Status = StatusType.Error;
        //            item.Error = error.Message;
        //        }
        //    }

        //    _notifications.Notification(NotificationType.Ok, "Обработка файлов завершена");
        //    IsStartGeoCoding = false;
        //}

        //private RelayCommand _myCommand;
        //public RelayCommand MyCommand =>
        //_myCommand ?? (_myCommand = new RelayCommand(
        //            () =>
        //            {
        //                CheckAllGeoFromFiles();
        //            }));

        //private async void CheckAllGeoFromFiles()
        //{
        //    foreach (var item in _collectionFiles)
        //    {
        //        if (item.FileType == FileType.Other)
        //        {
        //            continue;
        //        }

        //        item.Status = StatusType.Processed;
        //        _appSettings.FilesSettings.FileInput = item.NameFile;

        //        if (item.FileType == FileType.Temp)
        //        {
        //            _model.GetDataFromFile((list, e) =>
        //            {
        //                CreateCollection(list, e);
        //            }, item.NameFile, _appSettings.FilesSettings.CanUseANSI);
        //        }

        //        _appSettings.FilesSettings.FileOutput = SetDefNameFileOutput();

        //        var error = await _ver.CheckAll();
        //        if (error == null)
        //        {
        //            item.Status = StatusType.OK;
        //        }
        //        else
        //        {
        //            item.Error = error.Message;
        //            item.Status = StatusType.Error;
        //        }
        //    }
        //}

        //private RelayCommand<EntityFile> _commandOpenFile;
        //public RelayCommand<EntityFile> CommandOpenFile =>
        //_commandOpenFile ?? (_commandOpenFile = new RelayCommand<EntityFile>(
        //            obj =>
        //            {
        //                _appSettings.FilesSettings.FileInput = obj.NameFile;

        //                if (obj.Collection != null && obj.Collection.Any())
        //                {
        //                    CreateCollection(obj.Collection, null);
        //                }
        //                else
        //                {
        //                    _model.GetDataFromFile((list, e) =>
        //                    {
        //                        CreateCollection(list, e);
        //                    }, obj.NameFile, _appSettings.FilesSettings.CanUseANSI);
        //                }

        //                _appSettings.FilesSettings.FileOutput = SetDefNameFileOutput();
        //            }, obj => obj != null && obj.FileType != FileType.Other));

        private RelayCommand<EntityGeoCod> _commandCopyGeoDatad;
        public RelayCommand<EntityGeoCod> CommandCopyGeoData =>
        _commandCopyGeoDatad ?? (_commandCopyGeoDatad = new RelayCommand<EntityGeoCod>(
                    geoCod =>
                    {
                        CopiedGeoCod = geoCod.MainGeoCod;

                        try
                        {
                            string data = $"{geoCod.MainGeoCod?.Longitude}:{geoCod.MainGeoCod?.Latitude}";

                            Clipboard.SetText(data, TextDataFormat.UnicodeText);
                        }
                        catch (Exception ex)
                        {
                            _notifications.Notification(NotificationType.Error, ex.Message);
                        }
                    }));

        private RelayCommand<object> _commandSelectionChanged;
        public RelayCommand<object> CommandSelectionChanged =>
        _commandSelectionChanged ?? (_commandSelectionChanged = new RelayCommand<object>(
                    obj =>
                    {
                        var col = (IList)obj;
                        CollectionSelectionItem = col.Cast<EntityGeoCod>().ToList();
                    }));

        private GeoCod _copiedGeoCod;
        /// <summary>
        /// 
        /// </summary>
        public GeoCod CopiedGeoCod
        {
            get => _copiedGeoCod;
            set => Set(ref _copiedGeoCod, value);
        }

        private List<EntityGeoCod> _collectionSelectionItem;
        /// <summary>
        /// 
        /// </summary>
        public List<EntityGeoCod> CollectionSelectionItem
        {
            get => _collectionSelectionItem;
            set => Set(ref _collectionSelectionItem, value);
        }

        private RelayCommand _commandSetGeoData;
        public RelayCommand CommandSetGeoData =>
        _commandSetGeoData ?? (_commandSetGeoData = new RelayCommand(
                    () =>
                    {
                        if (_collectionSelectionItem != null && _collectionSelectionItem.Any())
                        {
                            foreach (var item in _collectionSelectionItem)
                            {
                                item.MainGeoCod = _copiedGeoCod;
                                item.Status = StatusType.OK;
                                item.Error = string.Empty;
                                item.DateTimeGeoCod = DateTime.Now;
                            }
                        }
                        else if (_currentGeoCod != null)
                        {
                            CurrentGeoCod.MainGeoCod = _copiedGeoCod;
                            CurrentGeoCod.Status = StatusType.OK;
                            CurrentGeoCod.Error = string.Empty;
                            CurrentGeoCod.DateTimeGeoCod = DateTime.Now;
                        }

                        _customerView.Refresh();
                        _stat.UpdateStatisticsCollection();
                    }, () => _copiedGeoCod != null));

        private RelayCommand _commandSetFirstGeoCod;
        public RelayCommand CommandSetFirstGeoCod =>
        _commandSetFirstGeoCod ?? (_commandSetFirstGeoCod = new RelayCommand(
                    () =>
                    {
                        if (_collectionSelectionItem != null && _collectionSelectionItem.Any())
                        {
                            foreach (var item in _collectionSelectionItem)
                            {
                                if (item.ListGeoCod != null && item.ListGeoCod.Any())
                                {
                                    item.MainGeoCod = item.ListGeoCod.FirstOrDefault();
                                    item.Status = StatusType.OK;
                                    item.Error = string.Empty;
                                    item.DateTimeGeoCod = DateTime.Now;
                                }
                            }

                            _customerView.Refresh();
                            _stat.UpdateStatisticsCollection();
                        }
                    }));

        #endregion NeedToMakeOut

        private bool _isGeoCodingModelBusy = false;
        /// <summary>
        /// Отображать когда геокодер занят
        /// </summary>
        public bool IsGeoCodingModelBusy
        {
            get => _isGeoCodingModelBusy;
            set => Set(ref _isGeoCodingModelBusy, value);
        }

        private async void SetGeoService()
        {
            IsGeoCodingModelBusy = true;
            var r = await _geoCodingModel.SetGeoService();
            if (!r.Successfully)
            {
                _notifications.Notification(NotificationType.Error, r.Error.Message);
            }
            IsGeoCodingModelBusy = false;
        }

        private RelayCommand _commandGetSqlTemplateNewAddress;
        public RelayCommand CommandGetSqlTemplateNewAddress =>
        _commandGetSqlTemplateNewAddress ?? (_commandGetSqlTemplateNewAddress = new RelayCommand(
                    async () =>
                    {
                        var result = await _geoCodingModel.GetCurrentLimit();
                        if (result.Successfully)
                        {
                            AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTemplateNewAddress(result.Entity);
                        }
                        else
                        {
                            AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTemplateNewAddress(10000);
                        }
                    }));

        private RelayCommand _commandGetSqlTemplateOldBadAddress;
        public RelayCommand CommandGetSqlTemplateOldBadAddress =>
        _commandGetSqlTemplateOldBadAddress ?? (_commandGetSqlTemplateOldBadAddress = new RelayCommand(
                    async () =>
                    {
                        var result = await _geoCodingModel.GetCurrentLimit();
                        if (result.Successfully)
                        {
                            AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTempleteOldBadAddresss(result.Entity);
                        }
                        else
                        {
                            AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTempleteOldBadAddresss(10000);
                        }
                    }));



        private RelayCommand _commandGetSqlTempleteNewOldAddressProcedure;
        public RelayCommand CommandGetSqlTempleteNewOldAddressProcedure =>
        _commandGetSqlTempleteNewOldAddressProcedure ?? (_commandGetSqlTempleteNewOldAddressProcedure = new RelayCommand(
        async () =>
        {
            if (_isGeoCodingModelBusy)
            {
                AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTempleteNewOldAddressProcedure(10000);
            }
            else
            {
                var result = await _geoCodingModel.GetCurrentLimit();
                if (result.Successfully)
                {
                    AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTempleteNewOldAddressProcedure(result.Entity);
                }
                else
                {
                    AppSettings.BDSettings.SQLQuery = _modelBd.GetSqlTempleteNewOldAddressProcedure(10000);
                }
            }
        }));
      }
}