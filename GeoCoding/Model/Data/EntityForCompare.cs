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

        private byte _qCode;
        /// <summary>
        /// 
        /// </summary>
        public byte Qcode
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

        private StatusType _status = StatusType.NotProcessed;
        /// <summary>
        /// 
        /// </summary>
        public StatusType Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private bool _isChanges;
        /// <summary>
        /// 
        /// </summary>
        public bool IsChanges
        {
            get => _isChanges;
            set => Set(ref _isChanges, value);
        }
    }
}