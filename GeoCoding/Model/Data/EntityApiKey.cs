// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeoCoding.Model.Data
{
    /// <summary>
    /// Класс для хранения сущности апи-ключа
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityApiKey:ViewModelBase
    {
        #region PrivateField
        private string _apiKey = string.Empty;
        private string _apiKeyStat = string.Empty;
        private string _apiKeyDevelop = string.Empty;
        private string _description = string.Empty;
        private int _currentLimit = 0;
        private int _currentSpent = 0;
        private int _currentSpentServer = 0;
        private DateTime _dateCurrentSpent;
        private int _maxLimit = 0;
        private List<DayWeek> _collectionDayWeekSettings;
        private string _error = string.Empty;
        private StatusSyncType _statusSync = StatusSyncType.NotSync;
        #endregion PrivateField

        /// <summary>
        /// Сам ключ
        /// </summary>
        [JsonProperty("ApiKey")]
        public string ApiKey
        {
            get => _apiKey;
            set => Set(ref _apiKey, value);
        }

        /// <summary>
        /// Ключ для получения статистики
        /// </summary>
        [JsonProperty("ApiKeyStat")]
        public string ApiKeyStat
        {
            get => _apiKeyStat;
            set => Set(ref _apiKeyStat, value);
        }

        /// <summary>
        /// Ключ разработчика
        /// </summary>
        [JsonProperty("ApiKeyDevelop")]
        public string ApiKeyDevelop
        {
            get => _apiKeyDevelop;
            set => Set(ref _apiKeyDevelop, value);
        }

        /// <summary>
        /// Описание ключа
        /// </summary>
        [JsonProperty("Description")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        /// <summary>
        /// Текущий лимит на сегодня
        /// </summary>
        public int CurrentLimit
        {
            get => _currentLimit;
            set => Set(ref _currentLimit, value);
        }

        /// <summary>
        /// Текущий потраченный лимит
        /// </summary>
        public int CurrentSpent
        {
            get => _currentSpent;
            set => Set(ref _currentSpent, value);
        }

        /// <summary>
        /// Текущий потраченный лимит информация с сервера
        /// </summary>
        public int CurrentSpentServer
        {
            get => _currentSpentServer;
            set => Set(ref _currentSpentServer, value);
        }

        /// <summary>
        /// Дата последнего потраченного лимита
        /// </summary>
        public DateTime DateCurrentSpent
        {
            get => _dateCurrentSpent;
            set => Set(ref _dateCurrentSpent, value);
        }

        /// <summary>
        /// Максимально допустимый лимит в сутки
        /// </summary>
        [JsonProperty("MaxLimit")]
        public int MaxLimit
        {
            get => _maxLimit;
            set => Set(ref _maxLimit, value);
        }

        /// <summary>
        /// Настройки по дням недели
        /// </summary>
        [JsonProperty("CollectionDayWeekSettings")]
        public List<DayWeek> CollectionDayWeekSettings
        {
            get => _collectionDayWeekSettings;
            set => Set(ref _collectionDayWeekSettings, value);
        }

        /// <summary>
        /// Статус синхронизации апи-ключа с базой данных
        /// </summary>
        public StatusSyncType StatusSync
        {
            get => _statusSync;
            set => Set(ref _statusSync, value);
        }

        /// <summary>
        /// Ошибка синхронизации
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }
    }
}