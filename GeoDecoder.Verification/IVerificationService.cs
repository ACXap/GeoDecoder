﻿using System;
using System.Collections.Generic;

namespace GeoCoding.VerificationService
{
    public interface IVerificationService
    {
        void GetMatch(Action<EntityResultCompare, Exception> callback, EntityForCompare data);
        void GetMatches(Action<IEnumerable<EntityResultCompare>, Exception> callback , IEnumerable<EntityForCompare> data);
        void GetLightMatch(Action<bool, Exception> callback, EntityForCompare data);
        void CheckServiceVerification(Action<bool, Exception> callback);
        void SettingsService(Action<Exception> callback, string connectSettings);
    }
}