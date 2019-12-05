// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace GeoCoding.Model.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityGeoCoder : ViewModelBase
    {
        private int _id = 0;
        /// <summary>
        /// Идентификатор геокодера
        /// </summary>
        [JsonProperty("Id")]
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        private string _name = string.Empty;
        /// <summary>
        /// Пользовательское имя геокодера
        /// </summary>
        [JsonProperty("Name")]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        private string _geoCoder = string.Empty;
        /// <summary>
        /// Название геокодера
        /// </summary>
        [JsonProperty("GeoCoder")]
        public string GeoCoder
        {
            get => _geoCoder;
            set => Set(ref _geoCoder, value);
        }
        private string _description = string.Empty;
        /// <summary>
        /// Описание
        /// </summary>
        [JsonProperty("Description")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
        private string _key = string.Empty;
        /// <summary>
        /// Апи-ключ
        /// </summary>
        [JsonProperty("Key")]
        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }
        private bool _isDefault = false;
        /// <summary>
        /// Использовать по умолчанию
        /// </summary>
        [JsonProperty("IsDefault")]
        public bool IsDefault
        {
            get => _isDefault;
            set => Set(ref _isDefault, value);
        }
    }
}