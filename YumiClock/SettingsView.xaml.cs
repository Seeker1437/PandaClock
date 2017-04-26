using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YumiClock
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                throw new ApplicationException("Unable to get mainwindow... this should NEVER happen.");
            }
            mainWindow.MainGrid.Dispatcher.Invoke(() => TransparnecySlider.Value = mainWindow.MainGrid.Opacity * 100);
        }

        private void TransparnecySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                throw new ApplicationException("Unable to get mainwindow... this should NEVER happen.");
            }

            mainWindow.MainGrid.Dispatcher.Invoke(() =>
            {
                if (!IsLoaded) return;

                if (e.NewValue == 0)
                {
                    mainWindow.MainGrid.Opacity = e.NewValue;
                    return;
                }

                mainWindow.MainGrid.Opacity = e.NewValue / 100;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                throw new ApplicationException("Unable to get mainwindow... this should NEVER happen.");
            }
            mainWindow.MainGrid.Dispatcher.Invoke(() => {
                Properties.Settings.Default.Opacity = mainWindow.MainGrid.Opacity * 100;
                Properties.Settings.Default.Save();
            });

            
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
