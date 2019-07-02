using System;
using System.Collections.Generic;

namespace GeoCoding.VerificationService
{
    public interface IVerificationService
    {
        void GetId(Action<Exception> callback, IEnumerable<EntityForCompare> data);
        void CheckServiceVerification(Action<Exception> callback);
        void SettingsService(Action<Exception> callback, string connectSettings);
    }
}