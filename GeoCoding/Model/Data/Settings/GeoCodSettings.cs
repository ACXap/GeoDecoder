using GalaSoft.MvvmLight;
using GeoCoding.Model.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек геокодирования
    /// </summary>
    public class GeoCodSettings : ViewModelBase
    {
        /// <summary>
        /// Поле для хранения имени файла для сохранения геокодеров
        /// </summary>
        private string _file = Directory.GetCurrentDirectory() + "//geocoder.dat";
        /// <summary>
        /// Поле для хранения параметра геокодировать все объекты
        /// </summary>
        private bool _canGeoCodGetAll = true;
        /// <summary>
        /// Поле для хранения параметра геокодировать только объекты с ошибками
        /// </summary>
        private bool _canGeoCodGetError = false;
        /// <summary>
        /// Поле для хранения параметра геокодировать негеокодированные объекты
        /// </summary>
        private bool _canGeoCodGetNotGeo = false;
        /// <summary>
        /// Поле для хранения параметра сохранить все данные геокодирования в полном объеме
        /// </summary>
        private bool _canSaveDataAsTemp = true;
        /// <summary>
        /// Поле для хранения параметра сохранить все данные геокодирования в формате для базы данных
        /// </summary>
        private bool _canSaveDataAsFinished = true;
        /// <summary>
        /// Поле для хранения параметра открывать ли папку с результатами после геокодирования и сохранения
        /// </summary>
        private bool _canOpenFolderAfter = false;
        /// <summary>
        /// Поле для хранения параметра геокодировать сразу по выбору файла
        /// </summary>
        private bool _canGeoCodAfterGetFile = false;

        /// <summary>
        /// Поле для хранения названия геосервиса
        /// </summary>
        //private string _geoService = "YANDEX";

        /// <summary>
        /// Поле для хранения параметра сохранять ли статистику
        /// </summary>
        private bool _canSaveStatistics = true;
        /// <summary>
        /// Имя файла для сохранения геокодеров
        /// </summary>
        public string File
        {
            get => _file;
            private set => Set(ref _file, value);
        }

        /// <summary>
        /// Геокодировать все объекты
        /// </summary>
        public bool CanGeoCodGetAll
        {
            get => _canGeoCodGetAll;
            set => Set(ref _canGeoCodGetAll, value);
        }

        /// <summary>
        /// Геокодировать только объекты с ошибками
        /// </summary>
        public bool CanGeoCodGetError
        {
            get => _canGeoCodGetError;
            set => Set(ref _canGeoCodGetError, value);

        }

        /// <summary>
        /// Геокодировать негеокодированные объекты
        /// </summary>
        public bool CanGeoCodGetNotGeo
        {
            get => _canGeoCodGetNotGeo;
            set => Set(ref _canGeoCodGetNotGeo, value);
        }

        /// <summary>
        /// Сохранить все данные геокодирования в полном объеме
        /// </summary>
        public bool CanSaveDataAsTemp
        {
            get => _canSaveDataAsTemp;
            set => Set(ref _canSaveDataAsTemp, value);
        }

        /// <summary>
        /// Сохранить все данные геокодирования в формате для базы данных
        /// </summary>
        public bool CanSaveDataAsFinished
        {
            get => _canSaveDataAsFinished;
            set => Set(ref _canSaveDataAsFinished, value);
        }

        /// <summary>
        /// открывать ли папку с результатом после сохранения
        /// </summary>
        public bool CanOpenFolderAfter
        {
            get => _canOpenFolderAfter;
            set => Set(ref _canOpenFolderAfter, value);
        }

        /// <summary>
        /// Геокодировать ли сразу по выбору файла
        /// </summary>
        public bool CanGeoCodAfterGetFile
        {
            get => _canGeoCodAfterGetFile;
            set => Set(ref _canGeoCodAfterGetFile, value);
        }

        /// <summary>
        /// Сохранять ли статистику
        /// </summary>
        public bool CanSaveStatistics
        {
            get => _canSaveStatistics;
            set => Set(ref _canSaveStatistics, value);
        }

        /// <summary>
        /// Название геосервиса
        /// </summary>
        //public string GeoService
        //{
        //    get => _geoService;
        //    set => Set(ref _geoService, value);
        //}

        private string _key = string.Empty;
        /// <summary>
        /// Апи-ключ
        /// </summary>
        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }

        private bool _isMultipleRequests = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMultipleRequests
        {
            get => _isMultipleRequests;
            set => Set(ref _isMultipleRequests, value);
        }

        private bool _isMultipleProxy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMultipleProxy
        {
            get => _isMultipleProxy;
            set => Set(ref _isMultipleProxy, value);
        }

        private int _countRequests = 5;
        /// <summary>
        /// 
        /// </summary>
        public int CountRequests
        {
            get => _countRequests;
            set => Set(ref _countRequests, value);
        }

        private int _countProxy = 20;
        /// <summary>
        /// 
        /// </summary>
        public int CountProxy
        {
            get => _countProxy;
            set => Set(ref _countProxy, value);
        }

        private int _maxCountError = 50;
        /// <summary>
        /// 
        /// </summary>
        public int MaxCountError
        {
            get => _maxCountError;
            set => Set(ref _maxCountError, value);
        }

        private int _maxCountErrorForProxy = 10;
        /// <summary>
        /// 
        /// </summary>
        public int MaxCountErrorForProxy
        {
            get => _maxCountErrorForProxy;
            set => Set(ref _maxCountErrorForProxy, value);
        }

        private bool _canUsePolygon = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanUsePolygon
        {
            get => _canUsePolygon;
            set
            {
                var oldValue = _canUsePolygon;
                Set(ref _canUsePolygon, value);
                RaisePropertyChanged(nameof(CanUsePolygon), oldValue, value, true);
            }
        }

        private ObservableCollection<EntityGeoCoder> _collectionGeoCoder;
        /// <summary>
        /// Коллекция геокодеров
        /// </summary>
        public ObservableCollection<EntityGeoCoder> CollectionGeoCoder
        {
            get => _collectionGeoCoder;
            set => Set(ref _collectionGeoCoder, value);
        }

        private EntityGeoCoder _currentGeoCoder;
        /// <summary>
        /// Текущий геокодер
        /// </summary>
        public EntityGeoCoder CurrentGeoCoder
        {
            get => _currentGeoCoder;
            set => Set(ref _currentGeoCoder, value);
        }

        /// <summary>
        /// Коллекция всех возможных геосервисов
        /// </summary>
        public ReadOnlyCollection<string> CollectionGeoService => GeoCodingService.MainGeoService.AllNameService;
    }
}