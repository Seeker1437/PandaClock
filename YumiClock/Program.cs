using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YumiClock.Util;

namespace YumiClock
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                try
                {
                    Log.Exception((Exception)eventArgs.ExceptionObject, "Fatal error occured in application!");
                    new ExceptionReporter((Exception)eventArgs.ExceptionObject).ShowDialog();
                }
                catch (Exception ex)
                {
                    Environment.Exit(1);
                }
                finally
                {
                    Environment.Exit(1);
                }
            };

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string directoryName = Path.GetDirectoryName(executingAssembly.Location);
            if (!Directory.Exists(directoryName + "\\Archived"))
                Directory.CreateDirectory(directoryName + "\\Archived");
            Log.Archive = directoryName + ".\\Archived";
            Log.LogFile = directoryName + ".\\YumiClock.log";
            Log.Info("Application Start.");

            App.Main();
        }
    }
}
