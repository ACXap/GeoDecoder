using MahApps.Metro.Controls.Dialogs;
using System;

namespace GeoCoding
{
    /// <summary>
    /// Класс для отображения оповещений
    /// </summary>
    public class NotificationsModel : INotifications
    {
        #region PrivateConst
        /// <summary>
        /// Заголовок оповещения с ошибками
        /// </summary>
        private const string _headerError = "Ошибка";
        /// <summary>
        /// Текст оповещения об ошибке в работе
        /// </summary>
        private const string _bodyError = "Процесс завершен с ошибкой";
        /// <summary>
        /// Заголовок оповещения по завершению обработки данных
        /// </summary>
        private const string _headerOk = "Успех";
        /// <summary>
        /// Текст оповещения об успешности работы
        /// </summary>
        private const string _bodyOk = "Процесс завершен успешно";
        /// <summary>
        /// Заголовок оповещения с отмененной операцией
        /// </summary>
        private const string _headerCancel = "Отмена";
        /// <summary>
        /// Текст оповещения об отмене работы пользователем
        /// </summary>
        private const string _bodyCancel = "Операция была отменена пользователем";
        ///// <summary>
        ///// Заголовок оповещения при успешном сохранении настроек
        ///// </summary>
        private const string _headerSaveSettings = "Сохранение настроек";
        /// <summary>
        /// Текст оповещения об успешности записи настроек в файл
        /// </summary>
        private const string _bodySaveSettings = "Настройки успешно сохранены в файле";
        /// <summary>
        /// Заголовок оповещения по завершению записи данных
        /// </summary>
        private const string _headerSaveData = "Сохранение данных";
        /// <summary>
        /// Заголовок оповещения закрытия программы
        /// </summary>
        private const string _headerClose = "Закрытие программы";
        /// <summary>
        /// Текст оповещения об успешности записи в файл
        /// </summary>
        private const string _bodySaveData = "Данные успешно сохранены в файле";
        /// <summary>
        /// Заголовок оповещения при пустой коллекции для обработки
        /// </summary>
        private const string _headerDataEmpty = "Данных нет";
        /// <summary>
        /// Текст оповещения об отсутствии данных в коллекции
        /// </summary>
        private const string _bodyDataEmpty = "Для обработки нет данных";
        /// <summary>
        /// Заголовок оповещения при повторном сохранении статистики
        /// </summary>
        private const string _headerStatAlreadySave = "Статистика уже сохранена";
        /// <summary>
        /// Текст сообщения при попытке сохранить статистику которая уже сохранена
        /// </summary>
        private const string _bodyStatAlreadySave = "С последнего сохранения статистики, в ней ничего не изменилось";
        #endregion PrivateConst

        #region PrivateField
        /// <summary>
        /// Поле для хранения ссылки на координатора диалогов
        /// </summary>
        private readonly IDialogCoordinator _dialogCoordinator = DialogCoordinator.Instance;
        /// <summary>
        /// Поле для хранения ссылки на настройки оповещений
        /// </summary>
        private readonly NotificationSettings _notificationSettings;
        #endregion PrivateField

        #region PublicMethod
        public async void Notification(string header, string body)
        {
            await _dialogCoordinator.ShowMessageAsync(this, header, body);
        }

        public void Notification(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Error:
                    Notification(_headerError, _bodyError);
                    break;
                case NotificationType.Ok:
                    Notification(_headerOk, _bodyOk);
                    break;
                case NotificationType.Cancel:
                    if (_notificationSettings.CanNotificationProcessCancel)
                    {
                        Notification(_headerCancel, _bodyCancel);
                    }
                    break;
                case NotificationType.SettingsSave:
                    if (_notificationSettings.CanNotificationSaveSettings)
                    {
                        Notification(_headerSaveSettings, _bodySaveSettings);
                    }
                    break;
                case NotificationType.SaveData:
                    if (_notificationSettings.CanNotificationSaveData)
                    {
                        Notification(_headerSaveData, _bodySaveData);
                    }
                    break;
                case NotificationType.StatAlreadySave:
                    if (_notificationSettings.CanNotificationStatAlreadySave)
                    {
                        Notification(_headerStatAlreadySave, _bodyStatAlreadySave);
                    }
                    break;
                case NotificationType.DataProcessed:
                    if (_notificationSettings.CanNotificationDataProcessed)
                    {
                        Notification(_headerOk, _bodyOk);
                    }
                    break;
                case NotificationType.DataEmpty:
                    if (_notificationSettings.CanNotificationDataEmpty)
                    {
                        Notification(_headerDataEmpty, _bodyDataEmpty);
                    }
                    break;
                default:
                    break;
            }
        }

        public void Notification(NotificationType notificationType, string header, string body)
        {
            switch (notificationType)
            {
                case NotificationType.Error:
                    break;
                case NotificationType.Ok:
                    break;
                case NotificationType.Cancel:
                    if (!_notificationSettings.CanNotificationProcessCancel)
                    {
                        return;
                    }
                    break;
                case NotificationType.SettingsSave:
                    if (!_notificationSettings.CanNotificationSaveSettings)
                    {
                        return;
                    }
                    break;
                case NotificationType.SaveData:
                    if (!_notificationSettings.CanNotificationSaveData)
                    {
                        return;
                    }
                    break;
                case NotificationType.StatAlreadySave:
                    if (!_notificationSettings.CanNotificationStatAlreadySave)
                    {
                        return;
                    }
                    break;
                case NotificationType.DataProcessed:
                    if (!_notificationSettings.CanNotificationDataProcessed)
                    {
                        return;
                    }
                    break;
                case NotificationType.DataEmpty:
                    if (!_notificationSettings.CanNotificationDataEmpty)
                    {
                        return;
                    }
                    break;
                default:
                    break;
            }

            Notification(header, body);
        }

        public void Notification(NotificationType notificationType, string body)
        {
            string header = string.Empty;

            switch (notificationType)
            {
                case NotificationType.Error:
                    header = _headerError;
                    break;
                case NotificationType.Ok:
                    header = _headerOk;
                    break;
                case NotificationType.Cancel:
                    if (!_notificationSettings.CanNotificationProcessCancel)
                    {
                        return;
                    }
                    header = _headerCancel;
                    break;
                case NotificationType.SettingsSave:
                    if (!_notificationSettings.CanNotificationSaveSettings)
                    {
                        return;
                    }
                    header = _headerSaveSettings;
                    break;
                case NotificationType.SaveData:
                    if (!_notificationSettings.CanNotificationSaveData)
                    {
                        return;
                    }
                    header = _headerSaveData;
                    break;
                case NotificationType.StatAlreadySave:
                    if (!_notificationSettings.CanNotificationStatAlreadySave)
                    {
                        return;
                    }
                    header = _headerStatAlreadySave;
                    break;
                case NotificationType.DataProcessed:
                    if (!_notificationSettings.CanNotificationDataProcessed)
                    {
                        return;
                    }
                    header = _headerOk;
                    break;
                case NotificationType.DataEmpty:
                    if (!_notificationSettings.CanNotificationDataEmpty)
                    {
                        return;
                    }
                    header = _headerDataEmpty;
                    break;
                case NotificationType.Close:
                    header = _headerClose;
                    break;
                default:
                    break;
            }

            Notification(header, body);
        }

        public void Notification(NotificationType notificationType, Exception error, bool canNotificationOnErrorNull)
        {
            if (error != null)
            {
                Notification(notificationType, error.Message);
            }
            else
            {
                if (canNotificationOnErrorNull)
                {
                    Notification(notificationType);
                }
            }
        }

        public void Notification(NotificationType notificationType, string body, Exception error)
        {
            if (error != null)
            {
                Notification(notificationType, error.Message);
            }
            else
            {
                Notification(notificationType, body);
            }
        }

        public bool NotificationWithConfirmation(NotificationType notificationType, string body)
        {
            string header = string.Empty;

            switch (notificationType)
            {
                case NotificationType.Error:
                    break;
                case NotificationType.Ok:
                    break;
                case NotificationType.Cancel:
                    break;
                case NotificationType.SettingsSave:
                    break;
                case NotificationType.SaveData:
                    break;
                case NotificationType.StatAlreadySave:
                    break;
                case NotificationType.DataProcessed:
                    break;
                case NotificationType.DataEmpty:
                    break;
                case NotificationType.Close:
                    if(!_notificationSettings.CanNotificationExit)
                    {
                        return true;
                    }
                    header = _headerClose;
                    break;
                default:
                    break;
            }

            return NotificationWithConfirmation(header, body);
        }

        public bool NotificationWithConfirmation(string header, string body)
        {
            var a = _dialogCoordinator.ShowModalMessageExternal(this, header, body, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет"
            });
            if (a == MessageDialogResult.Affirmative)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion PublicMethod

        public NotificationsModel(NotificationSettings settings)
        {
            _notificationSettings = settings;
        }
    }
}