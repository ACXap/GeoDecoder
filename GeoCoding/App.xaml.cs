using GalaSoft.MvvmLight.Threading;
using System.Windows;
using System.Linq;
using System.Configuration;

namespace GeoCoding
{
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var arg = e.Args;
            if (arg.Length > 0)
            {
                var comp = arg.Count(x => x == "-c" || x == "-C");
                if(comp>0)
                {
                    StartCompact();
                    return;
                }
            }

            try
            {
                var a = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).SectionGroups.Get("userSettings").Sections[1] as System.Configuration.ClientSettingsSection;
                var b = bool.Parse(a.Settings.Get("CanStartCompact").Value.ValueXml.InnerText);
                if(b)
                {
                    StartCompact();
                    return;
                }
            }
            catch
            {

            }
            

            MainWindow win = new MainWindow
            {
                Height = GetHeight(),
                Width = GetWidth()
            };
            win.Show();
        }

        private void StartCompact()
        {
            CompactMainWindow cwin = new CompactMainWindow()
            {
                Height = 450,
                Width = 700
            };

            cwin.Show();
        }

        private double GetHeight()
        {
            return SystemParameters.PrimaryScreenHeight * 0.60;
        }

        private double GetWidth()
        {
            return SystemParameters.PrimaryScreenWidth * 0.55;
        }
    }
}