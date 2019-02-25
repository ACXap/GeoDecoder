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

        public async void Notification(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Error:
                    await dialogCoordinator.ShowMessageAsync(this, _headerError, _bodyError);
                    break;
                case NotificationType.Ok:
                    await dialogCoordinator.ShowMessageAsync(this, _headerOk, _bodyOk);
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
                default:
                    break;
            }
        }

        public void Notification(NotificationType notificationType, string body)
        {
            switch (notificationType)
            {
                case NotificationType.Error:
                    break;
                case NotificationType.Ok:
                    break;
                default:
                    break;
            }
        }
    }
}