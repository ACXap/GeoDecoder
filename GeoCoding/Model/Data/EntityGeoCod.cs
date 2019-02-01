using GalaSoft.MvvmLight;
using System;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения объектов адресов
    /// </summary>
    public class EntityGeoCod : ViewModelBase
    {
        private int _globalId;
        /// <summary>
        /// Глобальный идентификатор в базе - данные для базы
        /// </summary>
        public int GlobalID
        {
            get => _globalId;
            set => Set(ref _globalId, value);
        }

        private string _address = string.Empty;
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

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

        private string _error = string.Empty;
        /// <summary>
        /// Различные ошибки при работе
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private StatusType _status;
        /// <summary>
        /// В каком статусе проверка объекта
        /// </summary>
        public StatusType Status
        {
            get => _status;
            set => Set("Status", ref _status, value, true);
        }

        private DateTime _dateTimeGeoCod;
        /// <summary>
        /// Дата и время геокодирования
        /// </summary>
        public DateTime DateTimeGeoCod
        {
            get => _dateTimeGeoCod;
            set => Set(ref _dateTimeGeoCod, value, true);
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

        private byte _countResult = 0;
        /// <summary>
        /// Количество найденных координат для данного адреса, должно быть 1.
        /// </summary>
        public byte CountResult
        {
            get => _countResult;
            set => Set(ref _countResult, value);
        }
    }
}