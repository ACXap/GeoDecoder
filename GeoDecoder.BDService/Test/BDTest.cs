using GeoCoding.Entities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GeoCoding.BDService
{
    public class BDTest : IBDService
    {
        public void ConnectBD(Action<Exception> callback, ConnectionSettingsDb conSettings)
        {
            throw new NotImplementedException();
        }

        public void ExecuteUserQuery(Action<IEnumerable<Entity>, Exception> callback, ConnectionSettingsDb conSettings, string query)
        {
            Thread.Sleep(int.Parse(query));
            callback(new List<Entity>() { new Entity() { Address="234", OrponId=1}, new Entity() { Address = "34", OrponId = 1 }, new Entity() { Address = "34", OrponId = 1 } }, null);
        }
    }
}