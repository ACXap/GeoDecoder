using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class GeoCodSettings:ViewModelBase
    {
        /// <summary>
        /// Поле для хранения параметра геокодировать все объекты
        /// </summary>
        private bool _canGeoCodGetAll = true;
        /// <summary>
        /// Геокодировать все объекты
        /// </summary>
        public bool CanGeoCodGetAll
        {
            get=> _canGeoCodGetAll;
            set=>Set(ref _canGeoCodGetAll, value);
        }

        /// <summary>
        /// Поле для хранения параметра геокодировать только объекты с ошибками
        /// </summary>
        private bool _canGeoCodGetError = false;
        /// <summary>
        /// Геокодировать только объекты с ошибками
        /// </summary>
        public bool CanGeoCodGetError
        {
            get => _canGeoCodGetError;
            set => Set(ref _canGeoCodGetError, value);
        }

        /// <summary>
        /// Поле для хранения параметра геокодировать негеокодированные объекты
        /// </summary>
        private bool _canGeoCodGetNotGeo = false;
        /// <summary>
        /// Геокодировать негеокодированные объекты
        /// </summary>
        public bool CanGeoCodGetNotGeo
        {
            get => _canGeoCodGetNotGeo;
            set => Set(ref _canGeoCodGetNotGeo, value);
        }
    }
}