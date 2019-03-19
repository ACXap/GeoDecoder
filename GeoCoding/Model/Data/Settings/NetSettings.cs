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


        private StatusConnect _status = StatusConnect.NotConnect;
        /// <summary>
        /// 
        /// </summary>
        public StatusConnect Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
    }
}