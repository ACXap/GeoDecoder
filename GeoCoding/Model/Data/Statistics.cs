using GalaSoft.MvvmLight;
using System;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения статистики
    /// </summary>
    public class Statistics : ViewModelBase
    {
        private int _allEntity = 0;
        /// <summary>
        /// Всего объектов в коллекции
        /// </summary>
        public int AllEntity
        {
            get => _allEntity;
            set => Set(ref _allEntity, value);
        }

        private int _ok = 0;
        /// <summary>
        /// Всего объектов со статусом "ОК"
        /// </summary>
        public int OK
        {
            get => _ok;
            set => Set(ref _ok, value);
        }

        private int _error = 0;
        /// <summary>
        /// Всего объектов со статусом "Ошибка"
        /// </summary>
        public int Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private int _notGeoCoding = 0;
        /// <summary>
        /// Всего объектов со статусом "Негеокодирован"
        /// </summary>
        public int NotGeoCoding
        {
            get => _notGeoCoding;
            set => Set(ref _notGeoCoding, value);
        }

        private int _geoCodingNow = 0;
        /// <summary>
        /// Всего объектов со статусом "Геокодируется сейчас"
        /// </summary>
        public int GeoCodingNow
        {
            get => _geoCodingNow;
            set => Set(ref _geoCodingNow, value);
        }

        private int _house = 0;
        /// <summary>
        /// Всего объектов с типом объекта "Дом"
        /// </summary>
        public int House
        {
            get => _house;
            set => Set(ref _house, value);
        }

        private int _exact = 0;
        /// <summary>
        /// Всего объектов с качеством геокодирования "Точное геокодирование"
        /// </summary>
        public int Exact
        {
            get => _exact;
            set => Set(ref _exact, value);
        }

        private int _notFound = 0;
        /// <summary>
        /// Всего объектов для которых не найдены варианты
        /// </summary>
        public int NotFound
        {
            get => _notFound;
            set => Set(ref _notFound, value);
        }

        private TimeSpan _timeGeoCod ;
        /// <summary>
        /// Время геокодирования в секундах
        /// </summary>
        public TimeSpan TimeGeoCod
        {
            get => _timeGeoCod;
            set => Set(ref _timeGeoCod, value);
        }

        private TimeSpan _timeLeftGeoCod;
        /// <summary>
        /// Время оставшееся до завершения геокодирования
        /// </summary>
        public TimeSpan TimeLeftGeoCod
        {
            get => _timeLeftGeoCod;
            set => Set(ref _timeLeftGeoCod, value);
        }

        private double _percent;
        /// <summary>
        /// Процент выполненого геокодирования
        /// </summary>
        public double Percent
        {
            get => _percent;
            set => Set(ref _percent, value);
        }

        private string _geoServiceName = string.Empty;
        /// <summary>
        /// Название геосервиса
        /// </summary>
        public string GeoServiceName
        {
            get => _geoServiceName;
            set => Set(ref _geoServiceName, value);
        }
    }
}