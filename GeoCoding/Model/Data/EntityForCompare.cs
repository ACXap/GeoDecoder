using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class EntityForCompare:ViewModelBase
    {
        private EntityGeoCod _geoCode;
        /// <summary>
        /// 
        /// </summary>
        public EntityGeoCod GeoCode
        {
            get => _geoCode;
            set => Set(ref _geoCode, value);
        }

        private int _globalIdAfterCompare;
        /// <summary>
        /// 
        /// </summary>
        public int GlobalIdAfterCompare
        {
            get => _globalIdAfterCompare;
            set => Set(ref _globalIdAfterCompare, value);
        }

        private int _qCode;
        /// <summary>
        /// 
        /// </summary>
        public int Qcode
        {
            get => _qCode;
            set => Set(ref _qCode, value);
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

        private StatusCompareType _status = StatusCompareType.NotCompare;
        /// <summary>
        /// 
        /// </summary>
        public StatusCompareType Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private bool _isNotMatch;
        /// <summary>
        /// 
        /// </summary>
        public bool IsNotMatch
        {
            get => _isNotMatch;
            set => Set(ref _isNotMatch, value);
        }
    }
}