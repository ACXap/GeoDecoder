using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class NotificationSettings : ViewModelBase
    {
        private bool _canNotificationSaveSettings = false;
        /// <summary>
        /// Оповещать ли о успешном сохранении настроек
        /// </summary>
        public bool MyProperty
        {
            get => _canNotificationSaveSettings;
            set => Set(ref _canNotificationSaveSettings, value);
        }

        private bool _canNotificationSaveData = false;
        /// <summary>
        /// Оповещать ли о сохранении файла
        /// </summary>
        public bool CanNotificationSaveData
        {
            get => _canNotificationSaveData;
            set => Set(ref _canNotificationSaveData, value);
        }

        private bool _canNotificationProcessCancel = false;
        /// <summary>
        /// Оповещать ли об отмене операции
        /// </summary>
        public bool CanNotificationProcessCancel
        {
            get => _canNotificationProcessCancel;
            set => Set(ref _canNotificationProcessCancel, value);
        }

        private bool _canNotificationDataProcessed = true;
        /// <summary>
        /// Оповещать о окончании процесса
        /// </summary>
        public bool CanNotificationDataProcessed
        {
            get => _canNotificationDataProcessed ;
            set => Set(ref _canNotificationDataProcessed , value);
        }

        private bool _canNotificationDataEmpty = false;
        /// <summary>
        /// Оповещать о отсутствии данных
        /// </summary>
        public bool CanNotificationDataEmpty
        {
            get => _canNotificationDataEmpty;
            set => Set(ref _canNotificationDataEmpty, value);
        }

        private bool _canNotificationStatAlreadySave = false;
        /// <summary>
        /// Оповещать если уже статистика была сохранена
        /// </summary>
        public bool CanNotificationStatAlreadySave
        {
            get => _canNotificationStatAlreadySave;
            set => Set(ref _canNotificationStatAlreadySave, value);
        }
    }
}