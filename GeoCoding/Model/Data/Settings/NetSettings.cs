using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения сетевых настроек
    /// </summary>
    public class NetSettings:ViewModelBase
    {
        private bool _isNotProxy = false;
        /// <summary>
        /// Не использовать прокси сервер
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
        /// Использовать системный прокси сервер
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
        /// Использовать прокси сервер настроенный вручную
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
        /// Использовать список прокси серверов
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
        /// Настроенный прокси сервер
        /// </summary>
        public ProxyEntity Proxy
        {
            get => _proxy;
            set => Set(ref _proxy, value);
        }

        private ObservableCollection<ProxyEntity> _collectionListProxy;
        /// <summary>
        /// Коллекция для хранения списка прокси серверов
        /// </summary>
        public ObservableCollection<ProxyEntity> CollectionListProxy
        {
            get => _collectionListProxy;
            set => Set(ref _collectionListProxy, value);
        }


        private StatusConnect _status = StatusConnect.NotConnect;
        /// <summary>
        /// Статус проверки коллекции прокси серверов
        /// </summary>
        public StatusConnect Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
    }
}