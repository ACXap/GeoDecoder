using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace GeoCoding.Model.Data.Settings
{
    public class ApiKeySettings : ViewModelBase
    {
        private string _file = Directory.GetCurrentDirectory() + "//apikey.dat";
        /// <summary>
        /// Файл для хранения настроек апи-ключей
        /// </summary>
        public string File
        {
            get => _file;
            private set => Set(ref _file, value);
        }

        private EntityApiKey _currentKey;
        /// <summary>
        /// Текущий выбранный апи-ключ
        /// </summary>
        public EntityApiKey CurrentKey
        {
            get => _currentKey;
            set
            {
                Set(ref _currentKey, value);
                if (value != null)
                {
                    if (value.CollectionDayWeekSettings == null)
                    {
                        var listDayWeek = new List<DayWeek>();
                        foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                        {
                            listDayWeek.Add(new DayWeek() { Day = d });
                        }
                        var s = listDayWeek.First(x => x.Day == 0);
                        listDayWeek.Remove(s);
                        listDayWeek.Insert(listDayWeek.Count, s);
                        value.CollectionDayWeekSettings = listDayWeek;
                    }
                    var l = value.CollectionDayWeekSettings.FirstOrDefault(x => x.Day == DateTime.Now.DayOfWeek && x.Selected)?.MaxCount;
                    value.CurrentLimit = l != null ? (int)l : 0;
                }
            }
        }

        //private EntityApiKey _currentKeyUse;
        ///// <summary>
        ///// Текущий выбранный апи-ключ для работы
        ///// </summary>
        //public EntityApiKey CurrentKeyUse
        //{
        //    get => _currentKeyUse;
        //    set
        //    {
        //        Set(ref _currentKeyUse, value);
        //        var l = _currentKeyUse.CollectionDayWeekSettings.FirstOrDefault(x => x.Day == DateTime.Now.DayOfWeek && x.Selected)?.MaxCount;
        //        _currentKeyUse.CurrentLimit = l != null ? (int)l : 0;
        //    }
        //}

        private ObservableCollection<EntityApiKey> _collectionApiKeys;
        /// <summary>
        /// Коллекция апи-ключей
        /// </summary>
        public ObservableCollection<EntityApiKey> CollectionApiKeys
        {
            get => _collectionApiKeys;
            set 
            {
                Set(ref _collectionApiKeys, value);
                if (value != null)
                {
                    CollectionKey = value.Select(x => x.ApiKey).ToList();
                }
            }
        }

        private List<string> _collectionKey;
        public List<string> CollectionKey
        {
            get => _collectionKey;
            set => Set(ref _collectionKey, value);
        }
    }
}