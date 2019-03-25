using GalaSoft.MvvmLight;
using MahApps.Metro;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GeoCoding
{
    public class GeneralSettings : ViewModelBase
    {
        private bool _canUseVerificationModule = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanUseVerificationModule
        {
            get => _canUseVerificationModule;
            set => Set(ref _canUseVerificationModule, value);
        }

        private bool _canUseBdModule = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanUseBdModule
        {
            get => _canUseBdModule;
            set => Set(ref _canUseBdModule, value);
        }

        private bool _canUseFtpModule = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanUseFtpModule
        {
            get => _canUseFtpModule;
            set => Set(ref _canUseFtpModule, value);
        }

        private bool _canStartCompact = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanStartCompact
        {
            get => _canStartCompact;
            set => Set(ref _canStartCompact, value);
        }

        private string _colorTheme = "Light.Blue";
        /// <summary>
        /// 
        /// </summary>
        public string ColorTheme
        {
            get => _colorTheme;
            set
            {
                Set(ref _colorTheme, value);
                ThemeManager.ChangeTheme(Application.Current, value);
            }
        }

        /// <summary>
        /// Коллекция всех возможных тем оформления окна
        /// </summary>
        public List<string> ListTheme => ThemeManager.Themes.Select(x=>x.Name).OrderBy(x=>x).ToList();
    }
}