using MahApps.Metro.Controls.Dialogs;
using System;

namespace GeoCoding
{
    public class NotificationsModel : INotifications
    {

        #region PrivateField
        ///// <summary>
        ///// Заголовок оповещения с ошибками
        ///// </summary>
        private const string _headerError = "Ошибка";

        private const string _bodyError = "Процесс завершен с ошибкой";
        /// <summary>
        /// Заголовок оповещения по завершению обработки данных
        /// </summary>
        private const string _headerOk = "Успех";
        private const string _bodyOk = "Процесс завершен успешно";
        /// <summary>
        /// Заголовок оповещения с отмененной операцией
        /// </summary>
        private const string _headerCancel = "Отмена";
        private const string _bodyCancel = "Операция была отменена пользователем";
        ///// <summary>
        ///// Заголовок оповещения при успешном сохранении настроек
        ///// </summary>
        private const string _headerSaveSettings = "Сохранение настроек";
        private const string _bodySaveSettings = "Настройки успешно сохранены в файле";
        /// <summary>
        /// Заголовок оповещения по завершению записи данных
        /// </summary>
        private const string _headerSaveData = "Сохранение данных";
        private const string _bodySaveData = "Данные успешно сохранены в файле";
        /// <summary>
        /// Заголовок оповещения при пустой коллекции для обработки
        /// </summary>
        private const string _headerDataEmpty = "Данных нет";
        private const string _bodyDataEmpty = "Для обработки нет данных";
        /// <summary>
        /// Заголовок оповещения при повторном сохранении статистики
        /// </summary>
        private const string _headerStatAlreadySave = "Статистика уже сохранена";
        private const string _bodyStatAlreadySave = "С последнего сохранения статистики, в ней ничего не изменилось";
        #endregion PrivateField

        #region PublicProperties
        #endregion PublicProperties

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod
        #endregion PublicMethod

        /// <summary>
        /// Поле для хранения ссылки на координатора диалогов
        /// </summary>
        private readonly IDialogCoordinator dialogCoordinator = DialogCoordinator.Instance;
        /// <summary>
        /// Поле для хранения ссылки на настройки оповещений
        /// </summary>
        private readonly NotificationSettings notificationSettings;

        public NotificationsModel(NotificationSettings settings)
        {
            notificationSettings = settings;
        }

        public async void Notification(string header, string body)
        {
            await dialogCoordinator.ShowMessageAsync(this, header, body);
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
                    if (notificationSettings.CanNotificationProcessCancel)
                    {
                        Notification(_headerCancel, _bodyCancel);
                    }
                    break;
                case NotificationType.SettingsSave:
                    if (notificationSettings.CanNotificationSaveSettings)
                    {
                        Notification(_headerSaveSettings, _bodySaveSettings);
                    }
                    break;
                case NotificationType.SaveData:
                    if (notificationSettings.CanNotificationSaveData)
                    {
                        Notification(_headerSaveData, _bodySaveData);
                    }
                    break;
                case NotificationType.StatAlreadySave:
                    if (notificationSettings.CanNotificationStatAlreadySave)
                    {
                        Notification(_headerStatAlreadySave, _bodyStatAlreadySave);
                    }
                    break;
                case NotificationType.DataProcessed:
                    if (notificationSettings.CanNotificationDataProcessed)
                    {
                        Notification(_headerOk, _bodyOk);
                    }
                    break;
                case NotificationType.DataEmpty:
                    if (notificationSettings.CanNotificationDataEmpty)
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
                    if (!notificationSettings.CanNotificationProcessCancel)
                    {
                        return;
                    }
                    break;
                case NotificationType.SettingsSave:
                    if (!notificationSettings.CanNotificationSaveSettings)
                    {
                        return;
                    }
                    break;
                case NotificationType.SaveData:
                    if (!notificationSettings.CanNotificationSaveData)
                    {
                        return;
                    }
                    break;
                case NotificationType.StatAlreadySave:
                    if (!notificationSettings.CanNotificationStatAlreadySave)
                    {
                        return;
                    }
                    break;
                case NotificationType.DataProcessed:
                    if (!notificationSettings.CanNotificationDataProcessed)
                    {
                        return;
                    }
                    break;
                case NotificationType.DataEmpty:
                    if (!notificationSettings.CanNotificationDataEmpty)
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
                    if (!notificationSettings.CanNotificationProcessCancel)
                    {
                        return;
                    }
                    header = _headerCancel;
                    break;
                case NotificationType.SettingsSave:
                    if (!notificationSettings.CanNotificationSaveSettings)
                    {
                        return;
                    }
                    header = _headerSaveSettings;
                    break;
                case NotificationType.SaveData:
                    if (!notificationSettings.CanNotificationSaveData)
                    {
                        return;
                    }
                    header = _headerSaveData;
                    break;
                case NotificationType.StatAlreadySave:
                    if (!notificationSettings.CanNotificationStatAlreadySave)
                    {
                        return;
                    }
                    header = _headerStatAlreadySave;
                    break;
                case NotificationType.DataProcessed:
                    if (!notificationSettings.CanNotificationDataProcessed)
                    {
                        return;
                    }
                    header = _headerOk;
                    break;
                case NotificationType.DataEmpty:
                    if (!notificationSettings.CanNotificationDataEmpty)
                    {
                        return;
                    }
                    header = _headerDataEmpty;
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
                if(canNotificationOnErrorNull)
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
    }
}