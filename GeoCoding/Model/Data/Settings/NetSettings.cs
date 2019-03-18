using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class NetSettings:ViewModelBase
    {
        private bool _isNotProxy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsNotProxy
        {
            get => _isNotProxy;
            set => Set(ref _isNotProxy, value);
        }


        private bool _isSystemProxy = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSystemProxy
        {
            get => _isSystemProxy;
            set => Set(ref _isSystemProxy, value);
        }

        private bool _isManualProxy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsManualProxy
        {
            get => _isManualProxy;
            set => Set(ref _isManualProxy, value);
        }

        private bool _isListProxy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsListProxy
        {
            get => _isListProxy;
            set => Set(ref _isListProxy, value);
        }


        private string _proxyAddress = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ProxyAddress
        {
            get => _proxyAddress;
            set => Set(ref _proxyAddress, value);
        }


        private int _proxyPort = 0;
        /// <summary>
        /// 
        /// </summary>
        public int ProxyPort
        {
            get => _proxyPort;
            set => Set(ref _proxyPort, value);
        }
    }
}
