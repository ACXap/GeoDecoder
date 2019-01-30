using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
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
        /// Сообщение об отмене опрерации
        /// </summary>
        private const string _errorCancel = "Операция была отменена.";
        /// <summary>
        /// Сообщение об успешности записи в файл
        /// </summary>
        private const string _messageSaveData = "Сохранение данных в файл завершено успешно";
        /// <summary>
        /// Сообщение об отмене опрерации
        /// </summary>
        private const string _messageCancel = "Процеес завершен на:";
        /// <summary>
        /// Сообщение об отмене опрерации
        /// </summary>
        private const string _messageCancelEntity = "элементе";
        /// <summary>
        /// Всего адресов
        /// </summary>
        private const string _allAddress = "Всего адресов:";
        /// <summary>
        /// Сообщение о завершении обработки объектов
        /// </summary>
        private const string _processedcompleted = "Обработка завершилась. Всего обработано:";
        #endregion PrivateConst

        #region PrivateFields

        /// <summary>
        /// Поле для хранения ссылки на модель
        /// </summary>
        private readonly MainWindowModel _model;

        /// <summary>
        /// Поле для хранения ссылки на координатора диалогов
        /// </summary>
        private readonly IDialogCoordinator dialogCoordinator = DialogCoordinator.Instance;

        /// <summary>
        /// Поле для хранения ссылки на коллекцию с данными
        /// </summary>
        private ObservableCollection<EntityGeoCod> _collectionGeoCod;

        /// <summary>
        /// Поле для хранения настроек входного и выходного файла
        /// </summary>
        private FilesSettings _filesSettings;

        /// <summary>
        /// Поле для хранения статистики
        /// </summary>
        private Statistics _statistics;

        /// <summary>
        /// Поле для хранения запущена ли процедура декодирования
        /// </summary>
        private bool _isStartGeoCoding = false;

        /// <summary>
        /// Поле для хранения переменной получать данные сразу
        /// </summary>
        private bool _canGetDataOnce = true;

        /// <summary>
        /// Поле для хранения ссылки на команду получения полного имени файла
        /// </summary>
        private RelayCommand _commandGetFile;

        /// <summary>
        /// Поле для хранения ссылки на команду получения полного имени файла для сохранения
        /// </summary>
        private RelayCommand _commandSetFileOutput;

        /// <summary>
        /// Поле для хранения ссылки на комаду сохранения данных
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

        #endregion PrivateFields

        #region PublicPropertys

        /// <summary>
        /// Коллекция с данными
        /// </summary>
        public ObservableCollection<EntityGeoCod> CollectionGeoCod
        {
            get => _collectionGeoCod;
            set => Set("CollectionGeoCod", ref _collectionGeoCod, value);
        }

        /// <summary>
        /// Настройки входного и выходного файла
        /// </summary>
        public FilesSettings FilesSettings
        {
            get => _filesSettings;
            set => Set("FilesSettings", ref _filesSettings, value);
        }

        /// <summary>
        /// Запущена ли процедура декодирования
        /// </summary>
        public bool IsStartGeoCoding
        {
            get => _isStartGeoCoding;
            set => Set("IsStartGeoCoding", ref _isStartGeoCoding, value);
        }

        /// <summary>
        /// Получать данные сразу после выбора файла
        /// </summary>
        public bool CanGetDataOnce
        {
            get => _canGetDataOnce;
            set => Set(ref _canGetDataOnce, value);
        }

        /// <summary>
        /// Статистика по выполненому геокодированию
        /// </summary>
        public Statistics Statistics
        {
            get => _statistics;
            set => Set(ref _statistics, value);
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
                        string defFolder = string.Empty;
                        if (FilesSettings.IsFileInputOnFTP)
                        {
                            defFolder = $"{FTPSettings.Server}{FTPSettings.FolderOutput}";
                        }
                        _model.GetFile((f, er) =>
                        {
                            if (er == null)
                            {
                                // Сохраняем полное имя файла в свойство FilesInput
                                FilesSettings.FileInput = f;
                                // Если данные получать сразу, то получаем
                                if (_canGetDataOnce && !string.IsNullOrEmpty(_filesSettings.FileInput))
                                {
                                    // Получаем данные из файла
                                    GetDataFromFile();
                                }
                            }
                            else
                            {
                                // Оповещаем, если были ошибки
                                NotificationPlainText(_headerNotificationError, er.Message);
                            }
                        }, defFolder);
                    }));

        /// <summary>
        /// Команда для выбора файла для сохранения (получения полного имени файла для сохранения)
        /// </summary>
        public RelayCommand CommandSetFileOutput =>
        _commandSetFileOutput ?? (_commandSetFileOutput = new RelayCommand(
                    () =>
                    {
                        string defName = string.Empty;
                        // Имя файла по умолчанию, если есть непустая коллекция с данными
                        if (_collectionGeoCod != null && _collectionGeoCod.Count > 0)
                        {
                            defName = $"{DateTime.Now.ToString("yyyy_MM_dd")}_UpLoad_{_collectionGeoCod.Count}";
                        }
                        _model.SetFileForSave((file, error) =>
                        {
                            if (error == null)
                            {
                                // Сохраняем полное имя файла в свойстве FileOutput
                                FilesSettings.FileOutput = file;
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
                        _model.SaveData(error =>
                        {
                            if (error == null)
                            {
                                // Оповещаем о успешности записи
                                NotificationPlainText(_headerNotificationSaveData, _messageSaveData);
                            }
                            else
                            {
                                // Оповещаем если были ошибки
                                NotificationPlainText(_headerNotificationError, error.Message);
                            }
                        }, _collectionGeoCod, _filesSettings.FileOutput);
                    }, () => !string.IsNullOrEmpty(_filesSettings.FileOutput) && _collectionGeoCod != null));

        /// <summary>
        /// Команда получения данных из файла
        /// </summary>
        public RelayCommand CommandGetDataFromFile =>
        _commandGetDataFromFile ?? (_commandGetDataFromFile = new RelayCommand(
                    () =>
                    {
                        // Получаем данные из файла
                        GetDataFromFile();
                    }, () => !string.IsNullOrEmpty(_filesSettings.FileInput)));

        /// <summary>
        /// Команда для геокодирования объекта
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandGetGeoCod =>
        _commandGetGeoCod ?? (_commandGetGeoCod = new RelayCommand<EntityGeoCod>(
                    geocod =>
                    {
                        _model.GetGeoCod(er =>
                        {
                            if (er == null)
                            {
                                // Оповещаем после окончания геокодирования
                            }
                            else
                            {
                                // Оповещаем если были ошибки
                                NotificationPlainText(_headerNotificationError, er.Message);
                            }
                        }, geocod);
                    }));

        /// <summary>
        /// Команда открытия папки
        /// </summary>
        public RelayCommand<string> CommandOpenFolder =>
        _commandOpenFolder ?? (_commandOpenFolder = new RelayCommand<string>(
                    str =>
                    {
                        _model.OpenFolder(e =>
                        {
                            if (e != null)
                            {
                                // Оповещаем если были ошибки
                                NotificationPlainText(_headerNotificationError, e.Message);
                            }
                        }, str);
                    }, str => !string.IsNullOrEmpty(str)));

        /// <summary>
        /// Команда для получения координат для множества объектов
        /// </summary>
        public RelayCommand CommandGetAllGeoCod =>
        _commandGetAllGeoCod ?? (_commandGetAllGeoCod = new RelayCommand(
                    () =>
                    {
                        // Отображаем индикацию работы процесса
                        IsStartGeoCoding = true;

                        _model.GetAllGeoCod((r, i, e) =>
                        {
                            if (e == null)
                            {
                                // Оповещаем о завершении получении координат
                                NotificationPlainText(_headerNotificationDataProcessed, $"{_processedcompleted} {_collectionGeoCod.Count}");
                                Customers.Refresh();
                            }
                            else if (e.Message == _errorCancel)
                            {
                                // Оповещаем если сами отменили
                                NotificationPlainText(_headerNotificationCancel, e.Message);
                            }
                            else
                            {
                                // Оповещаем если были ошибки и номер на котором была остановка
                                NotificationPlainText(_headerNotificationError, $"{e.Message}\n\r {_messageCancel} {i} {_messageCancelEntity}");
                            }

                            // Прекращаем отображение
                            IsStartGeoCoding = false;

                        }, _collectionGeoCod);

                    }, () => _collectionGeoCod != null && _collectionGeoCod.Count > 0));

        /// <summary>
        /// Команда остановки геокодирования
        /// </summary>
        public RelayCommand CommandStopGeoCoding =>
        _commandStopGeoCoding ?? (_commandStopGeoCoding = new RelayCommand(
                    () =>
                    {
                        // Останавливаем процесс
                        _model.StopGet();
                    }));

        #endregion PublicCommands

        /// <summary>
        ///
        /// </summary>
        private GeoCodSettings _geoCodSettings;
        public GeoCodSettings GeoCodSettings
        {
            get => _geoCodSettings;
            set => Set(ref _geoCodSettings, value);
        }

        /// <summary>
        ///
        /// </summary>
        private FTPSettings _ftpSettings;
        public FTPSettings FTPSettings
        {
            get => _ftpSettings;
            set => Set(ref _ftpSettings, value);
        }

        private RelayCommand<EntityGeoCod> _commandCopyAddress;
        /// <summary>
        /// Команда копирования адреса в буфер
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandCopyAddress =>
        _commandCopyAddress ?? (_commandCopyAddress = new RelayCommand<EntityGeoCod>(
        obj =>
        {
            Clipboard.SetText(obj.Address, TextDataFormat.UnicodeText);
        }));

        private RelayCommand<EntityGeoCod> _commandOpenInBrowser;
        /// <summary>
        /// Команда открыть адрес в браузере
        /// </summary>
        public RelayCommand<EntityGeoCod> CommandOpenInBrowser =>
          _commandOpenInBrowser ?? (_commandOpenInBrowser = new RelayCommand<EntityGeoCod>(
          obj =>
          {
              System.Diagnostics.Process.Start(@"https://geocode-maps.yandex.ru/1.x/?geocode=" + obj.Address);
          }));

        private ICollectionView _customerView;
        public ICollectionView Customers
        {
            get => _customerView;
            set => Set(ref _customerView, value);
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
            FilesSettings = new FilesSettings();
            GeoCodSettings = new GeoCodSettings();
            FTPSettings = new FTPSettings();


            Messenger.Default.Register<PropertyChangedMessage<StatusType>>(this, obj =>
            {
                // Обновляем статистику
                UpdateStatistics();
            });
        }

        /// <summary>
        /// Метод для получения данных из файла
        /// </summary>
        private void GetDataFromFile()
        {
            _model.GetDataFromFile((list, error) =>
            {
                if (error == null)
                {
                    // Если колекция данных уже есть, освобождаем и уничтожаем, можно конечно спросить о нужности данных???
                    if (_collectionGeoCod != null && _collectionGeoCod.Count > 0)
                    {
                        _collectionGeoCod.Clear();
                        _collectionGeoCod = null;
                    }
                    // Создаем коллекцию с данными
                    CollectionGeoCod = new ObservableCollection<EntityGeoCod>(list);
                    Customers= new CollectionViewSource { Source= CollectionGeoCod }.View;
                  //  Customers = CollectionViewSource.GetDefaultView(CollectionGeoCod);
                    Customers.Filter = CustomerFilter;

                    // Обновляем статистику
                    UpdateStatistics();
                    // Оповещаем о создании коллекции
                    NotificationPlainText(_headerNotificationDataProcessed, $"{_allAddress} {_collectionGeoCod.Count}");
                }
                else
                {
                    // Оповещаем если были ошибки
                    NotificationPlainText(_headerNotificationError, error.Message);
                }
            }, _filesSettings.FileInput);
        }

        /// <summary>
        /// Метод для оповещения о выполенных процессах
        /// </summary>
        /// <param name="header">Заголовок оповещения</param>
        /// <param name="message">Сообщение оповещения</param>
        private async void NotificationPlainText(string header, string message)
        {
            await dialogCoordinator.ShowMessageAsync(this, header, message);
        }

        /// <summary>
        /// Метод для получения статистики
        /// </summary>
        private void UpdateStatistics()
        {
            if (_collectionGeoCod != null && _collectionGeoCod.Count > 0)
            {
                _model.UpdateStatistic((s, e) =>
                {
                    if (e == null)
                    {
                        Statistics = s;
                    }
                }, _collectionGeoCod);
            }
        }

        private bool CustomerFilter(object item)
        {
            EntityGeoCod customer = item as EntityGeoCod;
            return customer.Status==StatusType.Error;
        }
    }
}
