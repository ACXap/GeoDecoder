// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GeoCoding.Model;
using GeoCoding.Model.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GeoCoding
{
    public class BackGeoViewModel : ViewModelBase
    {
        public BackGeoViewModel()
        {
            _model = new MainWindowModel();
            _modelBd = new BdModel();

            Notifications = new NotificationsModel();
            AppSettings = new AppSettings(_notifications);
            Stat = new StatisticsViewModel();

            _geoCodingModel = new GeoCodingModel(_appSettings.NetSettings, _appSettings.GeoCodSettings, _appSettings.GetLimitsModel());
            SetGeoService();

            Messenger.Default.Register<PropertyChangedMessage<EntityGeoCoder>>(this, data =>
            {
                if (data.PropertyName == "CurrentGeoCoder")
                {
                    SetGeoService();
                }
            });

            BindingOperations.EnableCollectionSynchronization(CollectionLog, new object());
        }

        #region PrivateField

        private readonly DateTime _timeStartSpendingAllLimits = new DateTime(1, 1, 1, 3, 32, 0);

        /// <summary>
        /// Поле для хранения статистики
        /// </summary>
        private StatisticsViewModel _stat;

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

        /// <summary>
        /// Поле для хранения ссылки на модельгеокодирования
        /// </summary>
        private readonly GeoCodingModel _geoCodingModel;

        /// <summary>
        /// Поле для хранения ссылки на модель информирования
        /// </summary>
        private INotifications _notifications;

        /// <summary>
        /// Поле для хранения токена отмены
        /// </summary>
        private CancellationTokenSource _ctr;

        private ObservableCollection<LogItemBackGeo> _collectionLog = new ObservableCollection<LogItemBackGeo>();

        private bool _isStartBackGeo = false;
        private bool _isGeoCodingModelBusy;
        private bool _isStartGeoCoding = false;
        private string _timeNextStartGeo = string.Empty;
        private bool _isStartGeo = false;

        /// <summary>
        /// Поле для хранения ссылки на команду закрытия приложения
        /// </summary>
        private RelayCommand<CancelEventArgs> _commandClosing;

        /// <summary>
        /// Поле для хранения ссылки на команду остановки фоновой работы
        /// </summary>
        private RelayCommand _commandStopBackGeo;

        /// <summary>
        /// Поле для хранения ссылки на команду запуска фоновой работы
        /// </summary>
        private RelayCommand _commandStartBackGeo;

        #endregion PrivateField

        #region PublicProperties

        /// <summary>
        /// Статистика выполнения геокодирования
        /// </summary>
        public StatisticsViewModel Stat
        {
            get => _stat;
            set => Set(ref _stat, value);
        }

        /// <summary>
        /// Настройки приложения
        /// </summary>
        public AppSettings AppSettings
        {
            get => _appSettings;
            set => Set(ref _appSettings, value);
        }

        /// <summary>
        /// Модель информирования
        /// </summary>
        public INotifications Notifications
        {
            get => _notifications;
            set => Set(ref _notifications, value);
        }

        /// <summary>
        /// Коллекция логов
        /// </summary>
        public ObservableCollection<LogItemBackGeo> CollectionLog
        {
            get => _collectionLog;
            set => Set(ref _collectionLog, value);
        }

        /// <summary>
        /// Занят ли геокодер
        /// </summary>
        public bool IsGeoCodingModelBusy
        {
            get => _isGeoCodingModelBusy;
            set => Set(ref _isGeoCodingModelBusy, value);
        }

        /// <summary>
        /// Запущен ли процесс фоновой работы
        /// </summary>
        public bool IsStartBackGeo
        {
            get => _isStartBackGeo;
            set => Set(ref _isStartBackGeo, value);
        }

        /// <summary>
        /// Запущен ли процесс геокодирования
        /// </summary>
        public bool IsStartGeoCoding
        {
            get => _isStartGeoCoding;
            set => Set(ref _isStartGeoCoding, value);
        }

        /// <summary>
        /// Текст оповещения времени следующего запуска
        /// </summary>
        public string TimeNextStartGeo
        {
            get => _timeNextStartGeo;
            set => Set(ref _timeNextStartGeo, value);
        }

        /// <summary>
        /// Запущен ли процесс геокодирования
        /// </summary>
        public bool IsStartGeo
        {
            get => _isStartGeo;
            set => Set(ref _isStartGeo, value);
        }
        #endregion PublicProperties

        #region Command

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
                        else if (_isStartBackGeo)
                        {
                            if (obj != null)
                            {
                                obj.Cancel = true;
                                _notifications.Notification(NotificationType.Close, "Фоновая работа запущена. Закрытие невозможно!");
                            }
                        }
                        else
                        {
                            if (obj != null)
                            {
                                obj.Cancel = !_notifications.NotificationWithConfirmation(NotificationType.Close, "Вы уверены?");
                                SaveLog();
                            }
                        }
                    }));

        /// <summary>
        /// Команда для запуска фоновой работы
        /// </summary>
        public RelayCommand CommandStartBackGeo =>
       _commandStartBackGeo ?? (_commandStartBackGeo = new RelayCommand(
                   () =>
                   {
                       IsStartBackGeo = true;
                       StartTimers();
                   }, () => !_isGeoCodingModelBusy && !_isStartBackGeo && !_isStartGeo));

        /// <summary>
        /// Команда для остановки фоновой работы
        /// </summary>
        public RelayCommand CommandStopBackGeo =>
        _commandStopBackGeo ?? (_commandStopBackGeo = new RelayCommand(
                    () =>
                    {
                        IsStartBackGeo = false;
                        _ctr?.Cancel();
                    }));

        #endregion Command

        #region PrivateMethod

        /// <summary>
        /// Установка геокодера
        /// </summary>
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

        /// <summary>
        /// Выбор первого варианта из предложенных
        /// </summary>
        /// <param name="d">Коллекция данных</param>
        private static void SetFirstItemGeoCod(IEnumerable<EntityGeoCod> d)
        {
            foreach (var item in d)
            {
                if (item.ListGeoCod != null && item.ListGeoCod.Any() && item.MainGeoCod == null)
                {
                    item.MainGeoCod = item.ListGeoCod.FirstOrDefault();
                    item.Status = StatusType.OK;
                    item.Error = string.Empty;
                    item.DateTimeGeoCod = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Метод для сохранения файла с данными
        /// </summary>
        /// <param name="d">Коллекция данных</param>
        private void SaveData(IEnumerable<EntityGeoCod> d)
        {
            var okCount = d.Count(x => x.Status == StatusType.OK);

            if (okCount > 0)
            {
                var file = GetFileName(okCount);
                _model.SaveData((er) =>
                {
                    if (er == null)
                    {
                        AddLog($"Сохраняем {file} на фтп-сервер");
                    }
                    else
                    {
                        AddLog(er.Message);
                    }
                }, d, file, okCount > 20000 ? 20000 : 0, true, _appSettings.FTPSettings);
            }
        }

        /// <summary>
        /// Метод для сохранения файла с ошибками
        /// </summary>
        /// <param name="d">Коллекция данных</param>
        private void SaveError(IEnumerable<EntityGeoCod> d)
        {
            var errorCount = d.Count(x => x.Status == StatusType.Error);

            if (errorCount > 0)
            {
                var fileError = GetFileErrorName(errorCount);
                AddLog($"Сохраняем {fileError}");
                _model.SaveError((er) =>
                {
                    if (er == null)
                    {

                    }
                    else
                    {
                        AddLog(er.Message);
                    }
                }, d, fileError);
            }
        }

        /// <summary>
        /// Метод для получения имени файла с данными
        /// </summary>
        /// <param name="countRow">Колличесво элементов</param>
        /// <returns>Имя файла</returns>
        private string GetFileName(int countRow)
        {
            return _appSettings.FilesSettings.FolderOutput + $"\\{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}_backGeo_{countRow}.csv";
        }

        /// <summary>
        /// Метод для получения имени файла с ошибками
        /// </summary>
        /// <param name="countRow">Колличесво элементов</param>
        /// <returns>Имя файла</returns>
        private string GetFileErrorName(int countRow)
        {
            return _appSettings.FilesSettings.FolderErrors + $"\\{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}_backGeo_error_{countRow}.csv";
        }

        /// <summary>
        /// Метод для получения имени файла статистики по умолчанию
        /// </summary>
        /// <returns>Имя файла</returns>
        private string GetFileStatisticsName()
        {
            return $"{_appSettings.FilesSettings.FolderStatistics}\\{DateTime.Now.ToString("yyyy_MM_dd")}_Statistics.csv";
        }

        /// <summary>
        /// Метод для добавлнеия лога в коллекцию
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="countRow">количество обрабатываемых элементов</param>
        private void AddLog(string text, string countRow = "")
        {
            CollectionLog.Add(new LogItemBackGeo() { DateTimeLog = DateTime.Now, TextLog = text, CountRow = countRow });
        }

        #endregion PrivateMethod

        private bool _isStartTimer = false;

        private void StartTimers()
        {
            if (_isStartTimer) return;

            _isStartTimer = true;

            _ctr?.Dispose();
            _ctr = new CancellationTokenSource();
            var t = _ctr.Token;

            // Таймер геокодирования по графику
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (t.IsCancellationRequested)
                    {
                        _ctr?.Dispose();
                        return;
                    }
                    var dataNow = DateTime.Now;

                    var dateTimeStart = _appSettings.GeneralSettings.ListDayWeek.First(x => x.Day == dataNow.DayOfWeek && x.Selected);
                    if (dateTimeStart != null && dataNow.TimeOfDay < dateTimeStart.Time.TimeOfDay)
                    {
                        while (true)
                        {
                            TimeNextStartGeo = (dateTimeStart.Time.TimeOfDay - dataNow.TimeOfDay).ToString(@"hh\:mm\:ss");
                            if (t.IsCancellationRequested)
                            {
                                _ctr?.Dispose();
                                return;
                            }
                            dataNow = DateTime.Now;
                            if (dataNow.TimeOfDay > dateTimeStart.Time.TimeOfDay && dataNow.TimeOfDay < dateTimeStart.Time.AddMinutes(1).TimeOfDay)
                            {
                                StartGeo();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        TimeNextStartGeo = "Не сегодня";
                        Thread.Sleep(10000);
                    }
                }
            }, t);

            //Таймер использование остатков лимита
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (t.IsCancellationRequested)
                    {
                        _ctr?.Dispose();
                        return;
                    }
                    var dataNow = DateTime.Now;

                    if (dataNow.TimeOfDay < _timeStartSpendingAllLimits.TimeOfDay)
                    {
                        while (true)
                        {
                            if (t.IsCancellationRequested)
                            {
                                _ctr?.Dispose();
                                return;
                            }

                            if (dataNow.TimeOfDay > _timeStartSpendingAllLimits.TimeOfDay && dataNow.TimeOfDay < _timeStartSpendingAllLimits.AddMinutes(1).TimeOfDay)
                            {
                                StartSpendingAllLimits();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10000);
                    }
                }
            }, t);
        }

        private void StartSpendingAllLimits()
        {
            _isStartTimer = false;
            _ctr.Cancel();

            AddLog("Старт выбора оставшегося лимита");
            Task.Factory.StartNew(async () =>
            {
                AddLog("Получение данных из базы");

                var limit = await _geoCodingModel.GetMaxLimit();
                if (limit.Successfully && limit.Entity > 0)
                {
                    var resultGetDataDb = await _modelBd.GetDataFromBDAsync(_appSettings.BDSettings, _appSettings.GeneralSettings, limit.Entity);
                    if (resultGetDataDb.Successfully)
                    {
                        AddLog("Данные получены", resultGetDataDb.Entities.Count().ToString());
                        AddLog("Старт получения геокодов");
                        IsStartGeo = true;


                        if (resultGetDataDb.Entities.Any())
                        {
                            StartStat(resultGetDataDb.Entities);

                            _geoCodingModel.GetAllGeoCodMaxLimit((error) =>
                            {
                                _stat.Stop();
                                IsStartGeo = false;
                                if (error == null)
                                {
                                    AddLog("Геокодирование завершено успешно!");
                                }
                                else
                                {
                                    AddLog(error.Message);
                                }

                                SetFirstItemGeoCod(resultGetDataDb.Entities);

                                SaveData(resultGetDataDb.Entities);

                                SaveError(resultGetDataDb.Entities);

                                SaveStatistics();
                                StartTimers();
                            }, resultGetDataDb.Entities);
                        }
                        else
                        {
                            AddLog("Нет данных для геокодирования");
                        }
                    }
                    else
                    {
                        AddLog(resultGetDataDb.Error.Message);
                    }
                }
                else if (limit.Entity == 0)
                {
                    AddLog("Лимита нет");
                }
                else
                {
                    AddLog(limit.Error.Message);
                }
            });

            StartTimers();
        }

        private void StartGeo()
        {
            _isStartTimer = false;
            _ctr?.Cancel();

            AddLog("Старт геокодирования");
            TimeNextStartGeo = "В процессе...";
            Task.Factory.StartNew(async () =>
            {
                AddLog("Получение данных из базы");

                var limit = await _geoCodingModel.GetCurrentLimit();
                if (limit.Successfully && limit.Entity > 0)
                {
                    var resultGetDataDb = await _modelBd.GetDataFromBDAsync(_appSettings.BDSettings, _appSettings.GeneralSettings, limit.Entity);
                    if (resultGetDataDb.Successfully)
                    {
                        AddLog("Данные получены", resultGetDataDb.Entities.Count().ToString());
                        AddLog("Старт получения геокодов");
                        IsStartGeo = true;

                        if (resultGetDataDb.Entities.Any())
                        {
                            StartStat(resultGetDataDb.Entities);

                            _geoCodingModel.GetAllGeoCod((error) =>
                            {
                                _stat.Stop();
                                IsStartGeo = false;
                                if (error == null)
                                {
                                    AddLog("Геокодирование завершено успешно!");
                                }
                                else
                                {
                                    AddLog(error.Message);
                                }

                                SetFirstItemGeoCod(resultGetDataDb.Entities);

                                SaveData(resultGetDataDb.Entities);

                                SaveError(resultGetDataDb.Entities);

                                SaveStatistics();

                                StartTimers();
                            }, resultGetDataDb.Entities);
                        }
                        else
                        {
                            AddLog("Нет данных для геокодирования");
                        }
                    }
                    else
                    {
                        AddLog(resultGetDataDb.Error.Message);
                    }
                }
                else if (limit.Entity == 0)
                {
                    AddLog("Лимита нет");
                }
                else
                {
                    AddLog(limit.Error.Message);
                }

                StartTimers();
            });
        }

        private RelayCommand _commandStart;
        public RelayCommand CommandStart =>
        _commandStart ?? (_commandStart = new RelayCommand(
                    () =>
                    {
                        StartGeo();
                    }));

        private void StartStat(IEnumerable<EntityGeoCod> data)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => 
            {
                _stat.Init(data);
                _stat.Start(_appSettings.GeoCodSettings.CurrentGeoCoder.GeoCoder);
            });
        }

        /// <summary>
        /// Метод для сохранения статистики
        /// </summary>
        private void SaveStatistics()
        {
            if (_stat.IsSave)
            {
                return;
            }

            var nameFile = GetFileStatisticsName();
            _model.SaveStatistics(e =>
            {
                _notifications.Notification(NotificationType.Error, e);
                if (e == null)
                {
                    _stat.IsSave = true;
                }
            }, _stat.Statistics, _appSettings.FilesSettings, nameFile);
        }

        private void SaveLog()
        {
            _model.SaveLog(_collectionLog);
        }
    }
}