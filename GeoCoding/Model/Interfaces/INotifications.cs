using System;

namespace GeoCoding
{
    public interface INotifications
    {
        /// <summary>
        /// Простое оповещение без вида работы и без возможности управлять
        /// </summary>
        /// <param name="header">Заголовок оповещения</param>
        /// <param name="body">Сообщение оповещения</param>
        void Notification(string header, string body);
        /// <summary>
        /// Оповещение по виду работы, с заголовком и сообщением по умолчанию
        /// </summary>
        /// <param name="notificationType">Вид работы</param>
        void Notification(NotificationType notificationType);
        /// <summary>
        /// Оповещение по виду работы, с заголовком и текстом сообщения, с возможностью управлять
        /// </summary>
        /// <param name="notificationType">Вид работы</param>
        /// <param name="header">Заголовок</param>
        /// <param name="body">Сообщение</param>
        void Notification(NotificationType notificationType, string header, string body);
        /// <summary>
        /// Оповещение по виду работы, с текстом сообщения, с возможностью управлять
        /// </summary>
        /// <param name="notificationType">Вид работы</param>
        /// <param name="body">Сообщение</param>
        void Notification(NotificationType notificationType, string body);
        /// <summary>
        /// Оповещение по виду работы, с ошибкой выполнения, при error==null, сообщение об успехе если разрешено, иначе об ошибке
        /// </summary>
        /// <param name="notificationType">Вид работы</param>
        /// <param name="error">Ошибка</param>
        /// <param name="canNotificationOnErrorNull">Оповещать об успехе, если ошибка null</param>
        void Notification(NotificationType notificationType, Exception error, bool canNotificationOnErrorNull = false);
        /// <summary>
        /// Оповещение по виду работы, с ошибкой выполнения, и текстом сообщения, при error==null, сообщение об успехе, иначе об ошибке
        /// </summary>
        /// <param name="notificationType">Вид работы</param>
        /// <param name="body">Сообщение</param>
        /// <param name="error">Ошибка</param>
        void Notification(NotificationType notificationType, string body, Exception error);
    }
}