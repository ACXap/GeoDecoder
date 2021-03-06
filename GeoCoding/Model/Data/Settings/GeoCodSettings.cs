﻿using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек геокодирования
    /// </summary>
    public class GeoCodSettings : ViewModelBase
    {
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
        private bool _canSaveDataAsFinished = false;
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
        private string _geoService = "YANDEX";

        /// <summary>
        /// Поле для хранения параметра сохранять ли статистику
        /// </summary>
        private bool _canSaveStatistics = true;

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
        public string GeoService
        {
            get => _geoService;
            set => Set(ref _geoService, value);
        }
    }
}