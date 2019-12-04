using GalaSoft.MvvmLight;
using Newtonsoft.Json;
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
            set =>Set(ref _currentKey, value);
        }

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