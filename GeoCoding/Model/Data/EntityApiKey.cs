using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.Model.Data
{
    public class EntityApiKey:ViewModelBase
    {
        private string _apiKey = string.Empty;
        /// <summary>
        /// Сам ключ
        /// </summary>
        public string ApiKey
        {
            get => _apiKey;
            set => Set(ref _apiKey, value);
        }

        private string _description = string.Empty;
        /// <summary>
        /// Описание ключа
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private int _currentLimit = 0;
        /// <summary>
        /// 
        /// </summary>
        public int CurrentLimit
        {
            get => _currentLimit;
            set => Set(ref _currentLimit, value);
        }

        private int _currentSpent = 0;
        /// <summary>
        /// 
        /// </summary>
        public int CurrentSpent
        {
            get => _currentSpent;
            set => Set(ref _currentSpent, value);
        }

        private List<DayWeek> _collectionDayWeekSettings;
        /// <summary>
        /// Настройки по дням недели
        /// </summary>
        public List<DayWeek> CollectionDayWeekSettings
        {
            get => _collectionDayWeekSettings;
            set => Set(ref _collectionDayWeekSettings, value);
        }
    }
}