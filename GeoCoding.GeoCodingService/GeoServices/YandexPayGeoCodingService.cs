using GeoCoding.GeoCodingService.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.GeoCodingService
{
    public class YandexPayGeoCodingService: YandexGeoCodingService
    {
        private string _key;
        private string _keyFile = "key";
        
        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "YANDEXPAY";

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address)
        {
            return $"{_url}{address}&apikey={_key}&format=json";
        }

        public YandexPayGeoCodingService()
        {
           var str = File.ReadAllText(_keyFile);

            ProtectedDataDPAPI.DecryptData((s, e) =>
            {
                _key = s;
            }, str);
        }
    }
}