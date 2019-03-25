using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек подключения к серверу фтп
    /// </summary>
    public class FTPSettings : ViewModelBase
    {
        private string _server = string.Empty;
        /// <summary>
        /// Сервер
        /// </summary>
        public string Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        private int _port = 21;
        /// <summary>
        /// Порт сервера
        /// </summary>
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private string _user;
        /// <summary>
        /// Пользователь
        /// </summary>
        public string User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        private string _password;
        /// <summary>
        /// Пароль от сервера
        /// </summary>
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private StatusType _statusConnect = StatusType.NotProcessed;
        /// <summary>
        /// Статус подключения
        /// </summary>
        public StatusType StatusConnect
        {
            get => _statusConnect;
            set => Set(ref _statusConnect, value);
        }

        private string _folderInput;
        /// <summary>
        /// Папка с загрузками
        /// </summary>
        public string FolderInput
        {
            get => _folderInput;
            set => Set(ref _folderInput, value);
        }

        private string _folderOutput;
        /// <summary>
        /// Папка с выгрузками
        /// </summary>
        public string FolderOutput
        {
            get => _folderOutput;
            set => Set(ref _folderOutput, value);
        }

        private string _error = string.Empty;
        /// <summary>
        /// Текст ошибки при подключении
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set("Error", ref _error, value);
        }
    }
}