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
    }
}