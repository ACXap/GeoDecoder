using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace GeoCoding
{
    public class EntityFile : ViewModelBase
    {
        private string _nameFile = string.Empty;
        /// <summary>
        /// Имя файла
        /// </summary>
        public string NameFile
        {
            get => _nameFile;
            set => Set(ref _nameFile, value);
        }

        private int _count = 0;
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        private FileType _fileType = FileType.Other;
        /// <summary>
        /// 
        /// </summary>
        public FileType FileType
        {
            get => _fileType;
            set => Set(ref _fileType, value);
        }

        private string _error = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private StatusType _status = StatusType.NotProcessed;
        /// <summary>
        /// 
        /// </summary>
        public StatusType Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private List<EntityGeoCod> _collection;
        /// <summary>
        /// 
        /// </summary>
        public List<EntityGeoCod> Collection
        {
            get => _collection;
            set => Set(ref _collection, value);
        }
    }
}