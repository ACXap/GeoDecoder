using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            set
            {
                var oldValue = _isNotProxy;
                Set(ref _isNotProxy, value);
                RaisePropertyChanged("IsNotProxy", oldValue, value, true);
            }
        }

        private bool _isSystemProxy = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSystemProxy
        {
            get => _isSystemProxy;
            set
            {
                var oldValue = _isSystemProxy;
                Set(ref _isSystemProxy, value);
                RaisePropertyChanged("IsSystemProxy", oldValue, value, true);
            }
        }

        private bool _isManualProxy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsManualProxy
        {
            get => _isManualProxy;
            set
            {
                var oldValue = _isManualProxy;
                Set(ref _isManualProxy, value);
                RaisePropertyChanged("IsManualProxy", oldValue, value, true);
            }
        }

        private bool _isListProxy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsListProxy
        {
            get => _isListProxy;
            set
            {
                var oldValue = _isListProxy;
                Set(ref _isListProxy, value);
                RaisePropertyChanged("IsListProxy",oldValue, value, true);
            }
        }

        //private string _proxyAddress = string.Empty;
        ///// <summary>
        ///// 
        ///// </summary>
        //public string ProxyAddress
        //{
        //    get => _proxyAddress;
        //    set => Set(ref _proxyAddress, value);
        //}

        //private int _proxyPort = 0;
        ///// <summary>
        ///// 
        ///// </summary>
        //public int ProxyPort
        //{
        //    get => _proxyPort;
        //    set => Set(ref _proxyPort, value);
        //}

        //private string _error = string.Empty;
        ///// <summary>
        ///// 
        ///// </summary>
        //public string Error
        //{
        //    get => _error;
        //    set => Set(ref _error, value);
        //}


        private ProxyEntity _proxy;
        /// <summary>
        /// 
        /// </summary>
        public ProxyEntity Proxy
        {
            get => _proxy;
            set => Set(ref _proxy, value);
        }

        private ObservableCollection<ProxyEntity> _collectionListProxy;
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ProxyEntity> CollectionListProxy
        {
            get => _collectionListProxy;
            set => Set(ref _collectionListProxy, value);
        }
    }
}