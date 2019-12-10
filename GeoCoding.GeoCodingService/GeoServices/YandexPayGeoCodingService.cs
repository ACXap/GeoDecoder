// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;

namespace GeoCoding.GeoCodingService
{
    public class YandexPayGeoCodingService: YandexGeoCodingService
    {
        private string _key;
        
        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "YANDEXPAY";

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address, List<double> polygon)
        {
            var box = string.Empty;
            if (polygon != null && polygon.Count == 4)
            {
                box= $"&bbox={DoubleToString(polygon[0])},{DoubleToString(polygon[1])}~{DoubleToString(polygon[2])},{DoubleToString(polygon[3])}&rspn=1";
            }

            if (string.IsNullOrEmpty(_key))
            {
                return $"{_url}{address}&format=json{box}";
            }

            return $"{_url}{address}&apikey={_key}&format=json{box}";
        }

        public override void SetKeyApi(string keyApi)
        {
            _key = keyApi;
        }

        public override string GetKeyApi()
        {
            return _key;
        }
    }
}