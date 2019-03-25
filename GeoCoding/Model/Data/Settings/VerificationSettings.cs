using GalaSoft.MvvmLight;

namespace GeoCoding
{
    public class VerificationSettings : ViewModelBase
    {
        private string _verificationServer = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string VerificationServer
        {
            get => _verificationServer;
            set => Set(ref _verificationServer, value);
        }
    }
}