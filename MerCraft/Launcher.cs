using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MerCraft
{
    class Launcher
    {
        private static Process GetJavaProcess(string U, string P)
        {
            Process Java = new Process();

            string JPath = Path.Combine(JavaDetect.JavaPath.GetJavaBinaryPath(), "java.exe");

            if (JPath != "")
                Java.StartInfo.FileName = JPath;
            else
                return null;

            // TODO: Try to do automatic RAM adjust on computers with more or less then 2 GB RAM.
            Java.StartInfo.Arguments = "" +
                "-Xincgc " +
                "-Xmn256M " +
                "-Xmx2048M " +
                "-cp \"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl_util.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\jinput.jar\" " +
                "-Djava.library.path=\"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\natives\" " +
                "net.minecraft.client.Minecraft " +
                U + " " +
                P + " " + // FIXME: minecraft.jar does not accept passwords, only session IDs, use login.minecraft.net to fetch it.
                "173.48.92.80";

            Java.StartInfo.EnvironmentVariables.Remove("APPDATA");
            Java.StartInfo.EnvironmentVariables.Add("APPDATA", Updater.appdata + "\\.mercraft\\ModPack");

            Java.StartInfo.UseShellExecute = false;

            return Java;
        }
        public static void Launch(string U, string P)
        {
            try
            {
                Process Java = GetJavaProcess(U, P);
                Java.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("You might not have java installed in the right place for MerCraft. Try asking Merbo about it.");
                Console.WriteLine(ex.ToString());
            }
        }
        public static void LaunchAfterUpdate(LaunchForm LF, string U, string P)
        {
            try
            {
                LF.lblCurrentAction.Text = "Opening MerCraft...";
                LF.Close();
                Process Java = GetJavaProcess(U, P);
                Java.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("You might not have java installed in the right place for MerCraft. Try asking Merbo about it.");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
