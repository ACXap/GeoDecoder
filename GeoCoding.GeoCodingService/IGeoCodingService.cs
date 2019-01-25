using System;

namespace GeoCoding.GeoCodingService
{
    public interface IGeoCodingService
    {
        /// <summary>
        /// Метод получения геокоординат для обного обхекта
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами найденные данные о адресе и ошибка</param>
        /// <param name="address">Искомый адрес</param>
        void GetGeoCod(Action<GeoCod, Exception> callback, string address);
    }
}