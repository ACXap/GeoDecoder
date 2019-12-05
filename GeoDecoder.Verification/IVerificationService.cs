// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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