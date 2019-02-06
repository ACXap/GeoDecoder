using System;

namespace GeoCoding.FTPService
{
    public interface IFtpService
    {
        void ConnectFtp(Action<Exception> callback, ConnectionSettings conSettings);
        void CopyFileOnFtp(Action<Exception> callback, ConnectionSettings conSettings, string path);
    }
}