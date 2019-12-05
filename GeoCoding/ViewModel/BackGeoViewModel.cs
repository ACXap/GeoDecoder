// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            AppSettings = new AppSettings(_notifications);

            _geoCodingModel = new GeoCodingModel(_appSettings.NetSettings, _appSettings.GeoCodSettings, _appSettings.GetLimitsModel());
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