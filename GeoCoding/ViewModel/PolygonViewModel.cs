using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GeoCodingLocalBD.Data;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding
{
    public class PolygonViewModel : ViewModelBase
    {
        private PolygonModel _model = new PolygonModel();
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
                if(value!=null)
                {
                    Polygon = _model.GetPolygon(value.Id);
                }
            }
        }

        private bool _canAutoChoicePolygon = true;
        public bool CanAutoChoicePolygon
        {
            get => _canAutoChoicePolygon;
            set => Set(ref _canAutoChoicePolygon, value);
        }

        private bool _canManualChoicePolygon = false;
        public bool CanManualChoicePolygon
        {
            get => _canManualChoicePolygon;
            set
            {
                Set(ref _canManualChoicePolygon, value);
                if(value)
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

        private void GetAddress()
        {
            if (_listAddress == null && _canManualChoicePolygon)
            {
                _listAddress = _model.GetAddress();

                ListRegion = _listAddress.Where(x => x.AdminLevel == 4).ToList();
            }
        }
    }
}