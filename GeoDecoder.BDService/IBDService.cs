using System;
using System.Collections.Generic;

namespace GeoCoding.BDService
{
    public interface IBDService
    {
        void GetAddress(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettings conSettings);
        void ExecuteUserQuery(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettings conSettings, string query);
        void ConnectBD(Action<Exception> callback, ConnectionSettings conSettings);
    }
}