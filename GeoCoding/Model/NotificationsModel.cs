using System;
using MahApps.Metro.Controls.Dialogs;

namespace GeoCoding
{
    public class NotificationsModel : INotifications
    {
        private const string _headerError = "Ошибка";
        private const string _bodyError = "Процесс завершен с ошибкой";
        private const string _headerOk = "Успех";
        private const string _bodyOk = "Процесс завершен успешно";

        /// <summary>
        /// Поле для хранения ссылки на координатора диалогов
        /// </summary>
        private readonly IDialogCoordinator dialogCoordinator = DialogCoordinator.Instance;

        public async void Notification(string header, string body)
        {
            await dialogCoordinator.ShowMessageAsync(this, header, body);
        }

        public void Notification(NotificationType notificationType)
        {
            throw new NotImplementedException();
        }

        public void Notification(NotificationType notificationType, string header, string body)
        {
            throw new NotImplementedException();
        }

        public void Notification(NotificationType notificationType, string body)
        {
            throw new NotImplementedException();
        }

        public void Notification(NotificationType notificationType, Exception error)
        {
            throw new NotImplementedException();
        }

        public void Notification(NotificationType notificationType, string body, Exception error)
        {
            throw new NotImplementedException();
        }
    }
}