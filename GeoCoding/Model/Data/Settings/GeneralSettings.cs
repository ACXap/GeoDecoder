using GalaSoft.MvvmLight;
using MahApps.Metro;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GeoCoding
{
    public class GeneralSettings : ViewModelBase
    {
        private bool _canUseVerificationModule = true;
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

        private bool _backgroundGeo = false;
        /// <summary>
        /// Автоматический режим работы приложения
        /// </summary>
        public bool BackgroundGeo
        {
            get => _backgroundGeo;
            set => Set(ref _backgroundGeo, value);
        }

        private List<DayWeek> _listDayWeek;
        /// <summary>
        /// Список настроек по дням недели
        /// </summary>
        public List<DayWeek> ListDayWeek
        {
            get => _listDayWeek;
            set => Set(ref _listDayWeek, value);
        }
        private string _scpriptBackgroundGeo = string.Empty;
        /// <summary>
        /// Скрипт получения данных из базы для фоновой работы
        /// </summary>
        public string ScpriptBackgroundGeo
        {
            get => _scpriptBackgroundGeo ;
            set => Set(ref _scpriptBackgroundGeo , value);
        }
        private bool _useScriptBackGeo = true;
        /// <summary>
        /// Использовать скрипт запроса к базе данных в фоновом режиме
        /// </summary>
        public bool UseScriptBackGeo
        {
            get => _useScriptBackGeo;
            set => Set(ref _useScriptBackGeo, value);
        }
        

        private string _colorTheme = "Light.Blue";
        /// <summary>
        /// Тема оформления приложения
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