using System;
using System.Runtime.InteropServices;
using System.Windows;


namespace YumiClock
{
    /// <summary>
    /// Interaction logic for ExceptionReporter.xaml
    /// </summary>
    public partial class ExceptionReporter : Window
    {
        private readonly Exception _ex;

        public ExceptionReporter(Exception ex)
        {
            _ex = ex;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ExceptionBox.Text =
                $"Date: {DateTime.Now}\r\nOS: {Environment.OSVersion}\r\nApplication Directory: {AppDomain.CurrentDomain.BaseDirectory}\r\nCurrent Directory: {Environment.CurrentDirectory}\r\nSystem Folder: {Environment.SystemDirectory}\r\n.NET Runtime: {RuntimeEnvironment.GetSystemVersion()}\r\nException Details: {_ex}";
        }
    }
}
