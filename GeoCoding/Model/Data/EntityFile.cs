﻿using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class EntityFile :ViewModelBase
    {
        private string _nameFile = string.Empty;
        /// <summary>
        /// 
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
    }
}