using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace GeoCoding
{
    public class StatisticsViewModel : ViewModelBase
    {
        private int _interval;
        private DateTime _timeStart;
        private DispatcherTimer _timer;
        private ObservableCollection<EntityGeoCod> _collection;
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
        public void Init(ObservableCollection<EntityGeoCod> collection, int interval = 1)
        {
            _interval = interval;
            _collection = collection;
            Statistics = new Statistics();
            GetStatColl();

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
        }
        public void Stop()
        {
            _timer.Stop();
        }
        private void GetStat(object sender, EventArgs e)
        {
            GetStatColl();
            _statistics.TimeGeoCod = TimeSpan.FromSeconds((DateTime.Now-_timeStart).TotalSeconds);
        }
        private void GetStatColl()
        {
            _statistics.AllEntity = _collection.Count;
            _statistics.OK = _collection.Count(x => x.Status == StatusType.OK);
            _statistics.NotGeoCoding = _collection.Count(x => x.Status == StatusType.NotGeoCoding);
            _statistics.GeoCodingNow = _collection.Count(x => x.Status == StatusType.GeoCodingNow);
            _statistics.Error = _collection.Count(x => x.Status == StatusType.Error);
            _statistics.House = _collection.Count(x => x.Kind == KindType.House);
            _statistics.Exact = _collection.Count(x => x.Precision == PrecisionType.Exact);
            _statistics.NotFound = _collection.Count(x => x.CountResult == 0);
        }
    }
}