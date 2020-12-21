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
            _mailModel = new MailModel(_appSettings.NotificationSettings);

            SetGeoService();

            _modelVerification = new VerificationModel(_appSettings.VerificationSettings.VerificationServer);

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

        private readonly MailModel _mailModel;

        private readonly DateTime _timeStartSpendingAllLimits = new DateTime(1, 1, 1, 3, 12, 0);

        private CancellationTokenSource _ctsTimerAllLimits;
        private CancellationTokenSource _ctsTimerLimits;

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
        /// Поле для хранения ссылки на модельпроверки координат
        /// </summary>
        private readonly VerificationModel _modelVerification;

        /// <summary>
        /// Поле для хранения ссылки на модель информирования
        /// </summary>
        private INotifications _notifications;

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
                   }, () => !_isGeoCodingModelBusy && !_isStartBackGeo && !_isStartGeo && _ctsTimerAllLimits == null && _ctsTimerLimits == null));

        /// <summary>
        /// Команда для остановки фоновой работы
        /// </summary>
        public RelayCommand CommandStopBackGeo =>
        _commandStopBackGeo ?? (_commandStopBackGeo = new RelayCommand(
                    () =>
                    {
                        IsStartBackGeo = false;
                        //_isStartTimer = false;

                        _ctsTimerAllLimits?.Cancel();
                        _ctsTimerLimits?.Cancel();
                    }));

        #endregion Command

        #region PrivateMethod

        /// <summary>
        /// Метод для запуска таймеров 
        /// </summary>
        private void StartTimers()
        {
            _ctsTimerAllLimits = new CancellationTokenSource();
            _ctsTimerLimits = new CancellationTokenSource();

            // Таймер геокодирования по графику
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (!_ctsTimerLimits.IsCancellationRequested)
                    {
                        System.Diagnostics.Debug.WriteLine("Таймер геокодинга по графику - 1 цикл");
                        var dataNow = DateTime.Now;

                        var dateTimeStart = _appSettings.GeneralSettings.ListDayWeek.FirstOrDefault(x => x.Day == dataNow.DayOfWeek && x.Selected);
                        if (dateTimeStart != null && dataNow.TimeOfDay < dateTimeStart.Time.TimeOfDay)
                        {
                            while (!_ctsTimerLimits.IsCancellationRequested)
                            {
                                System.Diagnostics.Debug.WriteLine("Таймер геокодинга по графику - 2 цикл");
                                TimeNextStartGeo = (dateTimeStart.Time.TimeOfDay - dataNow.TimeOfDay).ToString(@"hh\:mm\:ss");
                                dataNow = DateTime.Now;
                                if (dataNow.TimeOfDay > dateTimeStart.Time.TimeOfDay && dataNow.TimeOfDay < dateTimeStart.Time.AddMinutes(1).TimeOfDay)
                                {
                                    StartGeo();
                                }
                                await Task.Delay(1000, _ctsTimerLimits.Token);
                            }
                        }
                        else
                        {
                            TimeNextStartGeo = "Не сегодня";
                            await Task.Delay(60000, _ctsTimerLimits.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddLog("Таймер геокодинга по расписанию " + ex.Message);
                }

                _ctsTimerLimits.Dispose();
                _ctsTimerLimits = null;

            }, _ctsTimerLimits.Token);

            //Таймер использование остатков лимита
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (!_ctsTimerAllLimits.IsCancellationRequested)
                    {
                        System.Diagnostics.Debug.WriteLine("Таймер геокодинга по ночам - 1 цикл");
                        var dataNow = DateTime.Now;

                        if (dataNow.TimeOfDay < _timeStartSpendingAllLimits.TimeOfDay)
                        {
                            while (!_ctsTimerAllLimits.IsCancellationRequested)
                            {
                                System.Diagnostics.Debug.WriteLine("Таймер геокодинга по ночам - 2 цикл");
                                dataNow = DateTime.Now;
                                if (dataNow.TimeOfDay > _timeStartSpendingAllLimits.TimeOfDay && dataNow.TimeOfDay < _timeStartSpendingAllLimits.AddMinutes(1).TimeOfDay)
                                {
                                    StartSpendingAllLimits();
                                }
                                await Task.Delay(1000, _ctsTimerAllLimits.Token);
                            }
                        }
                        else
                        {
                            await Task.Delay(60000, _ctsTimerAllLimits.Token);
                        }
                    }

                }
                catch (Exception ex)
                {
                    AddLog("Таймер геокодинга по ночам " + ex.Message);
                }

                _ctsTimerAllLimits.Dispose();
                _ctsTimerAllLimits = null;

            }, _ctsTimerAllLimits.Token);
        }

        /// <summary>
        /// Метод для запуска подсчета статистики
        /// </summary>
        /// <param name="data">Коллекция для статистики</param>
        private void StartStat(IEnumerable<EntityGeoCod> data)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _stat.Init(data);
                _stat.Start(_appSettings.GeoCodSettings.CurrentGeoCoder.GeoCoder, _geoCodingModel.GetKeyShort());
            });
        }

        /// <summary>
        /// Метод для формирования почтового сообщения 
        /// </summary>
        /// <param name="entities">Коллекция результатов</param>
        /// <returns></returns>
        private string GetMessageFinish(IEnumerable<EntityGeoCod> entities)
        {
            var result = string.Empty;

            if (entities != null && entities.Any())
            {
                result = "Добрый день!";

                result += $"<br>Было обработано адресов: {entities.Count()}";
                result += $"<br>Хороших координат: {entities.Count(x => x.MainGeoCod?.Qcode == 1)}";

                var er = entities.Where(x => x.MainGeoCod == null);
                if (er.Any())
                {
                    result += $"<br>Требуется ручная обработка: {er.Count()}<br>";
                    foreach (var a in er)
                    {
                        result += $"<br>{a.GlobalID}";
                    }
                }

                result += "<br><br><b>Это сообщение сформировано автоматически. Отвечать на него не надо.</b>";
            }

            return result;
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
        private void SetFirstItemGeoCod(IEnumerable<EntityGeoCod> d)
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

            _stat.UpdateStatisticsCollection();
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

                try
                {
                    _modelBd.SaveData(_appSettings.BDSettings, d);
                } catch(Exception ex)
                {
                    string message = "Не удалось записать данные в базу. Ошибка: " + ex.Message;
                    AddLog(message);
                    _mailModel.SendEmailGeoFinish((m) =>
                    {
                        AddLog(m);
                    }, message, Array.Empty<string>());
                }
             }
        }

        /// <summary>
        /// Метод для сохранения файла с ошибками
        /// </summary>
        /// <param name="d">Коллекция данных</param>
        private string SaveError(IEnumerable<EntityGeoCod> d)
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

                return fileError;
            }

            return null;
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
        /// Метод для добавлнеия лога в коллекцию
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="countRow">количество обрабатываемых элементов</param>
        private void AddLog(string text, int? countRow = null)
        {
            lock (_lockLog)
            {
                try
                {
                    var l = new LogItemBackGeo() { DateTimeLog = DateTime.Now, TextLog = text, CountRow = countRow };
                    CollectionLog.Add(l);
                    _model.SaveLog(l);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private object _lockLog = new object();

        #endregion PrivateMethod

        private void StartSpendingAllLimits()
        {
            _ctsTimerAllLimits?.Cancel();
            _ctsTimerLimits?.Cancel();

            AddLog("Старт выбора оставшегося лимита");
            TimeNextStartGeo = "В процессе...";

            Task.Factory.StartNew(async () =>
            {
                AddLog("Запрос остатков лимита");
                var limit = await _geoCodingModel.GetMaxLimit();

                if (!limit.Successfully)
                {
                    await Task.Delay(10000);
                    AddLog("Повторная попытка получения лимита");
                    limit = await _geoCodingModel.GetMaxLimit();
                }

                if (limit.Successfully && limit.Entity > 0)
                {
                    AddLog("Оставшийся лимит", limit.Entity);

                    AddLog("Получение данных из базы");
                    var resultGetDataDb = await _modelBd.GetDataFromBDAsync(_appSettings.BDSettings, _appSettings.GeneralSettings, limit.Entity);

                    // Если не получилось 1 раз попробовать еще раз
                    if (!resultGetDataDb.Successfully)
                    {
                        AddLog("Повторная попытка получения данных из базы");
                        resultGetDataDb = await _modelBd.GetDataFromBDAsync(_appSettings.BDSettings, _appSettings.GeneralSettings, limit.Entity);
                    }

                    if (resultGetDataDb.Successfully)
                    {
                        AddLog("Данные получены", resultGetDataDb.Entities.Count());
                        AddLog("Старт получения геокодов");
                        IsStartGeo = true;

                        if (resultGetDataDb.Entities.Any())
                        {
                            StartStat(resultGetDataDb.Entities);

                            _geoCodingModel.GetAllGeoCodMaxLimit((error) =>
                            {
                                
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

                                AddLog("Всего хороших координат", resultGetDataDb.Entities.Count(x => x.MainGeoCod != null && x.MainGeoCod.Qcode == 1));

                                AddLog("Проверка координат");

                                _modelVerification.Check((e) =>
                                {
                                    if (e == null)
                                    {
                                        AddLog("Проверка координат успешна");
                                    }
                                    else
                                    {
                                        AddLog("Проверка координат завершено с ошибкой: " + e.Message);
                                    }

                                    AddLog("Всего хороших координат после проверки", resultGetDataDb.Entities.Count(x => x.MainGeoCod != null && x.MainGeoCod.Qcode == 1));

                                    _stat.Stop();
                                    SaveData(resultGetDataDb.Entities);

                                    _model.SaveTemp((er) =>
                                    {

                                    }, resultGetDataDb.Entities, _appSettings.FilesSettings.FolderTemp + $"\\{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}_afterCheck.csv");

                                    var fileError = SaveError(resultGetDataDb.Entities);

                                    SaveStatistics();

                                    _mailModel.SendEmailGeoFinish((m) =>
                                    {
                                        AddLog(m);
                                    }, GetMessageFinish(resultGetDataDb.Entities), new string[] { fileError });
                                }, resultGetDataDb.Entities);

                                StartTimers();
                            }, resultGetDataDb.Entities);
                        }
                        else
                        {
                            AddLog("Нет данных для геокодирования");
                            StartTimers();
                        }
                    }
                    else
                    {
                        AddLog(resultGetDataDb.Error.Message);
                        StartTimers();
                    }
                }
                else if (limit.Entity < 1 && limit.Error == null)
                {
                    AddLog("Лимита нет");
                    StartTimers();
                }
                else
                {
                    AddLog(limit.Error.Message);
                    StartTimers();
                }
            });
        }

        private void StartGeo()
        {
            _ctsTimerAllLimits?.Cancel();
            _ctsTimerLimits?.Cancel();

            AddLog("Старт геокодирования по графику");
            TimeNextStartGeo = "В процессе...";
            
            Task.Factory.StartNew(async () =>
            {
                AddLog("Запрос остатков лимита");
                var limit = await _geoCodingModel.GetCurrentLimit();

                if (!limit.Successfully)
                {
                    await Task.Delay(10000);
                    AddLog("Повторная попытка получения лимита");
                    limit = await _geoCodingModel.GetMaxLimit();
                }

                if (limit.Successfully && limit.Entity > 0)
                {
                    AddLog("Оставшийся лимит", limit.Entity);

                    AddLog("Получение данных из базы");
                    var resultGetDataDb = await _modelBd.GetDataFromBDAsync(_appSettings.BDSettings, _appSettings.GeneralSettings, limit.Entity);

                    // Если не получилось 1 раз попробовать еще раз
                    if (!resultGetDataDb.Successfully)
                    {
                        AddLog(resultGetDataDb.Error.Message);
                        AddLog("Повторная попытка получения данных из базы");
                        resultGetDataDb = await _modelBd.GetDataFromBDAsync(_appSettings.BDSettings, _appSettings.GeneralSettings, limit.Entity);
                    }

                    if (resultGetDataDb.Successfully)
                    {
                        AddLog("Данные получены", resultGetDataDb.Entities.Count());
                        AddLog("Старт получения геокодов");
                        IsStartGeo = true;

                        if (resultGetDataDb.Entities.Any())
                        {
                            StartStat(resultGetDataDb.Entities);

                            _geoCodingModel.GetAllGeoCod((error) =>
                            {
                                
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

                                AddLog("Всего хороших координат", resultGetDataDb.Entities.Count(x => x.MainGeoCod != null && x.MainGeoCod.Qcode == 1));

                                AddLog("Проверка координат");

                                _modelVerification.Check((e) =>
                                {
                                    if (e == null)
                                    {
                                        AddLog("Проверка координат успешна");
                                    }
                                    else
                                    {
                                        AddLog("Проверка координат завершено с ошибкой: " + e.Message);
                                    }

                                    AddLog("Всего хороших координат после проверки", resultGetDataDb.Entities.Count(x => x.MainGeoCod != null && x.MainGeoCod.Qcode == 1));

                                    _stat.Stop();
                                    SaveData(resultGetDataDb.Entities);

                                    _model.SaveTemp((er) =>
                                    {

                                    }, resultGetDataDb.Entities, _appSettings.FilesSettings.FolderTemp + $"\\{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}_afterCheck.csv");

                                    var fileError = SaveError(resultGetDataDb.Entities);

                                    SaveStatistics();

                                    _mailModel.SendEmailGeoFinish((m) =>
                                    {
                                        AddLog(m);
                                    }, GetMessageFinish(resultGetDataDb.Entities), new string[] { fileError });

                                }, resultGetDataDb.Entities);

                                StartTimers();
                            }, resultGetDataDb.Entities);
                        }
                        else
                        {
                            AddLog("Нет данных для геокодирования");
                            StartTimers();
                        }
                    }
                    else
                    {
                        AddLog(resultGetDataDb.Error.Message);
                        StartTimers();
                    }
                }
                else if (limit.Entity < 1 && limit.Error == null)
                {
                    AddLog("Лимита нет");
                    StartTimers();
                }
                else
                {
                    AddLog(limit.Error.Message);
                    StartTimers();
                }
            });
        }
    }
}