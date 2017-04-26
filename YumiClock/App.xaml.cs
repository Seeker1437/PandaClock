using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YumiClock.Core;

namespace YumiClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            BootScreen bootScreen = new BootScreen();
            Current.MainWindow = bootScreen;
            bootScreen.StatusIsVisible = true;
            bootScreen.Status = "Initializing...";
            bootScreen.Show();
            Updater updater = new Updater();
            bootScreen.RegisterEvents(updater);
            base.OnStartup(e);
        }
    }
}
