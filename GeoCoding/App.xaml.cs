using GalaSoft.MvvmLight.Threading;
using System.Windows;

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
            MainWindow win = new MainWindow
            {
                Height = GetHeight(),
                Width = GetWidth()
            };
            win.Show();
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