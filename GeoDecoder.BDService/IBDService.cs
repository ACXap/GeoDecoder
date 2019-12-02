using GeoCoding.Entities;
using System;
using System.Collections.Generic;

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
        /// <param name="callback">Функция обратного вызова,с параметрами: множество объектов, ошибка</param>
        /// <param name="conSettings">Свойства подключения</param>
        /// <param name="query">Пользовательский запрос</param>
        void ExecuteUserQuery(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettingsDb conSettings, string query);
        /// <summary>
        /// Метод для проверки соединения с базой данных 
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: ошибка</param>
        /// <param name="conSettings">Свойства подключения</param>
        void ConnectBD(Action<Exception> callback, ConnectionSettingsDb conSettings);
    }
}