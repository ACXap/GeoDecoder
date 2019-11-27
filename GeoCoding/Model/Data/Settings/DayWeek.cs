﻿using GalaSoft.MvvmLight;
using System;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек дня недели
    /// </summary>
    public class DayWeek: ViewModelBase
    {
        private bool _selected = false;
        /// <summary>
        /// Выбран ли день недели
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }
        private DayOfWeek _day;
        /// <summary>
        /// Имя дня недели
        /// </summary>
        public DayOfWeek Day
        {
            get => _day;
            set => Set(ref _day, value);
        }
        private int _maxCount = 0;
        /// <summary>
        /// Максимальное допустимое значение чего либо в этот день
        /// </summary>
        public int MaxCount
        {
            get => _maxCount;
            set => Set(ref _maxCount, value);
        }
    }
}