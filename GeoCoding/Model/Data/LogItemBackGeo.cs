// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using System;

namespace GeoCoding.Model.Data
{
    public class LogItemBackGeo:ViewModelBase
    {

        #region PrivateField
        
        private string _textLog = string.Empty;
        private DateTime _dateTimeLog;
        private string _countRow = string.Empty;
        
        #endregion PrivateField

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string TextLog
        {
            get => _textLog;
            set => Set(ref _textLog, value);
        }

        /// <summary>
        /// Время сообщения
        /// </summary>
        public DateTime DateTimeLog
        {
            get => _dateTimeLog;
            set => Set(ref _dateTimeLog, value);
        }

        /// <summary>
        /// Количество записей
        /// </summary>
        public string CountRow
        {
            get => _countRow;
            set => Set(ref _countRow, value);
        }
    }
}