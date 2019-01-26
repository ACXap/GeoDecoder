﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace GeoCoding
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region PrivateConst
        /// <summary>
        /// Заголовок оповещения с ошибками 
        /// </summary>
        private const string _headerNotificationError = "Ошибка";
        /// <summary>
        /// Заголовок оповещения по завершению обработки данных
        /// </summary>
        private const string _headerNotificationDataProcessed = "Данные обработаны";
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
        private FilesSettings _files;

        /// <summary>
        /// поле для хранения ссылки на команду получения полного имени файла
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
        public FilesSettings Files
        {
            get => _files;
            set => Set("Files", ref _files, value);
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
                        _model.GetFile((f, er) =>
                        {
                            if (er == null)
                            {
                                // Сохраняем полное имя файла в свойство FilesInput
                                Files.FileInput = f;
                            }
                            else
                            {
                                // Оповещаем, если были ошибки
                                NotificationPlainText(_headerNotificationError, er.Message);
                            }
                        });
                    }));

        /// <summary>
        /// Команда для выбора файла для сохранения (получения полного имени файла для сохранения)
        /// </summary>
        public RelayCommand CommandSetFileOutput =>
        _commandSetFileOutput ?? (_commandSetFileOutput = new RelayCommand(
                    () =>
                    {
                        _model.SetFileForSave((file, error) =>
                        {
                            if (error == null)
                            {
                                // Сохраняем полное имя файла в свойстве FileOutput
                                Files.FileOutput = file;
                            }
                            else
                            {
                                // Оповещаем, если были ошибки
                                NotificationPlainText(_headerNotificationError, error.Message);
                            }
                        });
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
                                NotificationPlainText("Сохранение файла", "Сохранение данных в файл завершено успешно");
                            }
                            else
                            {
                                NotificationPlainText(_headerNotificationError, error.Message);
                            }
                        }, _collectionGeoCod, _files.FileOutput);
                    }, () => !string.IsNullOrEmpty(_files.FileOutput) && _collectionGeoCod != null));

        /// <summary>
        /// Команда получения данных из файла
        /// </summary>
        public RelayCommand CommandGetDataFromFile =>
        _commandGetDataFromFile ?? (_commandGetDataFromFile = new RelayCommand(
                    () =>
                    {
                        _model.GetDataFromFile((list, error) =>
                        {
                            if (error == null)
                            {
                                // Создаем коллекцию с данными
                                CollectionGeoCod = new ObservableCollection<EntityGeoCod>(list);
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
                        }, _files.FileInput);
                    }, () => !string.IsNullOrEmpty(_files.FileInput)));

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

                            }
                            else
                            {
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
                        IsStartGeoCoding = true;

                        _model.GetAllGeoCod((r, i, e) =>
                        {
                            if (e == null)
                            {
                                // Оповещаем о завершении получении координат
                                NotificationPlainText(_headerNotificationDataProcessed, $"{_processedcompleted} {_collectionGeoCod.Count}");
                            }
                            else if(e.Message == "Операция была отменена.")
                            {
                                NotificationPlainText("Обработка прекращена", e.Message);
                            }
                            else
                            {
                                NotificationPlainText(_headerNotificationError, $"{e.Message}\n\r Процеес завершен на: {i} элементе");
                            }

                            IsStartGeoCoding = false;

                        }, _collectionGeoCod);

                    }, () => _collectionGeoCod != null && _collectionGeoCod.Count > 0));

        #endregion PublicCommands

        private bool _isStartGeoCoding = false;
        public bool IsStartGeoCoding
        {
            get => _isStartGeoCoding;
            set => Set("IsStartGeoCoding", ref _isStartGeoCoding, value);
        }

        private RelayCommand _commandStopGeoCoding;
        public RelayCommand CommandStopGeoCoding =>
        _commandStopGeoCoding ?? (_commandStopGeoCoding = new RelayCommand(
                    () =>
                    {
                        _model.StopGet();
                    }));

        private Statistics _statistics;
        public Statistics Statistics
        {
            get => _statistics;
            set => Set(ref _statistics, value);
        }

        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
            Files = new FilesSettings();

            Messenger.Default.Register<PropertyChangedMessage<StatusType>>(this, obj=>
            {
                UpdateStatistics();
            });
        }

        private async void NotificationPlainText(string header, string message)
        {
            await dialogCoordinator.ShowMessageAsync(this, header, message);
        }

        private void UpdateStatistics()
        {
            if(_collectionGeoCod!=null && _collectionGeoCod.Count>0)
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
    }
}