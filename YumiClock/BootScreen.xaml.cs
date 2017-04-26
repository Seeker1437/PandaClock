using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using YumiClock.Core;

namespace YumiClock
{
    /// <summary>
    /// Interaction logic for BootScreen.xaml
    /// </summary>
    public partial class BootScreen : Window
    {
        private static readonly string AssemblyLocation = Assembly.GetExecutingAssembly().Location;
        private static readonly string Assemblypath = Path.GetDirectoryName(AssemblyLocation);

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string),
            typeof(BootScreen), new PropertyMetadata((object)null));

        public static readonly DependencyProperty StatusIsVisibleProperty =
            DependencyProperty.Register("StatusIsVisible", typeof(bool), typeof(BootScreen),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress",
            typeof(double), typeof(BootScreen), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ProgressIsVisibleProperty =
            DependencyProperty.Register("ProgressIsVisible", typeof(bool), typeof(BootScreen),
                new PropertyMetadata(false));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public bool StatusIsVisible
        {
            get { return (bool)GetValue(StatusIsVisibleProperty); }
            set { SetValue(StatusIsVisibleProperty, value); }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public bool ProgressIsVisible
        {
            get { return (bool)GetValue(ProgressIsVisibleProperty); }
            set { SetValue(ProgressIsVisibleProperty, value); }
        }

        private Updater _updater;
        private Dictionary<string, string> _i;

        public BootScreen()
        {
            InitializeComponent();
        }

        public void SetProgressVisibility(bool value)
        {
            Dispatcher.Invoke((Action)(() => ProgressIsVisible = value), DispatcherPriority.Send);
        }

        public void SetStatusVisibility(bool value)
        {
            Dispatcher.Invoke((Action)(() => StatusIsVisible = value), DispatcherPriority.Send);
        }

        public void SetProgressValue(double value)
        {
            Dispatcher.Invoke((Action)(() => Progress = value), DispatcherPriority.Send);
        }

        public void SetStatusText(string text)
        {
            Dispatcher.Invoke((Action)(() => Status = text), DispatcherPriority.Send);
        }

        public void StartApplication()
        {
            Dispatcher.Invoke(() => StartCloseTimer(false));
        }

        private void UpdateAndRestart()
        {
            Dispatcher.Invoke(() => StartCloseTimer(true));
        }

        private void StartCloseTimer(bool update = false)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3.0)
            };
            if (update)
                dispatcherTimer.Tick += UpdateTick;
            else
                dispatcherTimer.Tick += StartTick;
            dispatcherTimer.Start();
        }

        private void UpdateOnClose(object sender, EventArgs e)
        {
            new Process
            {
                StartInfo = {
          Arguments = $"{_i["File"]} { _i["SHA256"]} { _i["Execute"]}",
          FileName = (Assemblypath + "\\Updater.exe")
        }
            }.Start();
            Environment.Exit(0);
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            DispatcherTimer dispatcherTimer = (DispatcherTimer)sender;
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= UpdateTick;
            Closed += UpdateOnClose;
            Close();
        }

        private void StartTick(object sender, EventArgs e)
        {
            DispatcherTimer dispatcherTimer = (DispatcherTimer)sender;
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= StartTick;
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            Close();
            mainWindow.Show();
        }

        public void RegisterEvents(Updater updater)
        {
            _updater = updater;
            _updater.UpdateCheckComplete += UpdateCheckComplete;
            _updater.DownloadCompleted += DownloadCompleted;
            _updater.DownloadStarted += DownloadStarted;
            _updater.CheckForUpdates();
        }

        private void DownloadStarted()
        {
            _updater.ProgressUpdate += ProgressUpdate;
            SetProgressVisibility(true);
        }

        private void ProgressUpdate(double obj)
        {
            SetProgressValue(obj);
        }

        private void DownloadCompleted(bool value)
        {
            _updater.ProgressUpdate -= ProgressUpdate;
            SetProgressVisibility(false);
            SetProgressValue(0.0);
            if (value)
            {
                SetStatusText("Finalizing update...");
                UpdateAndRestart();
                return;
            }
            SetStatusText("Failed to download update! Skipping update...");
            StartApplication();
        }

        private void UpdateCheckComplete(bool obj, Dictionary<string, string> obj2)
        {
            if (!obj)
            {
                SetStatusText(obj2 == null ? "Failed to fetch update info! Skipping..." : "Running current version.");
                StartApplication();
            }
            else
            {
                SetStatusText("Update found!");
                if (MessageBox.Show(
                    $"A new version of YumiCLock is available for download!\r\n\r\n\tCurrent Version: {obj2["Current"]}\r\n\tUpdated Version: {obj2["Version"]}\r\n\r\nWould you like to update?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    SetStatusText("Starting application...");
                    StartApplication();
                }
                else
                {
                    _i = obj2;
                    SetStatusText("Updating...");
                    _updater.Update();
                }
            }
        }

    }
}
