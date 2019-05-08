using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    /// <summary>
    /// Абстрактный класс для реализации основного функционала геокодирования, реализует интерфейс IGeoCodingService
    /// </summary>
    public abstract class GeoService : IGeoCodingService
    {
        #region ProtectedFields
        /// <summary>
        /// Сообщение по поводу превышения лимита в сутки
        /// </summary>
        protected virtual string _textLimit => "Ваш лимит исчерпан";
        /// <summary>
        /// Сообщение по поводу превышения времени ожидания
        /// </summary>
        protected virtual string _textTimeIsUp => "Скорее всего упал сайт";
        /// <summary>
        /// Текст сообщения о пустом адресе
        /// </summary>
        protected virtual string _textAddressEmpty => "Значение адреса пусто";
        /// <summary>
        /// Текст ошибки, если запрос вернул пустоту
        /// </summary>
        protected virtual string _textJsonEmpty => "Запрос вернул пустоту";
        #endregion ProtectedFields

        /// <summary>
        /// Ошибка при привышении лимита в сутки
        /// </summary>
        protected virtual string _errorWebRequestLimit { get; }
        /// <summary>
        /// Ошибка если привешено время ожидания (скорее всего сайт упал)
        /// </summary>
        protected virtual string _errorWebRequestTimeIsUp { get; }

        /// <summary>
        /// Ссылка на геокодер
        /// </summary>
        protected virtual string _url { get; }

        /// <summary>
        /// Имя геосервиса
        /// </summary>
        public virtual string Name { get; }

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
        protected virtual void GetJsonString(Action<string, Exception> callback, string address, ConnectSettings cs)
        {
            Exception error = null;
            string json = string.Empty;
            string url = GetUrlRequest(address);

            try
            {
                ServicePointManager.DefaultConnectionLimit = 100;

                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("Content-Encoding: gzip, deflate, br");

                if(cs.ProxyType == ProxyType.System)
                {
                    request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else if(cs.ProxyType == ProxyType.None)
                {
                    request.Proxy = null;
                }
                else
                {
                    WebProxy proxy = new WebProxy(cs.ProxyAddress, cs.ProxyPort);
                    request.Proxy = proxy;
                }
                
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
                else if (wex.Response != null && wex.Response is HttpWebResponse)
                {
                    var a = (HttpWebResponse)wex.Response;
                    if (a.StatusCode == HttpStatusCode.GatewayTimeout)
                    {
                        error = new Exception(_textTimeIsUp, wex);
                    }
                    else
                    {
                        error = wex;
                    }
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
        protected abstract void ParserJson(Action<IEnumerable<GeoCod>, Exception> callback, string json);

        /// <summary>
        /// Метод для получения геоокординат по адресу
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: объект, ошибка</param>
        /// <param name="address">Строка адреса для поиска</param>
        public virtual void GetGeoCod(Action<IEnumerable<GeoCod>, Exception> callback, string address, ConnectSettings cs)
        {
            Exception error = null;
            IEnumerable<GeoCod> data = null;

            if (!string.IsNullOrEmpty(address))
            {
                GetJsonString((s, e) =>
                {
                    error = e;
                    if (e == null)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            ParserJson((d, er) =>
                            {
                                error = er;
                                if (error == null)
                                {
                                    data = d;
                                }
                            }, s);
                        }
                        else
                        {
                            error = new Exception(_textJsonEmpty);
                        }
                    }
                }, address, cs);
            }
            else
            {
                error = new ArgumentNullException(_textAddressEmpty);
            }

            callback(data, error);
        }
    }
}