using System;
using System.IO;
#if DEBUG
using System.Threading;
using System.Windows;
#endif

namespace YumiClock.Util
{
    [Flags]
    public enum LogLevel
    {
        Info = 0x0001,
        Warning = 0x0002,
        Error = 0x0004,
        Debug = 0x0008,
        Status = 0x0010,
        Exception = 0x0020,
        None = 0x7FFF
    }

    public static class Log
    {
        private static string _logFile;

        public static string Archive { get; set; }

        public static string LogFile
        {
            get { return _logFile; }
            set
            {
                if (value != null)
                {
                    var pathToFile = Path.GetDirectoryName(value);

                    if (pathToFile != null && !Directory.Exists(pathToFile))
                        Directory.CreateDirectory(pathToFile);

                    if (File.Exists(value))
                    {
                        if (Archive != null)
                        {
                            if (!Directory.Exists(Archive))
                                Directory.CreateDirectory(Archive);

                            var time = File.GetCreationTime(value);
                            var archive = Path.Combine(Archive, time.ToString("yyyy-MM-dd_hh-mm"));
                            var archiveFilePath = Path.Combine(archive, Path.GetFileName(value));

                            if (!Directory.Exists(archive))
                                Directory.CreateDirectory(archive);

                            if (File.Exists(archiveFilePath))
                                File.Delete(archiveFilePath);

                            File.Move(value, archiveFilePath);
                        }
                        File.Delete(value);
                    }
                }
                _logFile = value;
            }
        }

        public static void Info(string format, params object[] args)
        {
            WriteLine(LogLevel.Info, format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            WriteLine(LogLevel.Warning, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            WriteLine(LogLevel.Error, format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            WriteLine(LogLevel.Debug, format, args);
        }

        public static void Debug(object obj)
        {
            WriteLine(LogLevel.Debug, obj.ToString());
        }

        public static void Status(string format, params object[] args)
        {
            WriteLine(LogLevel.Status, format, args);
        }

        public static void Exception(Exception ex, string description = null, params object[] args)
        {
            if (description != null)
            {
                WriteLine(LogLevel.Error, description, args);
            }
            WriteLine(LogLevel.Exception, ex.ToString());
#if DEBUG
            var t = new Thread(() =>
            {
                MessageBox.Show(ex.Message, "Error");
            });
            t.Start();
#endif
        }

        private static void WriteLine(LogLevel level, string format, params object[] args)
        {
            Write(level, format + Environment.NewLine, args);
        }

        private static void Write(LogLevel level, string format, params object[] args)
        {
            if (_logFile == null) return;
            using (var file = new StreamWriter(_logFile, true))
            {
                file.Write(DateTime.Now + " ");
                if (level != LogLevel.None)
                    file.Write("[{0}] - ", level);
                file.Write(format, args);
                file.Flush();
            }
        }
    }
}
