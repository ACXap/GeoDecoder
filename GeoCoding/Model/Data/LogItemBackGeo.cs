using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.Model.Data
{
    public class LogItemBackGeo:ViewModelBase
    {

        private string _textLog = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string TextLog
        {
            get => _textLog;
            set => Set(ref _textLog, value);
        }

        private DateTime _dateTimeLog;
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTimeLog
        {
            get => _dateTimeLog;
            set => Set(ref _dateTimeLog, value);
        }

        private string _countRow = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string CountRow
        {
            get => _countRow;
            set => Set(ref _countRow, value);
        }
    }
}