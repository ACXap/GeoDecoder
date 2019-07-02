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


        private string _verificationServerFactor = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string VerificationServerFactor
        {
            get => _verificationServerFactor;
            set => Set(ref _verificationServerFactor, value);
        }
    }
}