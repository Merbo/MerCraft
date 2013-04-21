using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace MerCraft
{
    /// <summary>
    /// Control used for updating
    /// </summary>
    public partial class UpdateInfoControl : UserControl
    {
        /// <summary>
        /// Webclient used for downloads
        /// </summary>
        public WebClient webClient;
        private DateTime now;
        private string currentDownload;
        private string downloadDestination;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="downloadName">Caption of download</param>
        public UpdateInfoControl(string downloadName)
        {
            InitializeComponent();
            this.webClient = new WebClient();
            this.webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.DownloadComplete);
            this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.ProgressChanged);
            this.label1.Text = downloadName;
        }

        /// <summary>
        /// Download a zip file and extract
        /// </summary>
        /// <param name="link">URL of zip</param>
        /// <param name="downloadPath">Local URL for download location</param>
        /// <param name="outFolder">Folder to extract to</param>
        /// <param name="deleteZipWhenDone">Delete the zip after extraction?</param>
        /// <returns></returns>
        public async Task<bool> DownloadAndExtractZip(string link, string downloadPath, string outFolder, bool deleteZipWhenDone = true)
        {
            bool ret = true;

            Task<string> downloadedZipTask = Download(link, downloadPath);

            string[] split = outFolder.Split('\\');
            if (split.Length <= 1)
                return false;
            for (int i = 0; i < split.Length; i++)
            {
                string path = "";
                for (int j = 0; j <= i; j++)
                    path += split[j] + "\\";
                if (i <= split.Length - 1)
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
            }

            string downloadedZip = await downloadedZipTask;

            this.label2.Text = "Extracting zip file...";
            Unzip(File.OpenRead(downloadedZip), outFolder);

            if (deleteZipWhenDone)
                File.Delete(downloadedZip);

            return ret;
        }

        /// <summary>
        /// Download a file to a location
        /// </summary>
        /// <param name="link">URL to download</param>
        /// <param name="downloadPath">Local file to save to</param>
        /// <returns></returns>
        public async Task<string> Download(string link, string downloadPath)
        {
            this.currentDownload = link;
            this.downloadDestination = downloadPath;
            string ret = downloadPath;

            //Make sure the path itself exists before we download the file to it
            string[] split = downloadPath.Split('\\');
            List<string> splitL = split.ToList<string>();
            splitL.RemoveAt(splitL.Count - 1);
            split = splitL.ToArray<string>();
            if (split.Length <= 1)
                return null;
            for (int i = 0; i < split.Length; i++)
            {
                string path = "";
                for (int j = 0; j <= i; j++)
                    path += split[j] + "\\";
                if (i <= split.Length - 1)
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
            }

            this.now = DateTime.Now;
            await this.webClient.DownloadFileTaskAsync(new Uri(link), downloadPath);

            if (File.Exists(ret))
                return ret;
            else
                return null;
        }

        private bool Unzip(Stream zipStream, string outFolder)
        {
            ZipInputStream zipInputStream = new ZipInputStream(zipStream);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                string entryFileName = zipEntry.Name.Replace('/', '\\');

                byte[] buffer = new byte[4096];

                string fullZipToPath = Path.Combine(outFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                if (fullZipToPath.EndsWith("\\"))
                    fullZipToPath = fullZipToPath.Remove(fullZipToPath.LastIndexOf('\\'), "\\".Length);

                if (Directory.Exists(fullZipToPath))
                {
                    zipEntry = zipInputStream.GetNextEntry();
                    continue;
                }

                using (FileStream streamWriter = File.Create(fullZipToPath))
                    StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                zipEntry = zipInputStream.GetNextEntry();
            }

            return true;
        }  

        private void DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.label1.Text += " ✓";
                this.label7.Text = "Download complete";
                this.label8.Text = "Estimated Time Remaining: This instant";
            }
            else
            {
                this.label1.Text += " ✗";
                this.label7.Text = "Download cancelled";
                this.label8.Text = "Operation will never complete";
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Calculations
            double elapsedTime_Seconds = (DateTime.Now - now).Seconds;
            double elapsedTime_Minutes = (DateTime.Now - now).Minutes;
            double elapsedTime_Hours = (DateTime.Now - now).Hours;
            double elapsedTime_Days = (DateTime.Now - now).Days;

            if (elapsedTime_Seconds == 0)
                elapsedTime_Minutes++;
            if (elapsedTime_Minutes == 60)
            {
                elapsedTime_Minutes = 0;
                elapsedTime_Hours++;
            }
            if (elapsedTime_Hours == 24)
            {
                elapsedTime_Hours = 0;
                elapsedTime_Days++;
            }

            double KBFileSize = e.TotalBytesToReceive / 1000;
            double MBFileSize = KBFileSize / 1000;
            double KBDownloaded = e.BytesReceived / 1000;
            double MBDownloaded = KBDownloaded / 1000;
            double KBToGo = (e.TotalBytesToReceive - e.BytesReceived) / 1000;
            double MBToGo = KBToGo / 1000;
            double KBPerSecond = 0;
            double MBPerSecond = 0;
            double remainingTime_Seconds = double.PositiveInfinity;
            if (elapsedTime_Seconds > 0 || elapsedTime_Minutes > 0 || elapsedTime_Hours > 0 || elapsedTime_Days > 0)
            {
                double eHours = elapsedTime_Hours + (elapsedTime_Days * 24);
                double eMinutes = elapsedTime_Minutes + (elapsedTime_Hours * 60);
                double eSeconds = elapsedTime_Seconds + (elapsedTime_Minutes * 60);
                KBPerSecond = KBDownloaded / eSeconds;
                MBPerSecond = MBDownloaded / eSeconds;
            }
            if (KBPerSecond > 0)
                remainingTime_Seconds = KBToGo / KBPerSecond;
            double remainingTime_Minutes = remainingTime_Seconds / 60;
            double remainingTime_Hours = remainingTime_Minutes / 60;
            double remainingTime_Days = remainingTime_Hours / 24;

            //Set GUI values
            this.progressBar1.Value = e.ProgressPercentage;
            this.label2.Text = "Now downloading: " + currentDownload;
            this.label3.Text = "To " + downloadDestination;
            this.label4.Text = e.ProgressPercentage + "% Complete";
            this.label5.Text = "Size: " + (MBFileSize >= 1 ?
                Math.Round(MBFileSize, 2) + " MB" : Math.Round(KBFileSize, 2) + " KB");
            this.label6.Text = "Downloaded: " + (MBDownloaded >= 1 ?
                Math.Round(MBDownloaded, 2) + " MB" : Math.Round(KBDownloaded, 2) + " KB");
            this.label7.Text = "Download Speed: " + (MBPerSecond >= 1 ?
                Math.Round(MBPerSecond, 2) + " MB/s" : Math.Round(KBPerSecond, 2) + " KB/s");
            this.label8.Text = "Estimated Time Remaining: " + (remainingTime_Days >= 1 ?
                Math.Round(remainingTime_Days, 2) + " Days" : (remainingTime_Hours >= 1 ?
                Math.Round(remainingTime_Hours, 2) + " Hours" : (remainingTime_Minutes >= 1 ?
                Math.Round(remainingTime_Minutes, 2) + " Minutes" : 
                Math.Round(remainingTime_Seconds, 2) + " Seconds")));
            this.label9.Text = "Elapsed Time: " + (elapsedTime_Days >= 1 ?
                elapsedTime_Days + " Days, " + elapsedTime_Hours + " Hours, " + elapsedTime_Minutes + " Minutes, " + elapsedTime_Seconds + " Seconds" : (elapsedTime_Hours >= 1 ?
                elapsedTime_Hours + " Hours, " + elapsedTime_Minutes + " Minutes, " + elapsedTime_Seconds + " Seconds" : (elapsedTime_Minutes >= 1 ?
                elapsedTime_Minutes + " Minutes, " + elapsedTime_Seconds + " Seconds" : 
                elapsedTime_Seconds + " Seconds")));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webClient.CancelAsync();
        }
    }
}
