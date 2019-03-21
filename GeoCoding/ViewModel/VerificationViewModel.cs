using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace GeoCoding
{
    public class VerificationViewModel : ViewModelBase
    {
        private readonly VerificationModel _model;
        private IEnumerable<EntityGeoCod> _collection;

        /// <summary>
        /// Поле для хранения ссылки на представления коллекции
        /// </summary>
        private ICollectionView _customerView;

        /// <summary>
        /// Представление коллекции
        /// </summary>
        public ICollectionView Customers
        {
            get => _customerView;
            set => Set(ref _customerView, value);
        }

        public void SetCollection(IEnumerable<EntityGeoCod> collection)
        {
            Customers = new CollectionViewSource { Source = collection }.View;
            _customerView.Filter = CustomerFilter;
            _collection = collection;
        }

        private bool CustomerFilter(object obj)
        {
            EntityGeoCod customer = obj as EntityGeoCod;
            return customer.MainGeoCod != null && customer.MainGeoCod.Qcode == 1;
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

        private string _error = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private string _connectSettings = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ConnectSettings
        {
            get => _connectSettings;
            set => Set(ref _connectSettings, value);
        }

        private RelayCommand _commandCheckServer;
        public RelayCommand CommandCheckServer =>
        _commandCheckServer ?? (_commandCheckServer = new RelayCommand(
                    () =>
                    {
                        _model.SettingsService(_connectSettings);

                        Status = StatusConnect.ConnectNow;

                        _model.CheckServerAsync(e =>
                        {
                            if (e != null)
                            {
                                Error = e.Message;
                                Status = StatusConnect.Error;
                            }
                            else
                            {
                                Error = string.Empty;
                                Status = StatusConnect.OK;
                            }
                        });
                    }));

        public VerificationViewModel(string connectionString)
        {
            ConnectSettings = connectionString;
            _model = new VerificationModel(connectionString);
        }
    }
}