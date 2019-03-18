using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class ProxyEntity:ViewModelBase
    {
        private string _address = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        private int _port = 0;
        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private int _delay = 0;
        /// <summary>
        /// 
        /// </summary>
        public int Delay
        {
            get => _delay;
            set => Set(ref _delay, value);
        }

        private bool _isActive = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }


        private string _error = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
    }
}