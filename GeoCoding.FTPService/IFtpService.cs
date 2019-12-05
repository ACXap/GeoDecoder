// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace GeoCoding.FTPService
{
    public interface IFtpService
    {
        void ConnectFtp(Action<Exception> callback, ConnectionSettings conSettings);
        void CopyFileOnFtp(Action<Exception> callback, ConnectionSettings conSettings, string path);
    }
}