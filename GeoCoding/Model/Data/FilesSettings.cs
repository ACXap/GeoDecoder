﻿using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения настроек о входном и выходном файле
    /// </summary>
    public class FilesSettings : ViewModelBase
    {
        /// <summary>
        /// Поле для хранения имени файла с данными
        /// </summary>
        private string _fileInput = string.Empty;

        /// <summary>
        /// Поле для хранения имени файла для записи
        /// </summary>
        private string _fileOutput = string.Empty;

        /// <summary>
        /// Поля для хранения значения копировать ли выходной файл на FTP сервер
        /// </summary>
        private bool _canCopyFileOutputToFTP = false;

        /// <summary>
        /// Поле для хранения максимального значения количества строк в выходном файле
        /// </summary>
        private int _maxSizePart = 0;

        /// <summary>
        /// Поле для хранения значения разбивать ли выходной файл по строчно
        /// </summary>
        private bool _canBreakFileOutput = false;

        /// <summary>
        /// Поле для хранения значения расположен ли файл на ФТП-сервере
        /// </summary>
        private bool _isFileInputOnFTP = false

        /// <summary>
        /// Имя файла с данными
        /// </summary>
        public string FileInput
        {
            get => _fileInput;
            set => Set("FileInput", ref _fileInput, value);
        }

        /// <summary>
        /// Имя файла для записи
        /// </summary>
        public string FileOutput
        {
            get => _fileOutput;
            set => Set("FileOutput", ref _fileOutput, value);
        }

        /// <summary>
        /// Копировать ли выходной файл на FTP сервер
        /// </summary>
        public bool CanCopyFileOutputToFtp
        {
            get => _canCopyFileOutputToFTP;
            set => Set("CanCopyFileOutputToFtp", ref _canCopyFileOutputToFTP, value);
        }

        /// <summary>
        /// Максимальное число строк в выходном файле
        /// </summary>
        public int MaxSizePart
        {
            get => _maxSizePart;
            set => Set("MaxSizePart", ref _maxSizePart, value);
        }

        /// <summary>
        /// Разбивать ли выходной файл по строчно
        /// </summary>
        public bool CanBreakFileOutput
        {
            get => _canBreakFileOutput;
            set => Set("CanBreakFileOutput", ref _canBreakFileOutput, value);
        }

        /// <summary>
        /// Расположен ли файл на ФТП-сервере
        /// </summary>
        public bool IsFileInputOnFTP
        {
            get => _isFileInputOnFTP;
            set => Set("IsFileInputOnFTP", ref _isFileInputOnFTP, value);
        }
    }
}
