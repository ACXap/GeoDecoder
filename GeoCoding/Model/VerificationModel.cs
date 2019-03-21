using GeoCoding.VerificationService;
using System;
using System.Threading.Tasks;

namespace GeoCoding
{
    public class VerificationModel
    {
        private readonly IVerificationService _verification;

        public async void CheckServerAsync(Action<Exception> callback)
        {
            Exception error = null;
            await Task.Factory.StartNew(() =>
            {
                _verification.CheckServiceVerification((r, e) =>
                {
                    if (e != null)
                    {
                        error = e;
                    }
                });

                callback(error);
            });
        }

        public void SettingsService(string connectionSettings)
        {
            _verification.SettingsService(e =>
            {

            }, connectionSettings);
        }

        public VerificationModel(string connectionSettings)
        {
            _verification = new Verification(connectionSettings);
        }
    }
}