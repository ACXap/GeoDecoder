// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек оповещения
    /// </summary>
    public class NotificationSettings : ViewModelBase
    {
        private bool _canNotificationSaveSettings = true;
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

        private bool _canNotificationSaveData = true;
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

        private bool _canNotificationProcessCancel = true;
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

        private bool _canNotificationStatAlreadySave = true;
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

        private bool _canNotificationOnlyError = true;
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

        private bool _canNotificationExit = true;
        /// <summary>
        /// Оповещать ли о закрытии
        /// </summary>
        public bool CanNotificationExit
        {
            get => _canNotificationExit;
            set => Set(ref _canNotificationExit, value);
        }


        private string _recipientsResultFile = string.Empty;
        /// <summary>
        /// Список получателей итоговых файлов
        /// </summary>
        public string RecipientsResultFile
        {
            get => _recipientsResultFile;
            set => Set(ref _recipientsResultFile, value);
        }

        private bool _canSendFileOnMail = false;
        /// <summary>
        /// Присылать ли результаты на почту
        /// </summary>
        public bool CanSendFileOnMail
        {
            get => _canSendFileOnMail;
            set => Set(ref _canSendFileOnMail, value);
        }

        private string _mailSender = string.Empty;
        /// <summary>
        /// Почта отправителя
        /// </summary>
        public string MailSender
        {
            get => _mailSender;
            set => Set(ref _mailSender, value);
        }
    }
}