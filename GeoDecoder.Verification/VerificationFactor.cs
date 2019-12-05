// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.VerificationService
{
    public class VerificationFactor : IVerificationService
    {
        private string _url;
        private string _testAddress = "Новсоибирск, улица Зорге";

        public void CheckServiceVerification(Action<Exception> callback)
        {
            Exception error = null;
            string responseFromServer = string.Empty;

            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(new Uri(_url));
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes($"{{\"query\":\"{_testAddress}\",\"count\":2}}");
                request.ContentType = "application/json";
                request.Headers.Add("Content-Encoding: gzip, deflate, br");
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (WebResponse response = request.GetResponse())
                {
                    dataStream = response.GetResponseStream();
                    using(StreamReader reader = new StreamReader(dataStream))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                    dataStream.Close();
                    response.Close();
                }

                if(string.IsNullOrEmpty(responseFromServer))
                {
                    error = new Exception("Ответ сервера пустота");
                }

            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        public void GetId(Action<Exception> callback, IEnumerable<EntityForCompare> data)
        {
            throw new NotImplementedException();
        }

        public void SettingsService(Action<Exception> callback, string connectSettings)
        {
            Exception error = null;
            _url = connectSettings;

            callback(error);
        }

        public VerificationFactor(string connectionSettings)
        {
            _url = connectionSettings;
        }
    }
}