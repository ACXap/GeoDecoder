// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight;

namespace GeoCoding
{
    /// <summary>
    /// Класс для хранения прокси сервера
    /// </summary>
    public class ProxyEntity : ViewModelBase
    {
        private string _address = string.Empty;
        /// <summary>
        /// Аддрес прокси сервера
        /// </summary>
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        private int _port = 0;
        /// <summary>
        /// Порт прокси сервера
        /// </summary>
        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private int _delay = 0;
        /// <summary>
        /// Задержка выполнения запроса через прокси
        /// </summary>
        public int Delay
        {
            get => _delay;
            set => Set(ref _delay, value);
        }

        private bool _isActive = true;
        /// <summary>
        /// Активен ли прокси сервер
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        private string _error = string.Empty;
        /// <summary>
        /// Ошибка
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }

        private StatusType _status = StatusType.NotProcessed;
        /// <summary>
        /// Статус подключения
        /// </summary>
        public StatusType Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
    }
}