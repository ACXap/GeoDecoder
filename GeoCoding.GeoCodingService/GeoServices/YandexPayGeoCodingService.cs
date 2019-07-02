using GeoCoding.GeoCodingService.Helpers;
using System.IO;

namespace GeoCoding.GeoCodingService
{
    public class YandexPayGeoCodingService: YandexGeoCodingService
    {
        private string _key;
        private string _keyFile = "keyYandex";
        
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
            if(string.IsNullOrEmpty(_key))
            {
                return $"{_url}{address}&format=json";
            }

            return $"{_url}{address}&apikey={_key}&format=json";
        }

        public YandexPayGeoCodingService()
        {
            if(File.Exists(_keyFile))
            {
                var str = File.ReadAllText(_keyFile);

                ProtectedDataDPAPI.DecryptData((s, e) =>
                {
                    _key = s;
                }, str);
            }
        }
    }
}