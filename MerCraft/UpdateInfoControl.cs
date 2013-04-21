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
            var s = File.OpenRead(downloadedZip);
            Unzip(s, outFolder);
            s.Close();
            s.Dispose();

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

        private string SizeToHumanReadable(double size)
        {
            string[] units = { "B", "kB", "MB", "GB", "TB", "PB", "EB" /* , "ZB", "YB" */ };
            int i = 0;
            while(size >= 1024 && i < units.Length - 1)
            {
                size /= 1024;
                i++;
            }
            return string.Format("{0:#.#} {1}", size, units[i]);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Calculations
            var elapsedTime = DateTime.Now - now;

            double FileSize = e.TotalBytesToReceive;
            double Downloaded = e.BytesReceived;
            double ToGo = e.TotalBytesToReceive - e.BytesReceived;
            double PerSecond = 0;
            TimeSpan remainingTime = default(TimeSpan);
            if (elapsedTime.TotalSeconds > 0)
            {
                PerSecond = Downloaded / elapsedTime.TotalSeconds;
            }
            if (PerSecond > 0)
                remainingTime = TimeSpan.FromSeconds(ToGo / PerSecond);

            //Set GUI values
            this.progressBar1.Value = e.ProgressPercentage;
            this.label2.Text = "Now downloading: " + currentDownload;
            this.label3.Text = "to " + downloadDestination;
            this.label4.Text = e.ProgressPercentage + "% complete";
            this.label5.Text = "Size: " + SizeToHumanReadable(FileSize);
            this.label6.Text = "Downloaded: " + SizeToHumanReadable(Downloaded);
            this.label7.Text = "Download Speed: " + SizeToHumanReadable(PerSecond) + "/s";
            this.label8.Text = "Estimated Time Remaining: " + remainingTime.ToHumanReadable();
            this.label9.Text = "Elapsed Time: " + elapsedTime.ToHumanReadable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webClient.CancelAsync();
        }

        private void UpdateInfoControl_Load(object sender, EventArgs e)
        {

        }
    }
}
