using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace GeoCoding
{
    public class StatisticsViewModel : ViewModelBase
    {
        private int _interval;
        private DateTime _timeStart;
        private DispatcherTimer _timer;
        private IEnumerable<EntityGeoCod> _collection;

        public bool IsSave { get; set; } = false;
        /// <summary>
        /// Поле для хранения статистики
        /// </summary>
        private Statistics _statistics;
        /// <summary>
        /// Статистика по выполненному геокодированию
        /// </summary>
        public Statistics Statistics
        {
            get => _statistics;
            set => Set(ref _statistics, value);
        }

        public void Init(IEnumerable<EntityGeoCod> collection, int interval = 1)
        {
            _interval = interval;
            _collection = collection;
            Statistics = new Statistics();
            UpdateStatisticsCollection();

            if (_timer == null)
            {
                _timer = new DispatcherTimer(TimeSpan.FromSeconds(_interval), DispatcherPriority.DataBind, GetStat, Dispatcher.CurrentDispatcher)
                {
                    IsEnabled = false
                };
            }
        }
        public void Start()
        {
            _timer.Start();
            _timeStart = DateTime.Now;
            GetStat(null, null);
        }
        public void Stop()
        {
            _timer.Stop();
            GetStat(null, null);
        }
        private void GetStat(object sender, EventArgs e)
        {
            UpdateStatisticsCollection();
            _statistics.TimeGeoCod = TimeSpan.FromSeconds((DateTime.Now - _timeStart).TotalSeconds);
        }
        public void UpdateStatisticsCollection()
        {
            if (_collection != null)
            {
                _statistics.AllEntity = _collection.Count();
                _statistics.OK = _collection.Count(x => x.Status == StatusType.OK);
                _statistics.NotGeoCoding = _collection.Count(x => x.Status == StatusType.NotGeoCoding);
                _statistics.GeoCodingNow = _collection.Count(x => x.Status == StatusType.GeoCodingNow);
                _statistics.Error = _collection.Count(x => x.Status == StatusType.Error);
                _statistics.House = _collection.Count(x => x.MainGeoCod?.Kind == KindType.House);
                _statistics.Exact = _collection.Count(x => x.MainGeoCod?.Precision == PrecisionType.Exact);
                _statistics.NotFound = _collection.Count(x => x.CountResult == 0);
                _statistics.Percent = ((_statistics.AllEntity - _statistics.NotGeoCoding - _statistics.GeoCodingNow) / (double)_statistics.AllEntity) * 100;
                IsSave = false;
            }
        }
    }
}