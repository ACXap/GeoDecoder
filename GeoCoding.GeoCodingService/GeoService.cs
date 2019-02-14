using System;
using System.IO;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    public abstract class GeoService
    {
        /// <summary>
        /// Сообщение по поводу превышения лимита в сутки
        /// </summary>
        protected virtual string _textLimit => "Ваш лимит исчерпан";
        /// <summary>
        /// Текст сообщения о пустом адресе
        /// </summary>
        protected virtual string _textAddressEmpty => "Значение адреса пусто";

        /// <summary>
        /// Ошибка при привышении лимита в сутки
        /// </summary>
        protected virtual string _errorWebRequestLimit { get; }
       
        /// <summary>
        /// Ссылка на геокодер
        /// </summary>
        protected virtual string _url { get; }

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public virtual string Name => "Имя сервиса";

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public virtual string GetUrlRequest(string address)
        {
            return $"{_url}{address}";
        }

        /// <summary>
        /// Метод для получения json ответа от геосервиса
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметроми строка, ошибка</param>
        /// <param name="address">Строка адреса для поиска координат</param>
        protected virtual void GetJsonString(Action<string, Exception> callback, string address)
        {
            Exception error = null;
            string json = string.Empty;
            string url = GetUrlRequest(address);
            string a = _errorWebRequestLimit;

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("Content-Encoding: gzip, deflate, br");
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                json = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Message == _errorWebRequestLimit)
                {
                    error = new Exception(_textLimit, wex);
                }
                else
                {
                    error = wex;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(json, error);
        }

        /// <summary>
        /// Метод преобразования json в объекты
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметроми объект, ошибка</param>
        /// <param name="json">Строка json</param>
        protected abstract void ParserJson(Action<GeoCod, Exception> callback, string json);

        /// <summary>
        /// Метод для получения геоокординат по адресу
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: объект, ошибка</param>
        /// <param name="address">Строка адреса для поиска</param>
        public virtual void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            Exception error = null;
            GeoCod geocod = null;

            if (!string.IsNullOrEmpty(address))
            {
                GetJsonString((s, e) =>
                {
                    error = e;
                    if (e == null && !string.IsNullOrEmpty(s))
                    {
                        ParserJson((g, er) =>
                        {
                            error = er;
                            if (error == null)
                            {
                                geocod = g;
                            }
                        }, s);
                    }
                }, address);
            }
            else
            {
                error = new ArgumentNullException(_textAddressEmpty);
            }

            callback(geocod, error);
        }
    }
}