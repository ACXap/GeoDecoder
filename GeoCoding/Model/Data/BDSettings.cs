using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class BDSettings : ViewModelBase
    {

        private string _server = string.Empty;
        public string Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        private string _bdName = string.Empty;
        public string BDName
        {
            get => _bdName;
            set => Set(ref _bdName, value);
        }

        private int _port = 5432;
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private string _login = string.Empty;
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }


        private string _sqlQuery = string.Empty;
        public string SQLQuery
        {
            get => _sqlQuery;
            set => Set(ref _sqlQuery, value);
        }

        private StatusConnectBD _statusConnect = StatusConnectBD.NotConnect;
        public StatusConnectBD StatusConnect
        {
            get => _statusConnect;
            set => Set(ref _statusConnect, value);
        }

        private string _error = string.Empty;
        public string Error
        {
            get => _error;
            set => Set("Error", ref _error, value);
        }
    }
}