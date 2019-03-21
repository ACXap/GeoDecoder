using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace GeoCoding
{
    public class VerificationViewModel : ViewModelBase
    {
        #region PrivateField

        /// <summary>
        /// Поле для хранения модели работы с серером
        /// </summary>
        private readonly VerificationModel _model;

        /// <summary>
        /// Поле для хранения ссылки на коллекцию данных
        /// </summary>
        private ObservableCollection<EntityForCompare> _collection;

        /// <summary>
        /// Поле для хранения ссылки на представления коллекции
        /// </summary>
        private ICollectionView _customerView;

        /// <summary>
        /// Поле для хранения статуса подключения к серверу
        /// </summary>
        private StatusConnect _status = StatusConnect.NotConnect;

        /// <summary>
        /// Поле для хранения ошибки при подключении к серверу
        /// </summary>
        private string _error = string.Empty;

        /// <summary>
        /// Строка подключения к сереру
        /// </summary>
        private string _connectSettings = string.Empty;

        /// <summary>
        /// Сверять все данные
        /// </summary>
        private bool _canCompareAllData = true;

        /// <summary>
        /// Сверять только точные данные
        /// </summary>
        private bool _canCompareGoodData = false;

        /// <summary>
        /// Поле для хранения ссылки на команду проверки подключения к сереру
        /// </summary>
        private RelayCommand _commandCheckServer;

        #endregion PrivateField

        #region PublicProperties

        /// <summary>
        /// Коллекция данных
        /// </summary>
        public ObservableCollection<EntityForCompare> Collection
        {
            get => _collection;
            set => Set(ref _collection, value);
        }

        /// <summary>
        /// Представление коллекции
        /// </summary>
        public ICollectionView CustomerView
        {
            get => _customerView;
            set => Set(ref _customerView, value);
        }

        /// <summary>
        /// Статус подключения к серверу
        /// </summary>
        public StatusConnect Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        /// <summary>
        /// Ошибка подключения к серверу
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        /// <summary>
        /// Строка подключения к сереру
        /// </summary>
        public string ConnectSettings
        {
            get => _connectSettings;
            set => Set(ref _connectSettings, value);
        }

        /// <summary>
        /// Сверять все данные
        /// </summary>
        public bool CanCompareAllData
        {
            get => _canCompareAllData;
            set
            {
                Set(ref _canCompareAllData, value);
                _customerView.Refresh();
            }
        }

        /// <summary>
        /// Сверять только точные данные
        /// </summary>
        public bool CanCompareGoodData
        {
            get => _canCompareGoodData;
            set => Set(ref _canCompareGoodData, value);
        }

        #endregion PublicProperties

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod

        /// <summary>
        /// Метод для заполнения коллекции данных
        /// </summary>
        /// <param name="collection">Коллекция данных</param>
        public void SetCollection(IEnumerable<EntityGeoCod> collection)
        {
            Collection = new ObservableCollection<EntityForCompare>(collection.Select(x =>
            {
                return new EntityForCompare() { GeoCode = x };
            }));
            CustomerView = new CollectionViewSource { Source = _collection }.View;
            CustomerView.Filter = CustomerFilter;

            CountGood = _collection.Count(x => x.GeoCode.MainGeoCod != null && x.GeoCode.MainGeoCod.Qcode == 1);
        }

        #endregion PublicMethod

        #region PublicCommand

        /// <summary>
        /// Команда проверки подключения к серверу
        /// </summary>
        public RelayCommand CommandCheckServer =>
            _commandCheckServer ?? (_commandCheckServer = new RelayCommand(
            () =>
            {
                _model.SettingsService(_connectSettings);

                Status = StatusConnect.ConnectNow;

                _model.CheckServerAsync(e =>
                {
                    if (e != null)
                    {
                        Error = e.Message;
                        Status = StatusConnect.Error;
                    }
                    else
                    {
                        Error = string.Empty;
                        Status = StatusConnect.OK;
                    }
                });
            }));

        #endregion PublicCommand



        private int _countGood = 0;
        /// <summary>
        /// 
        /// </summary>
        public int CountGood
        {
            get => _countGood;
            set => Set(ref _countGood, value);
        }


        private int _countGoodAfterCompare = 0;
        /// <summary>
        /// 
        /// </summary>
        public int CountGoodAfterCompare
        {
            get => _countGoodAfterCompare;
            set => Set(ref _countGoodAfterCompare, value);
        }

        private RelayCommand _commandCompare;
        public RelayCommand CommandCompare =>
        _commandCompare ?? (_commandCompare = new RelayCommand(
                    () =>
                    {
                        IsStartCompare = true;
                        List<EntityForCompare> list = null;

                        if (_canCompareAllData)
                        {
                            list = _collection.Where(x => x.GeoCode.MainGeoCod != null && (x.GeoCode.MainGeoCod.Qcode == 1 || x.GeoCode.MainGeoCod.Qcode == 2)).ToList();
                        }
                        else if (_canCompareGoodData)
                        {
                            list = _collection.Where(x => x.GeoCode.MainGeoCod != null && x.GeoCode.MainGeoCod.Qcode == 1).ToList();
                        }

                        _model.CompareAsync(e =>
                         {
                             if (e == null)
                             {
                                 CountGoodAfterCompare = _collection.Count(x => x.Qcode == 1);
                                 IsStartCompare = false;
                             }
                             else
                             {

                             }
                         }, list);
                    }));

        private bool CustomerFilter(object item)
        {
            EntityForCompare customer = item as EntityForCompare;
            if(_canCompareAllData)
            {
                return customer.GeoCode.MainGeoCod?.Qcode == 1 || customer.GeoCode.MainGeoCod?.Qcode == 2;
            }
            else
            {
                return customer.GeoCode.MainGeoCod?.Qcode == 1;
            }
            
        }

        private RelayCommand _commandStopCompare;
        public RelayCommand CommandStopCompare =>
        _commandStopCompare ?? (_commandStopCompare = new RelayCommand(
                    () =>
                    {
                        _model.StopCompare();
                    }));


        private bool _isStartCompare = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsStartCompare
        {
            get => _isStartCompare;
            set => Set(ref _isStartCompare, value);
        }

        public VerificationViewModel(string connectionString)
        {
            ConnectSettings = connectionString;
            _model = new VerificationModel(connectionString);
        }
    }
}