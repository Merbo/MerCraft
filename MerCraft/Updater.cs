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
        public string Us, Pa, currentDownload, downloadDestination;
        public bool runningDownload;
        DateTime now;

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
                    Reader.Dispose();
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
        public static bool UpToDate()
        {
            if (Options.SMPChanged || Options.SoundChanged)
            {
                Options.SMPChanged = false;
                Options.SoundChanged = false;
                return false;
            }

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://173.48.92.80/MerCraft/Default.aspx");
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

            if (!File.Exists(appdata + "\\.mercraft\\version"))
                File.WriteAllText(appdata + "\\.mercraft\\version", ServerVersion);
            else
            {
                string ClientVersion = File.ReadAllText(appdata + "\\.mercraft\\version");

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
        /// <param name="U">Username</param>
        /// <param name="P">Password</param>
        public async void UpdateVersion(string U, string P)
        {
            UpdateInfoControl UICModPack = new UpdateInfoControl("ModPack Update Download");
            UpdateInfoControl UICSoundPack = new UpdateInfoControl("Music SoundPack Update Download");    

            LF = new LaunchForm();
            Us = U;
            Pa = P;          
            LF.panel1.Controls.Add(UICModPack);
            LF.Show();
            
            string linkToModPack = Options.SMP ?
                "http://173.48.92.80/MerCraft/ModPack.zip" :
                "http://173.48.92.80/MerCraft/ModPackSSP.zip";
            await UICModPack.DownloadAndExtractZip(linkToModPack, appdata + "\\.mercraft\\ModPack.zip", appdata + "\\.mercraft", true);

            if (Options.Sound)
            {
                LF.panel1.Controls.Remove(UICModPack);
                LF.panel1.Controls.Add(UICSoundPack);
                string linkToSoundPack = "http://173.48.92.80/MerCraft/SoundPack.zip";
                await UICSoundPack.DownloadAndExtractZip(linkToSoundPack, appdata + "\\.mercraft\\SoundPack.zip", appdata + "\\.mercraft\\ModPack\\.minecraft\\audiotori", true);
            }

            if (!runningDownload)
                if (CorrectJar())
                    Launcher.LaunchAfterUpdate(LF, Us, Pa);
        }
    }
}
