// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

        private StatusType _statusConnect = StatusType.NotProcessed;
        /// <summary>
        /// Статус подключения
        /// </summary>
        public StatusType StatusConnect
        {
            get => _statusConnect;
            set => Set(ref _statusConnect, value);
        }
        private string _error = string.Empty;
        /// <summary>
        /// Текст ошибки при подключении
        /// </summary>
        public string Error
        {
            get => _error;
            set => Set(ref _error, value);
        }


        //private string _verificationServerFactor = string.Empty;
        ///// <summary>
        ///// 
        ///// </summary>
        //public string VerificationServerFactor
        //{
        //    get => _verificationServerFactor;
        //    set => Set(ref _verificationServerFactor, value);
        //}
    }
}