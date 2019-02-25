using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class NotificationSettings : ViewModelBase
    {
        private bool _canNotificationSaveSettings = false;
        /// <summary>
        /// Оповещать ли о успешном сохранении настроек
        /// </summary>
        public bool CanNotificationSaveSettings
        {
            get => _canNotificationSaveSettings;
            set
            {
                Set(ref _canNotificationSaveSettings, value);
                if (value)
                {
                    CanNotificationOnlyError = false;
                }
            }
        }

        private bool _canNotificationSaveData = false;
        /// <summary>
        /// Оповещать ли о сохранении файла
        /// </summary>
        public bool CanNotificationSaveData
        {
            get => _canNotificationSaveData;
            set
            {
                Set(ref _canNotificationSaveData, value);
                if (value)
                {
                    CanNotificationOnlyError = false;
                }
            }
        }

        private bool _canNotificationProcessCancel = false;
        /// <summary>
        /// Оповещать ли об отмене операции
        /// </summary>
        public bool CanNotificationProcessCancel
        {
            get => _canNotificationProcessCancel;
            set
            {
                Set(ref _canNotificationProcessCancel, value);
                if (value)
                {
                    CanNotificationOnlyError = false;
                }
            }
        }

        private bool _canNotificationDataProcessed = true;
        /// <summary>
        /// Оповещать о окончании процесса
        /// </summary>
        public bool CanNotificationDataProcessed
        {
            get => _canNotificationDataProcessed ;
            set
            {
                Set(ref _canNotificationDataProcessed, value);
                if (value)
                {
                    CanNotificationOnlyError = false;
                }
            }
        }

        private bool _canNotificationDataEmpty = true;
        /// <summary>
        /// Оповещать о отсутствии данных
        /// </summary>
        public bool CanNotificationDataEmpty
        {
            get => _canNotificationDataEmpty;
            set
            {
                Set(ref _canNotificationDataEmpty, value);
                if (value)
                {
                    CanNotificationOnlyError = false;
                }
            }
        }

        private bool _canNotificationStatAlreadySave = false;
        /// <summary>
        /// Оповещать если уже статистика была сохранена
        /// </summary>
        public bool CanNotificationStatAlreadySave
        {
            get => _canNotificationStatAlreadySave;
            set
            {
                Set(ref _canNotificationStatAlreadySave, value);
                if(value)
                {
                    CanNotificationOnlyError = false;
                }
            }
        }

        private bool _canNotificationOnlyError = false;
        /// <summary>
        /// Все оповещения отключить, кроме ошибок
        /// </summary>
        public bool CanNotificationOnlyError
        {
            get => _canNotificationOnlyError;
            set
            {
                Set(ref _canNotificationOnlyError, value);
                if(value)
                {
                    CanNotificationDataEmpty = false;
                    CanNotificationDataProcessed = false;
                    CanNotificationProcessCancel = false;
                    CanNotificationSaveData = false;
                    CanNotificationSaveSettings = false;
                    CanNotificationStatAlreadySave = false;
                }
            }
        }
    }
}