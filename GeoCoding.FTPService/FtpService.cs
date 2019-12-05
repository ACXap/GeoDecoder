// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;

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

                using FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                using StreamReader sr = new StreamReader(ftpResponse.GetResponseStream());
                sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        public void CopyFileOnFtp(Action<Exception> callback, ConnectionSettings conSettings, string path)
        {
            Exception error = null;

            try
            {
                string nameFile = GetNewName(path, conSettings);
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(conSettings.Login, conSettings.Password);
                    client.UploadFile($"{conSettings.Server}:{conSettings.Port}{conSettings.FolderOutput}/{nameFile}", "STOR", path);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        private string GetNewName(string nameFile, ConnectionSettings conSettings)
        {
            string name = Path.GetFileName(nameFile);
            string data = string.Empty;

            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create($"{conSettings.Server}:{conSettings.Port}{conSettings.FolderOutput}");
                ftpRequest.Credentials = new NetworkCredential(conSettings.Login, conSettings.Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(ftpResponse.GetResponseStream()))
                    {
                        data = sr.ReadToEnd();
                    }
                }
                List<string> list = data.Split('\n').ToList();
                bool exist = true;
                int i = 1;

                while (exist)
                {
                    var a = list.Count(x => x.Trim('\r') == name);
                    if (a > 0)
                    {
                        name = $"{Path.GetFileNameWithoutExtension(name)}_{i++}{Path.GetExtension(name)}";
                    }
                    else
                    {
                        exist = false;
                    }
                }
            }
            catch 
            {
                name = Path.GetRandomFileName() + Path.GetExtension(name);
            }

            return name;
        }
    }
}