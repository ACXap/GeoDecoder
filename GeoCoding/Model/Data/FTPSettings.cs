using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class FTPSettings : ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        private string _server;
        /// <summary>
        /// 
        /// </summary>
        public string Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private int _port;
        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private string _user;
        /// <summary>
        /// 
        /// </summary>
        public string User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private string _password;
        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private string _folderInput;
        /// <summary>
        /// 
        /// </summary>
        public string FolderInput
        {
            get => _folderInput;
            set => Set(ref _folderInput, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private string _folderOutput;
        /// <summary>
        /// 
        /// </summary>
        public string FolderOutput
        {
            get => _folderOutput;
            set => Set(ref _folderOutput, value);
        }
    }
}