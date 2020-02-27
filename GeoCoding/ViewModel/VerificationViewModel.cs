// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
        /// Сверять все данные
        /// </summary>
        private bool _canCompareAllData = true;

        /// <summary>
        /// Сверять только точные данные
        /// </summary>
        private bool _canCompareGoodData = false;

        /// <summary>
        /// Поле запущена ли процедура сравнения данных
        /// </summary>
        private bool _isStartCompare = false;

        /// <summary>
        /// Количество точных данных
        /// </summary>
        private int _countGood = 0;

        /// <summary>
        /// Количество точных данных после проверки
        /// </summary>
        private int _countGoodAfterCompare = 0;

        /// <summary>
        /// Поле для хранения ссылки на команду зафиксировать изменения данных проверки
        /// </summary>
        private RelayCommand _commandCommitChanges;

        /// <summary>
        /// Поле для хранения ссылки на команду запуска процедуры проверки
        /// </summary>
        private RelayCommand _commandCompare;

        /// <summary>
        /// Поле для хранения ссылки на команду остановки проверки данных
        /// </summary>
        private RelayCommand _commandStopCompare;

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
        /// Сверять все данные
        /// </summary>
        public bool CanCompareAllData
        {
            get => _canCompareAllData;
            set
            {
                Set(ref _canCompareAllData, value);
                _customerView?.Refresh();
            }
        }

        /// <summary>
        /// Запущена ли процедура сравнения данных
        /// </summary>
        public bool IsStartCompare
        {
            get => _isStartCompare;
            set
            {
                var oldValue = _isStartCompare;
                Set(ref _isStartCompare, value);
                RaisePropertyChanged("IsStartCompare", oldValue, value, true);
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

        /// <summary>
        /// Количество точных данных
        /// </summary>
        public int CountGood
        {
            get => _countGood;
            set => Set(ref _countGood, value);
        }

        /// <summary>
        /// Количество точных данных после проверки
        /// </summary>
        public int CountGoodAfterCompare
        {
            get => _countGoodAfterCompare;
            set => Set(ref _countGoodAfterCompare, value);
        }

        #endregion PublicProperties

        #region PrivateMethod

        /// <summary>
        /// Метод фильтрации коллекции
        /// </summary>
        /// <param name="item">Объект фильтрации</param>
        /// <returns>Возвращает истину если объект проходит критерий</returns>
        private bool CustomerFilter(object item)
        {
            EntityForCompare customer = item as EntityForCompare;
            if (_canCompareAllData)
            {
                return customer.GeoCode.MainGeoCod?.Qcode == 1 || customer.GeoCode.MainGeoCod?.Qcode == 2;
            }
            else
            {
                return customer.GeoCode.MainGeoCod?.Qcode == 1;
            }

        }

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
            }).ToList());

            CustomerView = new CollectionViewSource { Source = Collection }.View;
            CustomerView.Filter = CustomerFilter;

             CountGood = _collection.Count(x => x.GeoCode?.MainGeoCod?.Qcode == 1);
             CountGoodAfterCompare = _collection.Count(x => x.Qcode == 1);
             CountErrorAfterCompare = _collection.Count(x => x.Status == StatusType.Error);
        }

        /// <summary>
        /// Метод обновления информации проверки данных
        /// </summary>
        public void Update()
        {
            if (_collection != null && _customerView != null)
            {
                //DispatcherHelper.CheckBeginInvokeOnUI(() => _customerView?.Refresh());
                //CountGood = _collection.Count(x => x.GeoCode?.MainGeoCod?.Qcode == 1);
                CountGoodAfterCompare = _collection.Count(x => x.Qcode == 1);
                CountErrorAfterCompare = _collection.Count(x => x.Status == StatusType.Error);
            }
        }

        #endregion PublicMethod

        #region PublicCommand

        /// <summary>
        /// Команда для фиксации данных после проверки
        /// </summary>
        public RelayCommand CommandCommitChanges =>
        _commandCommitChanges ?? (_commandCommitChanges = new RelayCommand(
                    () =>
                    {
                        var list = _collection.Where(x => x.IsChanges);
                        foreach (var item in list)
                        {
                            if (item.Qcode != 0)
                            {
                                item.GeoCode.MainGeoCod.Qcode = item.Qcode;
                            }
                        }
                    }, () => _collection != null && _collection.Any()));

        /// <summary>
        /// Команда для остановки проверки данных
        /// </summary>
        public RelayCommand CommandStopCompare =>
        _commandStopCompare ?? (_commandStopCompare = new RelayCommand(
                    () =>
                    {
                        _model.StopCompare();
                    }));

        /// <summary>
        /// Команда для запуска процедуры проверки
        /// </summary>
        public RelayCommand CommandCompare =>
        _commandCompare ?? (_commandCompare = new RelayCommand(
                    () =>
                    {
                        IsStartCompare = true;
                        List<EntityForCompare> list = null;

                        if (_canCompareAllData)
                        {
                            list = _collection.Where(x => x.GeoCode.MainGeoCod != null).ToList();
                        }
                        else if (_canCompareGoodData)
                        {
                            list = _collection.Where(x => x.GeoCode.MainGeoCod != null && x.GeoCode.MainGeoCod.Qcode == 1).ToList();
                        }
                        else if (_canCompareErrorCompare)
                        {
                            list = _collection.Where(x => x.Status == StatusType.Error).ToList();
                        }

                        _model.CompareAsync(e =>
                        {
                            if (e == null)
                            {
                                CommandCommitChanges.RaiseCanExecuteChanged();
                                CommandCommitChanges.Execute(true);
                            }
                            else
                            {

                            }

                            var l = _collection.Where(x => x.IsChanges);
                            foreach (var item in l)
                            {
                                if (item.Qcode != 0)
                                {
                                    item.GeoCode.MainGeoCod.Qcode = item.Qcode;
                                }
                            }

                            Update();
                            IsStartCompare = false;
                        }, list);
                    }, () => _collection != null && _collection.Any()));

        #endregion PublicCommand

        private int _countErrorAfterCompare = 0;
        /// <summary>
        /// 
        /// </summary>
        public int CountErrorAfterCompare
        {
            get => _countErrorAfterCompare;
            set => Set(ref _countErrorAfterCompare, value);
        }

        private bool _canCompareErrorCompare = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanCompareErrorCompare
        {
            get => _canCompareErrorCompare;
            set => Set(ref _canCompareErrorCompare, value);
        }

        private VerificationSettings _verificationSettings;
        /// <summary>
        /// 
        /// </summary>
        public VerificationSettings VerificationSettings
        {
            get => _verificationSettings;
            set => Set(ref _verificationSettings, value);
        }

        private bool _canCompareNotGoodData = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanCompareNotGoodData
        {
            get => _canCompareNotGoodData;
            set => Set(ref _canCompareNotGoodData, value);
        }

        public Task<Exception> CheckAll()
        {
            Exception error = null;

            return Task.Factory.StartNew(() =>
            {
                IsStartCompare = true;

                List<EntityForCompare> list = null;

                if (_canCompareAllData)
                {
                    list = _collection.Where(x => x.GeoCode.MainGeoCod != null).ToList();
                }
                else if (_canCompareGoodData)
                {
                    list = _collection.Where(x => x.GeoCode.MainGeoCod != null && x.GeoCode.MainGeoCod.Qcode == 1).ToList();
                }
                else if (_canCompareErrorCompare)
                {
                    _collection.Where(x => x.Status == StatusType.Error).ToList();
                }
                else if (_canCompareNotGoodData)
                {
                    _collection.Where(x => x.GeoCode.MainGeoCod != null && x.GeoCode.MainGeoCod.Qcode == 2).ToList();
                }

                try
                {
                    error = _model.CheckGeo(list);
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                Update();

                var l = _collection.Where(x => x.IsChanges);
                foreach (var item in l)
                {
                    if (item.Qcode != 0)
                    {
                        item.GeoCode.MainGeoCod.Qcode = item.Qcode;
                    }
                }

                IsStartCompare = false;

                return error;
            });
        }

        //private bool _isServerVerFactor = false;
        //public bool IsServerVerFactor
        //{
        //    get => _isServerVerFactor;
        //    set => Set(ref _isServerVerFactor, value);
        //}

        private bool _isServerVerOrpon = true;
        public bool IsServerVerOrpon
        {
            get => _isServerVerOrpon;
            set => Set(ref _isServerVerOrpon, value);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Парамтры подключения к серверу проверки</param>
        public VerificationViewModel(VerificationSettings vset)
        {
            VerificationSettings = vset;
            _model = new VerificationModel(_verificationSettings.VerificationServer);
        }
    }
}