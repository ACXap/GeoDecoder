using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeoCoding.Model.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityApiKey:ViewModelBase
    {
        private string _apiKey = string.Empty;

        /// <summary>
        /// Сам ключ
        /// </summary>
        [JsonProperty("ApiKey")]
        public string ApiKey
        {
            get => _apiKey;
            set => Set(ref _apiKey, value);
        }

        private string _apiKeyStat = string.Empty;
        /// <summary>
        /// Ключ для получения статистики
        /// </summary>
        [JsonProperty("ApiKeyStat")]
        public string ApiKeyStat
        {
            get => _apiKeyStat;
            set => Set(ref _apiKeyStat, value);
        }

        private string _apiKeyDevelop = string.Empty;
        /// <summary>
        /// Ключ разработчика
        /// </summary>
        [JsonProperty("ApiKeyDevelop")]
        public string ApiKeyDevelop
        {
            get => _apiKeyDevelop;
            set => Set(ref _apiKeyDevelop, value);
        }

        private string _description = string.Empty;
        /// <summary>
        /// Описание ключа
        /// </summary>
        [JsonProperty("Description")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private int _currentLimit = 0;
        /// <summary>
        /// Текущий лимит на сегодня
        /// </summary>
        public int CurrentLimit
        {
            get => _currentLimit;
            set => Set(ref _currentLimit, value);
        }

        private int _currentSpent = 0;
        /// <summary>
        /// Текущий потраченный лимит
        /// </summary>
        public int CurrentSpent
        {
            get => _currentSpent;
            set => Set(ref _currentSpent, value);
        }

        private int _currentSpentServer = 0;
        /// <summary>
        /// Текущий потраченный лимит информация с сервера
        /// </summary>
        public int CurrentSpentServer
        {
            get => _currentSpentServer;
            set => Set(ref _currentSpentServer, value);
        }

        private DateTime _dateCurrentSpent;
        /// <summary>
        /// Дата последнего потраченного лимита
        /// </summary>
        public DateTime DateCurrentSpent
        {
            get => _dateCurrentSpent;
            set => Set(ref _dateCurrentSpent, value);
        }

        private List<DayWeek> _collectionDayWeekSettings;
        /// <summary>
        /// Настройки по дням недели
        /// </summary>
        [JsonProperty("CollectionDayWeekSettings")]
        public List<DayWeek> CollectionDayWeekSettings
        {
            get => _collectionDayWeekSettings;
            set => Set(ref _collectionDayWeekSettings, value);
        }

        private StatusSyncType _statusSync = StatusSyncType.NotSync;
        /// <summary>
        /// Статус синхронизации апи-ключа с базой данных
        /// </summary>
        public StatusSyncType StatusSync
        {
            get => _statusSync;
            set => Set(ref _statusSync, value);
        }

        private string _error = string.Empty;
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