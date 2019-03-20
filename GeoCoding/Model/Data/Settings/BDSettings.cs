using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек подключения к базе данных
    /// </summary>
    public class BDSettings : ViewModelBase
    {
        private string _server = string.Empty;
        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        private string _bdName = string.Empty;
        /// <summary>
        /// Имя базы данных
        /// </summary>
        public string BDName
        {
            get => _bdName;
            set => Set(ref _bdName, value);
        }

        private int _port = 5432;
        /// <summary>
        /// Порт сервера
        /// </summary>
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private string _login = string.Empty;
        /// <summary>
        /// Логин подключения к базе данных
        /// </summary>
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string _password = string.Empty;
        /// <summary>
        /// пароль подключения к базе данных
        /// </summary>
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _sqlQuery = string.Empty;
        /// <summary>
        /// SQL - запрос к базе данных
        /// </summary>
        public string SQLQuery
        {
            get => _sqlQuery;
            set => Set(ref _sqlQuery, value);
        }

        private StatusConnect _statusConnect = StatusConnect.NotConnect;
        /// <summary>
        /// Статус подключения к базе данных
        /// </summary>
        public StatusConnect StatusConnect
        {
            get => _statusConnect;
            set => Set(ref _statusConnect, value);
        }

        private string _error = string.Empty;
        /// <summary>
        /// Сообщение об ошибке при подключении
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set("Error", ref _error, value);
        }
    }
}