// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения результата геокодирования адреса
    /// </summary>
    public class GeoCod : ViewModelBase
    {
        private string _addressWeb = string.Empty;
        /// <summary>
        /// Адрес объекта полученный после геокодирования
        /// </summary>
        public string AddressWeb
        {
            get => _addressWeb;
            set => Set(ref _addressWeb, value);
        }

        private string _longitude = string.Empty;
        /// <summary>
        /// Долгота - данные для базы
        /// </summary>
        public string Longitude
        {
            get => _longitude;
            set => Set(ref _longitude, value);
        }

        private string _latitude = string.Empty;
        /// <summary>
        /// Широта - данные для базы
        /// </summary>
        public string Latitude
        {
            get => _latitude;
            set => Set(ref _latitude, value);
        }

        private byte _qCode;
        /// <summary>
        /// Качество полученных геокоординат - данные для базы
        /// </summary>
        public byte Qcode
        {
            get => _qCode;
            set => Set(ref _qCode, value);
        }

        private KindType _kind;
        /// <summary>
        /// Тип объекта
        /// </summary>
        public KindType Kind
        {
            get => _kind;
            set => Set(ref _kind, value);
        }

        private PrecisionType _precision;
        /// <summary>
        /// Точность соответствия запроса и результата
        /// </summary>
        public PrecisionType Precision
        {
            get => _precision;
            set => Set(ref _precision, value);
        }


        private string _matchQuality;
        /// <summary>
        /// Точность по сегментам
        /// </summary>
        public string MatchQuality
        {
            get => _matchQuality;
            set => Set(ref _matchQuality, value);
        }
    }
}