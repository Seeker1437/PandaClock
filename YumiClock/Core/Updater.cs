using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using YumiClock.Util;

namespace YumiClock.Core
{
    public class Updater
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundWorker1 = new BackgroundWorker();
        private readonly Dictionary<string, string> _updateInfo = new Dictionary<string, string>();

        // -------------------------------------------------------------

        /// <summary>
        /// Rasied once the update check is complete
        /// </summary>
        public event Action<bool, Dictionary<string, string>> UpdateCheckComplete;
        private void OnUpdateCheckComplete(bool value, Dictionary<string, string> info)
        {
            UpdateCheckComplete.Raise(value, info);
        }

        // -------------------------------------------------------------

        /// <summary>
        /// Rasied on progress update
        /// </summary>
        public event Action<double> ProgressUpdate;
        private void OnProgressChanged(double value)
        {
            ProgressUpdate.Raise(value);
        }

        // -------------------------------------------------------------

        /// <summary>
        /// Rasied when the download has started
        /// </summary>
        public event Action DownloadStarted;
        private void OnDownloadStarted()
        {
            DownloadStarted.Raise();
        }


        // -------------------------------------------------------------

        /// <summary>
        /// Raise once the download has completed or failed
        /// </summary>
        public event Action<bool> DownloadCompleted;
        public void OnDownloadCompleted(bool value)
        {
            DownloadCompleted.Raise(value);
        }

        // -------------------------------------------------------------

        public Updater()
        {
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker1.DoWork += _backgroundWorker1_DoWork;
        }

        public void CheckForUpdates()
        {
            _backgroundWorker1.RunWorkerAsync();
        }

        public void Update()
        {
            if (_updateInfo.Count == 0)
                return;
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                Version version1 = Assembly.GetExecutingAssembly().GetName().Version;
                using (FileReader fileReader = new FileReader(webClient.OpenRead("http://www.imabrokedude.com/yumiclock/version")))
                {
                    foreach (FileReaderLine fileReaderLine in fileReader)
                    {
                        int length;
                        if ((length = fileReaderLine.Value.IndexOf(':')) >= 0)
                            _updateInfo[fileReaderLine.Value.Substring(0, length).Trim()] = fileReaderLine.Value.Substring(length + 1).Trim();
                    }
                }
                _updateInfo["Current"] = version1.ToString();
                Version version2 = new Version(_updateInfo["Version"]);
                var updateAvailable = version1 < version2;
                OnUpdateCheckComplete(updateAvailable, _updateInfo);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "Unable to check for updates.");
                OnUpdateCheckComplete(false, null);
            }
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
            webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            OnDownloadStarted();
            try
            {
                webClient.DownloadFileAsync(new Uri(_updateInfo["Link"]), _updateInfo["File"]);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                OnDownloadCompleted(false);
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnProgressChanged(e.ProgressPercentage);
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            OnDownloadCompleted(true);
        }
    }
}
