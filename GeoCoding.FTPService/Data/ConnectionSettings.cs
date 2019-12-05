// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace GeoCoding.FTPService
{
    /// <summary>
    /// Класс для хранения настроек подключения
    /// </summary>
    public class ConnectionSettings
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
        /// Логин
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Папка для загрузки на фтп
        /// </summary>
        public string FolderOutput { get; set; }
        /// <summary>
        /// Папка для скачивания с фтп
        /// </summary>
        public string FolderInput { get; set; }
    }
}