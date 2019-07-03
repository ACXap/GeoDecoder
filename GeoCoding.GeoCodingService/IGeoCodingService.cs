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
    }
}