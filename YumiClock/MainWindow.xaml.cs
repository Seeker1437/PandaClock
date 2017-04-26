using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace YumiClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer = new DispatcherTimer();

        DoubleAnimation da;

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(string),
            typeof(MainWindow), new PropertyMetadata("00:00"));

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            SetTime();
            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void Timer_Click(object sender, EventArgs e)
        {
            SetTime();
        }

        private void SetTime()
        {
            DateTime d;
            d = DateTime.Now;
            TimeTextBlock.Dispatcher.Invoke(() => Time = $"{d.Hour}:{d.Minute:00}");
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void This_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var settingsView = new SettingsView();
            settingsView.Show();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var opacity = Properties.Settings.Default.Opacity;
            if (opacity < 15)
            {
                MessageBox.Show("You transparency setting is below 15 which can make this application unusable. We have set the lower limit to three and will increase you transparency to 15.", "Notice", MessageBoxButton.OK);
                opacity = 15;
            }
            opacity = opacity == 0 ? opacity : opacity / 100;

            da = new DoubleAnimation();
            da.From = MainGrid.Opacity;
            da.To = opacity;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.AutoReverse = false;
            da.FillBehavior = FillBehavior.Stop;

            da.Completed += (o, d) =>
            {
                MainGrid.Opacity = opacity;
            };

            MainGrid.BeginAnimation(OpacityProperty, da);
        }
    }
}
