using GalaSoft.MvvmLight;

namespace GeoCoding.Model.Data
{
    public class EntityGeoCoder : ViewModelBase
    {
        private int _id = 0;
        /// <summary>
        /// Идентификатор геокодера
        /// </summary>
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        private string _name = string.Empty;
        /// <summary>
        /// Пользовательское имя геокодера
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        private string _geoCoder = string.Empty;
        /// <summary>
        /// Название геокодера
        /// </summary>
        public string GeoCoder
        {
            get => _geoCoder;
            set => Set(ref _geoCoder, value);
        }
        private string _description = string.Empty;
        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
        private string _key = string.Empty;
        /// <summary>
        /// Апи-ключ
        /// </summary>
        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }
        private bool _isDefault = false;
        /// <summary>
        /// Использовать по умолчанию
        /// </summary>
        public bool IsDefault
        {
            get => _isDefault;
            set => Set(ref _isDefault, value);
        }
    }
}