using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System;
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
        /// Заголовок оповещения с ошибками
        /// </summary>
        private const string _headerNotificationError = "Ошибка";
        /// <summary>
        /// Заголовок оповещения с отмененной операцией
        /// </summary>
        private const string _headerNotificationCancel = "Обработка прекращена";
        /// <summary>
        /// Заголовок оповещения по завершению обработки данных
        /// </summary>
        private const string _headerNotificationDataProcessed = "Данные обработаны";
        /// <summary>
        /// Заголовок оповещения по завершению обработки данных
        /// </summary>
        private const string _headerNotificationSaveData = "Сохранение файла";
        /// <summary>
        /// Заголовок оповещения при успешном сохранении настроек
        /// </summary>
        private const string _headerNotificationSettingsSave = "Настройки сохранены успешно";
        /// <summary>
        /// Заголовок окна при запуске процесса геокодирования с пустой коллекцией 
        /// </summary>
        private const string _headerNotificationErrorDataEmpty = "Нечего обрабатывать";
        /// <summary>
        /// Заголовок окна при попытке сохранения статистики которая уже сохранена была
        /// </summary>
        private const string _headerNotificationStatAlreadySave = "Статистика уже был сохранена";
        /// <summary>
        /// Текст сообщения при попытке сохранить статистику которая уже сохранена
        /// </summary>
        private const string _notificationStatAlreadySave = "В статистике с последнего раза ничего не изменилось";
        /// <summary>
        /// Сообщение об отмене операции
        /// </summary>
        private const string _errorCancel = "Операция была отменена.";
        /// <summary>
        /// Текст сообщения если адрес пустой при геокодировании
        /// </summary>
        private const string _errorAddressEmpty = "Адрес пуст";
        /// <summary>
        /// Сообщение об успешности записи в файл
        /// </summary>
        private const string _messageSaveData = "Сохранение данных в файл завершено успешно";
        /// <summary>
        /// Всего адресов
        /// </summary>
        private const string _allAddress = "Всего адресов:";
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

        /// <summary>
        /// Поле для хранения ссылки на модельгеокодирования
        /// </summary>
        private readonly GeoCodingModel _geoCodingModel;

        /// <summary>
        /// Поле для хранения ссылки на координатора диалогов
        /// </summary>
        private readonly IDialogCoordinator dialogCoordinator = DialogCoordinator.Instance;

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
        /// Поле для хранения настроек входного и выходного файла
        /// </summary>
        private FilesSettings _filesSettings;

        /// <summary>
        /// Поле для хранения запущена ли процедура декодирования
        /// </summary>
        private bool _isStartGeoCoding = false;

        /// <summary>
        /// Поле для хранения запущено ли получение данных из БД
        /// </summary>
        private bool _isStartGetDataFromBD = false;

        /// <summary>
        /// Поле для хранения ссылки на текущий выбранный геосервис
        /// </summary>
        private GeoCodingService.IGeoCodingService _currentGeoService;

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
        /// Поле для хранения статистики
        /// </summary>
        private StatisticsViewModel _stat;

        /// <summary>
        /// Поле для хранения ссылки на текущую тему оформления
        /// </summary>
        private Theme _colorTheme = ThemeManager.DetectTheme();

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

        #endregion PrivateFields

        #region PublicPropertys

        /// <summary>
        /// Коллекция с данными
        /// </summary>
        public ObservableCollection<EntityGeoCod> CollectionGeoCod
        {
            get => _collectionGeoCod;
            set
            {
                Set(ref _collectionGeoCod, value);
                DispatcherHelper.CheckBeginInvokeOnUI(() => _stat.Init(value));
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
        /// Запущена ли процедура декодирования
        /// </summary>
        public bool IsStartGeoCoding
        {
            get => _isStartGeoCoding;
            set => Set(ref _isStartGeoCoding, value);
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
        /// Текущий геосервис
        /// </summary>
        public GeoCodingService.IGeoCodingService CurrentGeoService
        {
            get => _currentGeoService;
            set
            {
                _geoCodSettings.GeoService = value.Name;
                _model.SetGeoService(value);
                _geoCodingModel.SetGeoService(value);
                Set(ref _currentGeoService, value);
            }
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
        /// настройки БД
        /// </summary>
        public BDSettings BDSettings
        {
            get => _bdSettings;
            set => Set(ref _bdSettings, value);
        }

        /// <summary>
        /// Статистика выполнения геокодирования
        /// </summary>
        public StatisticsViewModel Stat
        {
            get => _stat;
            set => Set(ref _stat, value);
        }

        /// <summary>
        /// Текущая тема оформления окна
        /// </summary>
        public Theme ColorTheme
        {
            get => _colorTheme;
            set
            {
                Set(ref _colorTheme, value);
                ThemeManager.ChangeTheme(Application.Current, value.Name);
            }
        }

        /// <summary>
        /// Коллекция всех возможных тем оформления окна
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Theme> ListTheme => ThemeManager.Themes;

        /// <summary>
        /// Коллекция всех возможных геосервисов
        /// </summary>
        public ReadOnlyObservableCollection<GeoCodingService.IGeoCodingService> CollectionGeoService => GeoCodingService.MainGeoService.AllService;

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
                            if (er == null)
                            {
                                if (!string.IsNullOrEmpty(f))
                                {
                                    SetFileInput(f);
                                }
                            }
                            else
                            {
                                // Оповещаем, если были ошибки
                                NotificationPlainText(_headerNotificationError, er.Message);
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
                            if (error == null)
                            {
                                if (!string.IsNullOrEmpty(file))
                                {
                                    // Сохраняем полное имя файла в свойстве FileOutput
                                    FilesSettings.FileOutput = file;
                                }
                            }
                            else
                            {
                                // Оповещаем, если были ошибки
                                NotificationPlainText(_headerNotificationError, error.Message);
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
                                if (er == null)
                                {
                                    Customers.Refresh();
                                    _stat.UpdateStatisticsCollection();
                                }
                                else
                                {
                                    // Оповещаем если были ошибки
                                    NotificationPlainText(_headerNotificationError, er.Message);
                                }
                            }, geocod);
                        }
                        else
                        {
                            NotificationPlainText(_headerNotificationError, _errorAddressEmpty);
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
                        GetAllGeoCod();
                    }, () => _collectionGeoCod != null && _collectionGeoCod.Count > 0));

        /// <summary>
        /// Команда остановки геокодирования
        /// </summary>
        public RelayCommand CommandStopGeoCoding =>
        _commandStopGeoCoding ?? (_commandStopGeoCoding = new RelayCommand(
                    () =>
                    {
                        // Останавливаем процесс
                        _geoCodingModel.StopGet();
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
                        NotificationPlainText(_headerNotificationError, ex.Message);
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
                         NotificationPlainText(_headerNotificationError, ex.Message);
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
                          System.Diagnostics.Process.Start(_currentGeoService.GetUrlRequest(obj.Address));
                      }
                      catch (Exception ex)
                      {
                          NotificationPlainText(_headerNotificationError, ex.Message);
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
                            if (a.Length > 0)
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
                            if (e == null)
                            {
                                NotificationPlainText(_headerNotificationSettingsSave, null);
                            }
                            else
                            {
                                NotificationPlainText(_headerNotificationError, e.Message);
                            }
                        }, _filesSettings, _ftpSettings, _geoCodSettings, _bdSettings, ColorTheme.Name);
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
                            Clipboard.SetText(_currentGeoService.GetUrlRequest(obj.Address), TextDataFormat.UnicodeText);
                        }
                        catch (Exception ex)
                        {
                            NotificationPlainText(_headerNotificationError, ex.Message);
                        }
                    }));

        /// <summary>
        /// Команда для проверки соединения с базой
        /// </summary>
        public RelayCommand CommandCheckConnect =>
        _commandCheckConnect ?? (_commandCheckConnect = new RelayCommand(
                    () =>
                    {
                        _bdSettings.StatusConnect = StatusConnect.ConnectNow;
                        _bdSettings.Error = string.Empty;
                        _model.ConnectBDAsync(e =>
                        {
                            if (e != null)
                            {
                                NotificationPlainText(_headerNotificationError, e.Message);
                                _bdSettings.StatusConnect = StatusConnect.Error;
                                _bdSettings.Error = e.Message;
                            }
                            else
                            {
                                _bdSettings.StatusConnect = StatusConnect.OK;
                                _bdSettings.Error = string.Empty;
                            }
                        }, _bdSettings);
                    }, () => !string.IsNullOrEmpty(_bdSettings.Server) || !string.IsNullOrEmpty(_bdSettings.BDName) || _bdSettings.StatusConnect == StatusConnect.ConnectNow));

        /// <summary>
        /// Команда для проверки соединения с фтп-сервером
        /// </summary>
        public RelayCommand CommandCheckConnectFtp =>
        _commandCheckConnectFtp ?? (_commandCheckConnectFtp = new RelayCommand(
                    () =>
                    {
                        _ftpSettings.StatusConnect = StatusConnect.ConnectNow;
                        _ftpSettings.Error = string.Empty;
                        _model.ConnectFTPAsync(e =>
                        {
                            if (e != null)
                            {
                                NotificationPlainText(_headerNotificationError, e.Message);
                                _ftpSettings.StatusConnect = StatusConnect.Error;
                                _ftpSettings.Error = e.Message;
                            }
                            else
                            {
                                FTPSettings.StatusConnect = StatusConnect.OK;
                                _ftpSettings.Error = string.Empty;
                            }
                        }, _ftpSettings);

                    }, () => !string.IsNullOrEmpty(_ftpSettings.Server) || _ftpSettings.StatusConnect == StatusConnect.ConnectNow));

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
                            CreateCollection(data, error);
                            IsStartGetDataFromBD = false;
                        }, _bdSettings, _bdSettings.SQLQuery);

                    }, () => !string.IsNullOrEmpty(_bdSettings.SQLQuery) && !_isStartGetDataFromBD));

        /// <summary>
        /// Метод для создание коллекции из листа данных из файла или БД
        /// </summary>
        /// <param name="data">Лист данных</param>
        /// <param name="error">Ошибки</param>
        private void CreateCollection(System.Collections.Generic.IEnumerable<EntityGeoCod> data, Exception error)
        {
            if (error == null)
            {
                if (data.Any())
                {
                    // Создаем коллекцию с данными
                    CollectionGeoCod = new ObservableCollection<EntityGeoCod>(data);

                    // Создаем представление, группируем по ошибкам и отбираем только объекты с ошибками
                    Customers = new CollectionViewSource { Source = CollectionGeoCod }.View;
                    Customers.GroupDescriptions.Add(new PropertyGroupDescription("Error"));
                    Customers.Filter = CustomerFilter;
                }
                else
                {
                    NotificationPlainText("Данных нет", null);
                }
            }
            else
            {
                // Оповещаем если были ошибки
                NotificationPlainText(_headerNotificationError, error.Message);
            }
        }

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
                                _customerView.Refresh();
                                _stat.UpdateStatisticsCollection();
                            }
                        }
                    }));

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
        /// Метод для оповещения о выполненных процессах
        /// </summary>
        /// <param name="header">Заголовок оповещения</param>
        /// <param name="message">Сообщение оповещения</param>
        private async void NotificationPlainText(string header, string message)
        {
            await dialogCoordinator.ShowMessageAsync(this, header, message);
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
                if (error == null)
                {
                    // Оповещаем о успешности записи
                    NotificationPlainText(_headerNotificationSaveData, _messageSaveData);
                    if (_geoCodSettings.CanOpenFolderAfter)
                    {
                        OpenFolder(_filesSettings.FileOutput);
                    }
                }
                else
                {
                    // Оповещаем если были ошибки
                    NotificationPlainText(_headerNotificationError, error.Message);
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
                if (e != null)
                {
                    // Оповещаем если были ошибки
                    NotificationPlainText(_headerNotificationError, e.Message);
                }
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
                GetAllGeoCod();
            }
        }

        /// <summary>
        /// Метод задания имени файла для сохранения по умолчанию
        /// </summary>
        /// <returns>Возвращает полное имя файла</returns>
        private string SetDefNameFileOutput()
        {
            string defNameOutput = string.Empty;

            if (_collectionGeoCod != null && _collectionGeoCod.Count > 0)
            {
                defNameOutput = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_filesSettings.FileInput)}_UpLoad_{_collectionGeoCod.Count}.csv";
            }
            else
            {
                defNameOutput = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_filesSettings.FileInput)}_UpLoad.csv";
            }

            return $"{_filesSettings.FolderOutput}\\{defNameOutput}";
        }

        /// <summary>
        /// Метод для получения имени файла с ошибками
        /// </summary>
        /// <returns>Имя файла</returns>
        private string SetDefNameFileErrors()
        {
            string defName = string.Empty;

            if (_collectionGeoCod != null && _collectionGeoCod.Count > 0)
            {
                int countError = _collectionGeoCod.Count(x => x.Status == StatusType.Error);
                defName = $"{_filesSettings.FolderErrors}\\{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_filesSettings.FileInput)}_Errors_{countError}.csv";
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

            if (_collectionGeoCod != null && _collectionGeoCod.Count > 0)
            {
                defName = $"{_filesSettings.FolderTemp}\\{DateTime.Now.ToString("yyyy_MM_dd")}_{System.IO.Path.GetFileNameWithoutExtension(_filesSettings.FileInput)}_Temp_{_collectionGeoCod.Count}.csv";
            }

            return defName;
        }

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
            if (_collectionGeoCod != null && _collectionGeoCod.Count(x => x.Status == StatusType.Error) > 0)
            {
                var nameFile = SetDefNameFileErrors();
                _model.SaveError(error =>
                {
                    if (error != null)
                    {
                        NotificationPlainText(_headerNotificationError, $"{error.Message}\n\r{nameFile}");
                    }
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
                    if (error != null)
                    {
                        NotificationPlainText(_headerNotificationError, $"{error.Message}\n\r{nameFile}");
                    }
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
                NotificationPlainText(_headerNotificationStatAlreadySave, _notificationStatAlreadySave);
                return;
            }

            var nameFile = SetDefNameFileStatistics();
            _model.SaveStatistics(e =>
            {
                if (e != null)
                {
                    NotificationPlainText(_headerNotificationError, $"{e.Message}\n\r{nameFile}");
                }
                else
                {
                    _stat.IsSave = true;
                }
            }, _stat.Statistics, _filesSettings, nameFile);
        }

        /// <summary>
        /// Метод для получения координат для всей коллекции 
        /// </summary>
        private void GetAllGeoCod()
        {
            System.Collections.Generic.IEnumerable<EntityGeoCod> data = null;

            if (_geoCodSettings.CanGeoCodGetAll)
            {
                data = _collectionGeoCod;
            }
            else if (_geoCodSettings.CanGeoCodGetError)
            {
                data = _collectionGeoCod.Where(x => x.Status == StatusType.Error);
            }
            else if (_geoCodSettings.CanGeoCodGetNotGeo)
            {
                data = _collectionGeoCod.Where(x => x.Status == StatusType.Error || x.Status == StatusType.NotGeoCoding);
            }

            data = data.Where(x => !string.IsNullOrEmpty(x.Address));

            int coutData = data.Count();
            if (coutData > 0)
            {
                // Отображаем индикацию работы процесса
                IsStartGeoCoding = true;
                _stat.Start();

                _geoCodingModel.GetAllGeoCod((r, i, e) =>
                {
                    IsStartGeoCoding = false;
                    _stat.Stop();
                    Customers.Refresh();

                    if (e == null)
                    {
                        // Оповещаем о завершении получении координат
                        NotificationPlainText(_headerNotificationDataProcessed, $"{_processedcompleted} {coutData}");
                    }
                    else if (e.Message == _errorCancel)
                    {
                        // Оповещаем если сами отменили
                        NotificationPlainText(_headerNotificationCancel, e.Message);
                    }
                    else
                    {
                        // Оповещаем если были ошибки и номер на котором была остановка
                        NotificationPlainText(_headerNotificationError, $"{e.Message}");
                    }

                    if (_geoCodSettings.CanSaveDataAsFinished && !string.IsNullOrEmpty(_filesSettings.FileOutput) && coutData > 0)
                    {
                        SaveData();
                        SaveErrors();
                    }

                    if (_geoCodSettings.CanSaveDataAsTemp && coutData > 0)
                    {
                        SaveTemp();
                    }
                    if (_geoCodSettings.CanSaveStatistics)
                    {
                        SaveStatistics();
                    }
                }, data);
            }
            else
            {
                NotificationPlainText(_headerNotificationErrorDataEmpty, null);
            }
        }

        /// <summary>
        /// Метод для настройки программ из файла настроек
        /// </summary>
        /// <param name="file">Имя файла с настройками</param>
        /// <param name="ftp">Ссылка на настройки фтп</param>
        /// <param name="bd">Сылка на настройки БД</param>
        private void GetSettingsFromFile(string file, FTPSettings ftp, BDSettings bd)
        {
            _model.GetSettingsFromFile(e =>
            {
                if (e != null)
                {
                    NotificationPlainText(_headerNotificationError, e.Message);
                }
                else
                {
                    NotificationPlainText(_headerNotificationDataProcessed, _settingsGood);
                }
            }, file, ftp, bd);
        }

        #endregion PrivateMethod

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
            Stat = new StatisticsViewModel();
            _geoCodingModel = new GeoCodingModel();

            _model.GetSettings((e, f, g, ftp, bds, c) =>
            {
                if (e == null)
                {
                    FilesSettings = f;
                    GeoCodSettings = g;
                    FTPSettings = ftp;
                    BDSettings = bds;
                    ColorTheme = ThemeManager.ChangeTheme(Application.Current, c);
                    CurrentGeoService = CollectionGeoService.FirstOrDefault(x => x.Name == g.GeoService);
                }
                else
                {
                    NotificationPlainText(_headerNotificationError, e.Message);
                }
            });
        }
    }
}