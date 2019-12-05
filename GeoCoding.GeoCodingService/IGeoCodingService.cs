// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;

namespace GeoCoding.GeoCodingService
{
    public interface IGeoCodingService
    {
        /// <summary>
        /// Название сервиса
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Метод получения геокоординат для одного объекта
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами найденные данные о адресе и ошибка</param>
        /// <param name="address">Искомый адрес</param>
        /// /// <param name="cs">Настройки подключения</param>
        void GetGeoCod(Action<IEnumerable<GeoCod>, Exception> callback, string address, ConnectSettings cs, List<double> polygon);

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        string GetUrlRequest(string address, List<double> polygon);

        /// <summary>
        /// Метод установки ключа API
        /// </summary>
        /// <param name="keyApi"></param>
        void SetKeyApi(string keyApi);

        /// <summary>
        /// Метод просмотра ключа API
        /// </summary>
        /// <returns></returns>
        string GetKeyApi();
    }
}