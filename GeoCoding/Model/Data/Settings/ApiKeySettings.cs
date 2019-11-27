using GalaSoft.MvvmLight;
using System;
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
                var l = _currentKey.CollectionDayWeekSettings.FirstOrDefault(x => x.Day == DateTime.Now.DayOfWeek && x.Selected)?.MaxCount;
                _currentKey.CurrentLimit = l!=null ? (int)l : 0;
            }
        }

        private ObservableCollection<EntityApiKey> _collectionApiKeys;
        /// <summary>
        /// Коллекция апи-ключей
        /// </summary>
        public ObservableCollection<EntityApiKey> CollectionApiKeys
        {
            get => _collectionApiKeys;
            set => Set(ref _collectionApiKeys, value);
        }
    }
}