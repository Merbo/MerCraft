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
                    J = "Vanilla";

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
            }
            catch (IOException)
            {
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
        public static bool UpToDate()
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://173.48.94.88/MerCraft/Versions/" + Program.M.PreferredVersion + "/Version.txt");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string ServerVersion = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            if (!Directory.Exists(appdata + "\\.mercraft"))
                Directory.CreateDirectory(appdata + "\\.mercraft");

            if (!File.Exists(appdata + "\\.mercraft\\version"))
                File.WriteAllText(appdata + "\\.mercraft\\version", Program.M.PreferredVersion + "|" + ServerVersion);
            else
            {
                string ClientVersion = File.ReadAllText(appdata + "\\.mercraft\\version");

                if (!ClientVersion.Contains('|'))
                    return false;

                string McVersion = ClientVersion.Split('|')[0];
                string LocalVersion = ClientVersion.Split('|')[1];

                if (McVersion != Program.M.PreferredVersion)
                    return false;

                double C = 0.0;
                double.TryParse(LocalVersion, out C);

                double S = 0.1;
                double.TryParse(ServerVersion, out S);

                if (C >= S)
                {
                    return true;
                }
                else
                {
                    System.IO.File.WriteAllText(appdata + "\\.mercraft\\version", Program.M.PreferredVersion + "|" + ServerVersion);
                }
            }
            return false;
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

            string linkToModPack = "http://173.48.94.88/MerCraft/Versions/" + Program.M.PreferredVersion + "/ModPack.zip";

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
