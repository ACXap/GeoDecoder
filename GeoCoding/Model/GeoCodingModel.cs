using GeoCoding.GeoCodingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoding
{
    public class GeoCodingModel
    {
        private IGeoCodingService _geoCodingService;

        private CancellationTokenSource _cts;

        /// <summary>
        /// Метод для получения координат объекта
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
        /// <param name="data">Объект для которого нужны координаты</param>
        public async void GetGeoCod(Action<Exception> callback, EntityGeoCod data)
        {
            Exception error = null;

            await Task.Factory.StartNew(() =>
            {
                throw new Exception();
            });

            callback(error);
        }

        /// <summary>
        /// Метод остановки процесса
        /// </summary>
        public void StopGet()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// Метод установки текущего геосериса
        /// </summary>
        /// <param name="geoService">Ссылка на геосервис</param>
        public void SetGeoService(IGeoCodingService geoService)
        {
            _geoCodingService = geoService;
        }
    }
}