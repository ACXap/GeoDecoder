using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GeoCoding.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding
{
    public class BackGeoViewModel : ViewModelBase
    {
        public BackGeoViewModel()
        {
            _model = new MainWindowModel();
            Notifications = new NotificationsModel();

            AppSettings = new AppSettings(_model, _notifications);

            _notifications.SetSettings(_appSettings.NotificationSettings);
            _geoCodingModel = new GeoCodingModel(_appSettings.NetSettings, _appSettings.GeoCodSettings, null);
        }

        #region PrivateField

        /// <summary>
        /// Настройки приложения
        /// </summary>
        private AppSettings _appSettings;

        /// <summary>
        /// Поле для хранения ссылки на модель
        /// </summary>
        private readonly MainWindowModel _model;

        /// <summary>
        /// Поле для хранения ссылки на модельгеокодирования
        /// </summary>
        private readonly GeoCodingModel _geoCodingModel;

        /// <summary>
        /// Поле для хранения ссылки на модель информирования
        /// </summary>
        private INotifications _notifications;

        /// <summary>
        /// Поле для хранения ссылки на команду закрытия приложения
        /// </summary>
        private RelayCommand<CancelEventArgs> _commandClosing;

        #endregion PrivateField

        #region PublicProperties
       
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
        #endregion PublicProperties

        #region Command

        /// <summary>
        /// Команда для обработки закрытия программы
        /// </summary>
        public RelayCommand<CancelEventArgs> CommandClosing =>
        _commandClosing ?? (_commandClosing = new RelayCommand<CancelEventArgs>(
                    obj =>
                    {
                        //if (_isStartGeoCoding)
                        //{
                        //    if (obj != null)
                        //    {
                        //        obj.Cancel = true;
                        //        _notifications.Notification(NotificationType.Close, "Идет процесс геокодирования. Закрытие невозможно!");
                        //    }
                        //}
                        //else
                        //{
                        //    if (obj != null)
                        //    {
                        //        obj.Cancel = !_notifications.NotificationWithConfirmation(NotificationType.Close, "Вы уверены?");
                        //    }
                        //}
                    }));
        
        #endregion Command

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod
        #endregion PublicMethod
    }
}



//private bool _isStartBackGeo = false;
///// <summary>
///// Запущена ли фоновая работа геокодера
///// </summary>
//public bool IsStartBackGeo
//{
//    get => _isStartBackGeo;
//    set => Set(ref _isStartBackGeo, value);
//}

//private bool _start = false;
///// <summary>
///// 
///// </summary>
//public bool Start
//{
//    get => _start;
//    set => Set(ref _start, value);
//}

//private RelayCommand _commandSaveSettingsBackGeo;
//public RelayCommand CommandSaveSettingsBackGeo =>
//_commandSaveSettingsBackGeo ?? (_commandSaveSettingsBackGeo = new RelayCommand(
//            () =>
//            {
//                _appSettings.SaveGeneralSettings();
//            }));

//private RelayCommand _stopBackGeo;
//public RelayCommand StopBackGeo =>
//_stopBackGeo ?? (_stopBackGeo = new RelayCommand(
//            () =>
//            {
//                IsStartBackGeo = false;
//            }));

//private RelayCommand _startBackGeo;
//public RelayCommand StartBackGeo =>
//_startBackGeo ?? (_startBackGeo = new RelayCommand(
//            () =>
//            {
//                IsStartBackGeo = true;
//                Task.Factory.StartNew(() =>
//                {
//                    while (IsStartBackGeo)
//                    {
//                        CountLastTime();

//                        var date = DateTime.Now;
//                        var day = date.DayOfWeek;
//                        var s = _appSettings.GeneralSettings.ListDayWeek.Where(x => x.Day == day && x.Selected);
//                        if (s != null && date.Date != LastDayStartGeo.Date)
//                        {
//                            var t = (s.First().Time.TimeOfDay - date.TimeOfDay);
//                            if (t.TotalSeconds < 0)
//                            {
//                                BackGeoProcessStart();
//                            }
//                        }

//                        Thread.Sleep(1000);
//                    }
//                });
//            }));


//private ObservableCollection<EntityGeoCod> _collectionBackGeo;
///// <summary>
///// 
///// </summary>
//public ObservableCollection<EntityGeoCod> CollectionBackGeo
//{
//    get => _collectionBackGeo;
//    set => Set(ref _collectionBackGeo, value);
//}

//private void BackGeoProcessStart()
//{
//    Task.Factory.StartNew(() =>
//    {
//        LastDayStartGeo = DateTime.Now;
//        Start = true;

//        _model.GetDataFromBDAsync((data, e) =>
//        {
//            if (e == null)
//            {
//                if (data != null && data.Any())
//                {
//                    CollectionBackGeo = new ObservableCollection<EntityGeoCod>(data);
//                    _geoCodingModel.GetAllGeoCod((er) =>
//                    {
//                        if (er == null)
//                        {
//                            var a = data;
//                            var file = $"{_appSettings.FilesSettings.FolderInput}\\{DateTime.Now.ToString("yyyy_MM_dd")}_back_{CollectionBackGeo.Count}.csv";
//                            _model.SaveData((error) =>
//                            {
//                                if (error != null)
//                                {
//                                    _notifications.Notification(NotificationType.Error, error.Message);
//                                }
//                            }, a, file, 0, true, _appSettings.FTPSettings);

//                            var dataError = a.Where(x => x.Status == StatusType.Error);
//                            file = $"{_appSettings.FilesSettings.FolderErrors}\\{DateTime.Now.ToString("yyyy_MM_dd")}_back_error_{dataError.Count()}.csv";
//                            _model.SaveError((error) =>
//                            {

//                            }, dataError, file);
//                        }
//                        else
//                        {
//                            _notifications.Notification(NotificationType.Error, er.Message);
//                        }
//                    }, data);
//                }
//            }
//            else
//            {
//                _notifications.Notification(NotificationType.Error, e.Message);
//            }
//        }, _appSettings.BDSettings, _appSettings.GeneralSettings.ScpriptBackgroundGeo);

//        Work += Environment.NewLine + DateTime.Now.ToString();
//        //Thread.Sleep(5000);
//    });
//}

//private void CountLastTime()
//{
//    NextStartGeo = "Не сегодня!";
//    var date = DateTime.Now;
//    var day = date.DayOfWeek;
//    var s = _appSettings.GeneralSettings.ListDayWeek.Where(x => x.Day >= day && x.Selected);
//    if (s != null && s.Any())
//    {
//        var t = (s.First().Time.TimeOfDay - date.TimeOfDay);
//        if (t.TotalSeconds > 0)
//        {
//            NextStartGeo = t.ToString(@"hh\:mm\:ss");
//        }
//    }
//}

//private string _work = "Тут будет запуск:";
///// <summary>
///// 
///// </summary>
//public string Work
//{
//    get => _work;
//    set => Set(ref _work, value);
//}

//private DateTime _lastDayStartGeo = new DateTime(2019, 11, 19, 15, 0, 0);
///// <summary>
///// 
///// </summary>
//public DateTime LastDayStartGeo
//{
//    get => _lastDayStartGeo;
//    set => Set(ref _lastDayStartGeo, value);
//}

//private string _nextStartGeo;
///// <summary>
///// Время до следующего старта геокодирования
///// </summary>
//public string NextStartGeo
//{
//    get => _nextStartGeo;
//    set => Set(ref _nextStartGeo, value);
//}