using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class FTPSettings : ViewModelBase
    {
        private string _server;
        /// <summary>
        /// Сервер
        /// </summary>
        public string Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        private int _port;
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

        private StatusConnect _statusConnect;
        /// <summary>
        /// Статус подключения
        /// </summary>
        public StatusConnect StatusConnect
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
    }
}