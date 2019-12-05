// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек дня недели
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DayWeek: ViewModelBase
    {
        private bool _selected = false;
        /// <summary>
        /// Выбран ли день недели
        /// </summary>
        [JsonProperty("Selected")]
        public bool Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }
        private DayOfWeek _day;
        /// <summary>
        /// Имя дня недели
        /// </summary>
        [JsonProperty("Day")]
        public DayOfWeek Day
        {
            get => _day;
            set => Set(ref _day, value);
        }
        private int _maxCount = 0;
        /// <summary>
        /// Максимальное допустимое значение чего либо в этот день
        /// </summary>
        [JsonProperty("MaxCount")]
        public int MaxCount
        {
            get => _maxCount;
            set => Set(ref _maxCount, value);
        }
    }
}