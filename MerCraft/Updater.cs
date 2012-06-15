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

        public static bool UpToDate()
        {

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
            DownloadUpdate(new Uri("http://173.48.92.80/MerCraft/ModPack.zip"), appdata + "\\.mercraft\\Update.zip");
        }
        private void DownloadUpdate(Uri DownloadFile, string SaveFile)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(DownloadFile, SaveFile);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            LF.progressBar1.Value = e.ProgressPercentage;
        }

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
