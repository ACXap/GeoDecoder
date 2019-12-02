using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.Entities
{
    /// <summary>
    /// Класс для хранения настроек подключения
    /// </summary>
    public class ConnectionSettingsDb
    {
        /// <summary>
        /// Сервер для подключения
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Порт подключения
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Имя базы данных для подключения
        /// </summary>
        public string BDName { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// пароль
        /// </summary>
        public string Password { get; set; }
    }
}