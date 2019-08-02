using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GeoCodingLocalBD.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding
{
    public class PolygonViewModel : ViewModelBase
    {
        private PolygonModel _model = new PolygonModel();
        private INotifications _notification;

        public PolygonViewModel(INotifications notification)
        {
            _notification = notification;

            if (!_model.CheckBD())
            {
              
                //  _notification.Notification(NotificationType.Error, "Отсутствует база полигонов");
            }
        }

        private List<EntityAddress> _listAddress;

        private List<double> _polygon = new List<double>();
        public List<double> Polygon
        {
            get => _polygon;
            set => Set(ref _polygon, value);
        }

        private string _addressPolygon = string.Empty;
        public string AddressPolygon
        {
            get => _addressPolygon;
            set => Set(ref _addressPolygon, value);
        }

        private List<EntityAddress> _listRegion;
        public List<EntityAddress> ListRegion
        {
            get => _listRegion;
            set => Set(ref _listRegion, value);
        }

        private List<EntityAddress> _listDistrict;
        public List<EntityAddress> ListDistrict
        {
            get => _listDistrict;
            set => Set(ref _listDistrict, value);
        }

        private EntityAddress _currentRegion;
        public EntityAddress СurrentRegion
        {
            get => _currentRegion;
            set
            {
                Set(ref _currentRegion, value);

                if (value != null)
                {
                    ListDistrict = _listAddress.Where(x => x.ParentId == value.Id).ToList();
                    Polygon = _model.GetPolygon(value.Id);
                }
            }
        }

        private EntityAddress _currentDistrict;
        public EntityAddress CurrentDistrict
        {
            get => _currentDistrict;
            set
            {
                Set(ref _currentDistrict, value);
                if (value != null)
                {
                    Polygon = _model.GetPolygon(value.Id);
                }
            }
        }

        private bool _canAutoChoicePolygon = true;
        public bool CanAutoChoicePolygon
        {
            get => _canAutoChoicePolygon;
            set
            {
                Set(ref _canAutoChoicePolygon, value);
                if (value)
                {
                    GetAddress();
                }
            }
        }

        private bool _canManualChoicePolygon = false;
        public bool CanManualChoicePolygon
        {
            get => _canManualChoicePolygon;
            set
            {
                Set(ref _canManualChoicePolygon, value);
                if (value)
                {
                    GetAddress();
                }
            }
        }

        private RelayCommand _commandGetSettingsPolygon;
        public RelayCommand CommandGetSettingsPolygon =>
        _commandGetSettingsPolygon ?? (_commandGetSettingsPolygon = new RelayCommand(
                    () =>
                    {
                        GetAddress();
                    }));

        public List<double> GetPolygon(string address)
        {
            if (_canManualChoicePolygon)
            {
                return _polygon;
            }

            foreach (var item in _listRegion)
            {
                if (address.Contains(item.Address))
                {
                    foreach (var adr in _listAddress.Where(x => x.ParentId == item.Id))
                    {
                        if (address.Contains(adr.Address))
                        {
                            return _model.GetPolygon(adr.OrponId);

                        }
                    }

                    return _model.GetPolygon(item.OrponId);
                }
            }

            //foreach (var item in _listAddress)
            //{
            //    if(address.Contains(item.Address))
            //    {
            //        return _model.GetPolygon(item.OrponId);
            //        break;
            //    }
            //}

            return null;
        }

        public void GetAddress()
        {
            if (_listAddress == null)
            {
                _listAddress = _model.GetAddress();
            }
            if (!_model.CheckBD())
            {
                _notification.Notification(NotificationType.Error, "Отсутствует база полигонов");
                return;
            }


            // if(_canManualChoicePolygon)
            //{
            //  ListRegion = _listAddress.Where(x => x.AdminLevel == 4).ToList();
            try
            {
                ListRegion = _listAddress.Where(x => x.ParentId == 354539191).ToList();
            }
            catch (Exception ex)
            {
                _notification.Notification(NotificationType.Error, ex.Message);
            }
            
            // }
        }
    }
}