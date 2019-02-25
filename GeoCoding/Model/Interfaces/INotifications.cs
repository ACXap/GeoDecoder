namespace GeoCoding
{
    public interface INotifications
    {
        void Notification(string header, string body);
        void Notification(NotificationType notificationType);
        void Notification(NotificationType notificationType, string header, string body);
        void Notification(NotificationType notificationType, string body);
    }
}