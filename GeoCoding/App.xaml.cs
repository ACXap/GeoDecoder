﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GalaSoft.MvvmLight.Threading;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Windows;

namespace GeoCoding
{
    public partial class App : Application
    {
        static App()
        {
            ServicePointManager.DefaultConnectionLimit = 10;
            DispatcherHelper.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var arg = e.Args;
            if (arg.Length > 0)
            {
                var comp = arg.Count(x => x == "-c" || x == "-C");
                if (comp > 0)
                {
                    StartCompact();
                    return;
                }

                comp = arg.Count(x => x == "-b" || x == "-B");
                if (comp > 0)
                {
                    StartBackGeo();
                    return;
                }
            }

            try
            {
                var a = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).SectionGroups.Get("userSettings").Sections[1] as ClientSettingsSection;

                if (bool.Parse(a.Settings.Get("CanStartCompact").Value.ValueXml.InnerText))
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

        private void StartBackGeo()
        {
            BackGeoMainWindow bwin = new BackGeoMainWindow();
            bwin.Show();
        }

        private void StartCompact()
        {
            CompactMainWindow cwin = new CompactMainWindow();
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