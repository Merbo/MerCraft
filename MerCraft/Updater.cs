using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace MerCraft
{
    class Updater
    {
        public LaunchForm LF = new LaunchForm();
        public static string appdata = Environment.GetEnvironmentVariable("APPDATA");
        public string Us, Pa;
        DateTime now; 

        /// <summary>
        /// Determines if the jar is the correct jar. Blocks thread, pretty bad at the moment.
        /// Needs some revision.
        /// </summary>
        /// <returns>If the jar used is correct.</returns>
        public static bool CorrectJar()
        {
            string J = "";
            StreamReader Reader = null;
            if (File.Exists(Updater.appdata + "\\.mercraft\\config"))
            {
                Reader = new StreamReader(Updater.appdata + "\\.mercraft\\config");
                J = Reader.ReadLine();
                Reader.Close();
                Reader = null;
            }
            else
            {
                J = "Vanilla";
            }
                    
            if (File.Exists(appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar"))
                File.Delete(appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar");
            switch (J)
            {
                case "Vanilla":
                    File.Copy(appdata + "\\.mercraft\\ModPack\\bin\\MCJars\\minecraft_vanilla.jar", appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar");
                    break;
                case "OptiFine":
                    File.Copy(appdata + "\\.mercraft\\ModPack\\bin\\MCJars\\minecraft_optifine.jar", appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar");
                    break;
                case "Shaders":
                    File.Copy(appdata + "\\.mercraft\\ModPack\\bin\\MCJars\\minecraft_shaders.jar", appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar");
                    break;
            }
            return true;
        }

        /// <summary>
        /// Determines if MerCraft ModPack is up to date.
        /// </summary>
        /// <returns>If MerCraft ModPack is up to date.</returns>
        public static bool UpToDate()
        {

            if (Options.SMPChanged)
            {
                Options.SMPChanged = false;
                return false;
            }


            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://173.48.92.80/MerCraft/Default.aspx");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string ServerVersion = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            if (!Directory.Exists(appdata + "\\.mercraft"))
            {
                Directory.CreateDirectory(appdata + "\\.mercraft");
            }

            if (!File.Exists(appdata + "\\.mercraft\\version"))
            {
                System.IO.File.WriteAllText(appdata + "\\.mercraft\\version", ServerVersion);
            }
            else
            {
                string ClientVersion = System.IO.File.ReadAllText(appdata + "\\.mercraft\\version");

                double C = 0.0;
                double.TryParse(ClientVersion, out C);

                double S = 0.1;
                double.TryParse(ServerVersion, out S);

                if (C >= S)
                {
                    return true;
                }
                else
                {
                    System.IO.File.WriteAllText(appdata + "\\.mercraft\\version", ServerVersion);
                }
            }
            return false;
        }

        /// <summary>
        /// Updates MerCraft.
        /// </summary>
        /// <param name="U">Username to pass forth.</param>
        /// <param name="P">Password to pass forth.</param>
        public void UpdateVersion(string U, string P)
        {
            Us = U;
            Pa = P;
            LF.Show();
            if (Directory.Exists(appdata + "\\.mercraft\\ModPack"))
            {
                LF.lblCurrentAction.Text = "Backing up saves, options, stats, texture packs, and screenshots...";
                
                List<string> BackupFiles = new List<string>();
                List<string> BackupDirs = new List<string>();
                
                BackupFiles.Add("options.txt");
                BackupFiles.Add("optionsof.txt");
                BackupDirs.Add("stats");
                BackupDirs.Add("saves");
                BackupDirs.Add("texturepacks");
                BackupDirs.Add("screenshots");

                foreach (string s in BackupFiles)
                {
                    if (File.Exists(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s))
                    {
                        if (!Directory.Exists(appdata + "\\.mercraft\\backup"))
                        {
                            Directory.CreateDirectory(appdata + "\\.mercraft\\backup");
                        }
                        File.Move(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s, appdata + "\\.mercraft\\backup\\" + s);
                    }
                }
                foreach (string s in BackupDirs)
                {
                    if (Directory.Exists(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s))
                    {
                        Directory.Move(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s, appdata + "\\.mercraft\\backup\\" + s);
                    }
                }

                LF.lblCurrentAction.Text = "Removing old MerCraft installation...";
                Directory.Delete(appdata + "\\.mercraft\\ModPack", true);
            }
            Directory.CreateDirectory(appdata + "\\.mercraft\\ModPack");
            LF.lblCurrentAction.Text = "Downloading Update...";
            //Download the update
            DownloadUpdate((Options.SMP ? new Uri("http://173.48.92.80/MerCraft/ModPack.zip") : new Uri("http://173.48.92.80/MerCraft/ModPackSSP.zip")), appdata + "\\.mercraft\\Update.zip");
        }
        /// <summary>
        /// Downloads a file and saves it.
        /// </summary>
        /// <param name="DownloadFile">Link to the file on the internet.</param>
        /// <param name="SaveFile">Local file save path.</param>
        private void DownloadUpdate(Uri DownloadFile, string SaveFile)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            now = DateTime.Now;
            webClient.DownloadFileAsync(DownloadFile, SaveFile);
        }

        /// <summary>
        /// What happens when the progress of the download changes.
        /// </summary>
        /// <param name="sender">The object that caused this. Should be a WebClient.</param>
        /// <param name="e">Event Arguments.</param>
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            LF.progressBar1.Value = e.ProgressPercentage;
            if (now != null)
            {
                double kbPerSecond = 0d;
                double remainingTimeSeconds = double.PositiveInfinity;
                string retText = "";
                if ((DateTime.Now - now).Seconds > 0)
                {
                    kbPerSecond = (e.BytesReceived / 1000) / (DateTime.Now - now).Seconds;
                }
                if (kbPerSecond > 0)
                    remainingTimeSeconds = (((e.TotalBytesToReceive - e.BytesReceived) / 1000) / kbPerSecond);

                retText += "Now downloading: " + (Options.SMP ? "http://173.48.92.80/MerCraft/ModPack.zip" : "http://173.48.92.80/MerCraft/ModPackSSP.zip") + Environment.NewLine;
                retText += "Download size: " + Math.Round((double)(Convert.ToDouble(e.TotalBytesToReceive) / 1000000), 2) + " MB" + Environment.NewLine;
                retText += "Download progress: " + Math.Round((double)(Convert.ToDouble(e.BytesReceived) / 1000000), 2) + " MB (" + e.ProgressPercentage + "%)" + Environment.NewLine;
                retText += "Download rate: " + (kbPerSecond > 1000 ? (kbPerSecond / 1000) + " MB/s" : kbPerSecond + " KB/s") + Environment.NewLine;
                double remainingTime = double.PositiveInfinity;

                string timeType = " Seconds";

                if (remainingTimeSeconds < 60)
                {
                    remainingTime = remainingTimeSeconds;
                    timeType = " Seconds";
                }
                else if (remainingTimeSeconds > 60 && remainingTimeSeconds < 3600)
                {
                    remainingTime = remainingTimeSeconds / 60;
                    timeType = " Minutes";
                }
                else if (remainingTimeSeconds > 3600 && remainingTimeSeconds < 7200)
                {
                    remainingTime = remainingTimeSeconds / 3600;
                    timeType = " Hours";
                }
                else
                {
                    timeType = " Millenia";
                }
                retText += "Time remaining: ~" + Math.Round(remainingTime, 2) + timeType;

                LF.lblRate.Text = retText;
            }
        }

        /// <summary>
        /// What to do when the download completes.
        /// </summary>
        /// <param name="sender">The object that caused this. Should be a WebClient.</param>
        /// <param name="e">Event Arguments.</param>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            //Extract the update
            LF.lblCurrentAction.Text = "Extracting Update...";
            if (UnZipFile(appdata + "\\.mercraft\\Update.zip") == true)
            {
                //Restore backup
                FileIO.CopyDirectory(appdata + "\\.mercraft\\backup", appdata + "\\.mercraft\\ModPack\\.minecraft", true);

                //Remove backup
                if (Directory.Exists(appdata + "\\.mercraft\\backup"))
                    Directory.Delete(appdata + "\\.mercraft\\backup", true);

                //Remove update
                if (File.Exists(appdata + "\\.mercraft\\Update.zip"))
                    File.Delete(appdata + "\\.mercraft\\Update.zip");

                //Launch the game
                if (CorrectJar())
                    Launcher.LaunchAfterUpdate(LF, Us, Pa);
            }
        }

        /// <summary>
        /// Unzips a file.
        /// </summary>
        /// <param name="InputPathOfZipFile">Path to the file.</param>
        /// <returns>Successful</returns>
        private bool UnZipFile(string InputPathOfZipFile)
        {
            bool ret = true;
            try
            {
                if (File.Exists(InputPathOfZipFile))
                {
                    string baseDirectory = Path.GetDirectoryName(InputPathOfZipFile);

                    using (ZipInputStream ZipStream = new ZipInputStream(File.OpenRead(InputPathOfZipFile)))
                    {
                        ZipEntry theEntry;
                        while ((theEntry = ZipStream.GetNextEntry()) != null)
                        {
                            if (theEntry.IsFile)
                            {
                                if (theEntry.Name != "")
                                {
                                    string strNewFile = @"" + baseDirectory + @"\" + theEntry.Name;
                                    if (File.Exists(strNewFile))
                                    {
                                        continue;
                                    }

                                    using (FileStream streamWriter = File.Create(strNewFile))
                                    {
                                        int size = 2048;
                                        byte[] data = new byte[2048];
                                        while (true)
                                        {
                                            size = ZipStream.Read(data, 0, data.Length);
                                            if (size > 0)
                                                streamWriter.Write(data, 0, size);
                                            else
                                                break;
                                        }
                                        streamWriter.Close();
                                    }
                                }
                            }
                            else if (theEntry.IsDirectory)
                            {
                                string strNewDirectory = @"" + baseDirectory + @"\" + theEntry.Name;
                                if (!Directory.Exists(strNewDirectory))
                                {
                                    Directory.CreateDirectory(strNewDirectory);
                                }
                            }
                        }
                        ZipStream.Close();
                    }
                }
                else
                {
                    MessageBox.Show("File doesn't appear to exist! D:");
                }
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.ToString());
            }
            return ret;
        }  
    }
}
