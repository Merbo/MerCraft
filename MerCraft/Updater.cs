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

using MerCraft.Controls;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace MerCraft
{
    class Updater
    {
        public LaunchForm LF;
        public static string appdata = Environment.GetEnvironmentVariable("APPDATA");
        public string Us, Pa;
        public bool runningDownload;

        public Updater()
        {
            runningDownload = false;
        }

        /// <summary>
        /// Determines if the jar is the correct jar. Blocks thread, pretty bad at the moment.
        /// Needs some revision.
        /// </summary>
        /// <returns>If the jar used is correct.</returns>
        public static bool CorrectJar()
        {
            try
            {
                string Jar = Program.M.Opts.Config.GetConfigVarString("Jar");

                if (File.Exists(appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar"))
                    File.Delete(appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar");
                switch (Jar)
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
            }
            catch (IOException Error)
            {
                Console.WriteLine("IOException in CorrectJar: {0}", Error);
                MessageBox.Show("IO Exception. Something else is probably using MerCraft's core files at the moment." + Environment.NewLine +
                    "Did you leave another copy of MerCraft open?");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if MerCraft ModPack is up to date.
        /// </summary>
        /// <returns>If MerCraft ModPack is up to date.</returns>
        public static bool UpToDate(out string CurrentMCVersion, out double CurrentVersion, out string NewMCVersion, out double NewVersion)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://mercraft.merbo.org/MerCraft/Versions/" + Program.M.PreferredVersion + "/Version.txt");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            var s = myResponse.GetResponseStream();
            StreamReader sr = new StreamReader(s, System.Text.Encoding.UTF8);
            string ServerVersion = sr.ReadToEnd();
            s.Close();
            s.Dispose();
            sr.Close();
            sr.Dispose();
            myResponse.Close();
            myResponse.Dispose();

            if (!Directory.Exists(appdata + "\\.mercraft"))
                Directory.CreateDirectory(appdata + "\\.mercraft");

            string ClientMCVersion = Program.M.Opts.Config.GetConfigVarString("MCVersion");
            double ClientVersion = Program.M.Opts.Config.GetConfigVarDouble("Version");

            if (ClientMCVersion != Program.M.PreferredVersion)
            {
                CurrentMCVersion = ClientMCVersion;
                NewMCVersion = Program.M.PreferredVersion;

                NewVersion = 0;
                CurrentVersion = 0;

                Program.M.Opts.Config.SetConfigVar("MCVersion", Program.M.PreferredVersion);
                return false;
            }
            double S = 0.1;
            double.TryParse(ServerVersion, out S);

            if (ClientVersion >= S)
            {
                NewMCVersion = "";
                CurrentMCVersion = "";

                NewVersion = 0;
                CurrentVersion = 0;

                return true;
            }
            else
            {
                NewMCVersion = "";
                CurrentMCVersion = "";

                NewVersion = S;
                CurrentVersion = ClientVersion;

                Program.M.Opts.Config.SetConfigVar("Version", S);
                return false;
            }
        }

        /// <summary>
        /// Updates MerCraft.
        /// </summary>
        /// <param name="U">Username</param>
        /// <param name="P">Password</param>
        public async void UpdateVersion(string U, string P)
        {
            UpdateInfoControl UICModPack = new UpdateInfoControl("ModPack Update Download (MC " + Program.M.PreferredVersion + ")");

            LF = new LaunchForm();
            Us = U;
            Pa = P;
            LF.panel1.Controls.Add(UICModPack);
            LF.Show();

            string linkToModPack = "http://mercraft.merbo.org/MerCraft/Versions/" + Program.M.PreferredVersion + "/ModPack.zip";

            await MakeBackup();
            await PrepareForUpdate();
            await UICModPack.DownloadAndExtractZip(linkToModPack, appdata + "\\.mercraft\\ModPack.zip", appdata + "\\.mercraft", true);
            await RestoreBackup();

            if (!runningDownload)
                if (CorrectJar())
                    Launcher.LaunchAfterUpdate(LF, Us, Pa);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Always true.</returns>
        public async Task<bool> MakeBackup()
        {
            //Relative to ModPack/.minecraft/
            string[] purgeBlacklist =  {
                "options.txt",
                "optionsof.txt",
                "saves/",
                "stats/",
                "schematics/",
                "screenshots/",
            };

            await Task.Run(() =>
            {
                foreach (string s in purgeBlacklist)
                {
                    if (!Directory.Exists(appdata + "\\.mercraft\\backup"))
                        Directory.CreateDirectory(appdata + "\\.mercraft\\backup");

                    if (s.EndsWith("/"))
                        if (Directory.Exists(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s.Replace("/", "")))
                            FileIO.CopyDirectory(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s.Replace("/", ""), appdata + "\\.mercraft\\backup\\" + s.Replace("/", ""), true);
                        else
                            if (File.Exists(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s))
                                File.Copy(appdata + "\\.mercraft\\ModPack\\.minecraft\\" + s, appdata + "\\.mercraft\\backup\\" + s);
                }
            });


            return true;
        }

        public async Task<bool> PrepareForUpdate()
        {
            await Task.Run(() =>
            {
                if (Directory.Exists(appdata + "\\.mercraft\\ModPack"))
                    Directory.Delete(appdata + "\\.mercraft\\ModPack", true);
            });

            return true;
        }

        public async Task<bool> RestoreBackup()
        {
            return await Task.Run<bool>(() =>
            {
                if (Directory.Exists(appdata + "\\.mercraft\\backup"))
                {
                    FileIO.CopyDirectory(appdata + "\\.mercraft\\backup", appdata + "\\.mercraft\\ModPack\\.minecraft", true);
                    Directory.Delete(appdata + "\\.mercraft\\backup", true);
                    return true;
                }
                else
                    return false;
            });
        }
    }
}
