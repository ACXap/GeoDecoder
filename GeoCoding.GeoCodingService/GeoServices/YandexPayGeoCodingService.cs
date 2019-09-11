using GeoCoding.GeoCodingService.Helpers;
using System.Collections.Generic;
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
            if(!string.IsNullOrEmpty(keyApi))
            {
                _key = keyApi;

                ProtectedDataDPAPI.EncryptData((s, e) =>
                {
                    File.WriteAllText(_keyFile, s);
                },keyApi);
            }
        }

        public override string GetKeyApi()
        {
            return _key;
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