using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.FTPService
{
    public interface IFtpService
    {
        void ConnectFtp(Action<Exception> callback, ConnectionSettings conSettings);
    }
}
