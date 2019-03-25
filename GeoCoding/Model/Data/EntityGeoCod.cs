using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;

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

        private GeoCod _mainGeoCod;
        /// <summary>
        /// Выбранные координаты для адреса
        /// </summary>
        public GeoCod MainGeoCod
        {
            get => _mainGeoCod;
            set => Set(ref _mainGeoCod, value);
        }

        private List<GeoCod> _listGeoCod;
        /// <summary>
        /// Коллекция координат
        /// </summary>
        public List<GeoCod> ListGeoCod
        {
            get => _listGeoCod;
            set => Set(ref _listGeoCod, value);
        }

        private int _countResult = 0;
        /// <summary>
        /// Количество найденных координат для данного адреса, должно быть 1.
        /// </summary>
        public int CountResult
        {
            get => _countResult;
            set => Set(ref _countResult, value);
        }

        private string _proxy = string.Empty;
        /// <summary>
        /// Прокси через который было выполнено геокодирование
        /// </summary>
        public string Proxy
        {
            get => _proxy;
            set => Set(ref _proxy, value);
        }

        private string _geoCoder = string.Empty;
        /// <summary>
        /// Кодер которым был геокодирован адрес
        /// </summary>
        public string GeoCoder
        {
            get => _geoCoder;
            set => Set(ref _geoCoder, value);
        }
    }
}