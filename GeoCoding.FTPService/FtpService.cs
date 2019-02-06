using System;
using System.IO;
using System.Net;

namespace GeoCoding.FTPService
{
    public class FtpService : IFtpService
    {
        public void ConnectFtp(Action<Exception> callback, ConnectionSettings conSettings)
        {
            Exception error = null;
            string data = string.Empty;

            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create($"{conSettings.Server}:{conSettings.Port}");
                ftpRequest.Credentials = new NetworkCredential(conSettings.Login, conSettings.Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(ftpResponse.GetResponseStream()))
                    {
                        data = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }
    }
}