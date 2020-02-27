// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.Entities;

namespace GeoCoding.BDService
{
    /// <summary>
    /// Интерфейс для описания работы с базой данных
    /// </summary>
    public interface IBDService
    {
        /// <summary>
        /// Метод для выполнения пользовательского запроса к базе данных
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="query">Пользовательский запрос</param>
        /// <param name="limitRow">Ограничение выборки устанавливается, если её нет в самом скрипте</param>
        EntityResult<EntityAddress> ExecuteUserQuery(ConnectionSettingsDb conSettings, string query, int limitRow);
        /// <summary>
        /// Метод для проверки соединения с базой данных 
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        EntityResult<bool> ConnectBD(ConnectionSettingsDb conSettings);

        /// <summary>
        /// Метод для получения адресов без координат
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="limitRow">Лимит строк</param>
        EntityResult<EntityAddress> GetNewAddress(ConnectionSettingsDb conSettings, int limitRow);

        /// <summary>
        /// Метод для получения адресов с плохими координатами
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="limitRow">Лимит строк</param>
        EntityResult<EntityAddress> GetBadAddress(ConnectionSettingsDb conSettings, int limitRow);

        /// <summary>
        /// Метод для получения скрипта в текстовом виде по новым адресам
        /// </summary>
        /// <param name="limitRow">Лимит выгружаемых строк</param>
        /// <returns>Текст скрипта</returns>
        string GetSqlTempleteNewAddress(int limitRow);
        /// <summary>
        /// Метод для получения скрипта в текстовом виде по старым некачественным адресам
        /// </summary>
        /// <param name="limitRow">Лимит выгружаемых строк</param>
        /// <returns>Текст скрипта</returns>
        string GetSqlTempleteOldBadAddresss(int limitRow);
    }
}