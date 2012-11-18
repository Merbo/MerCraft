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
        //public static string ProgramFiles = Environment.GetEnvironmentVariable("PROGRAMFILES");
        public static string path;
        private static string JavaProcessFileName(bool debug = false)
        {
            if (debug)
                path = Path.Combine(JavaDetect.JavaPath.GetJavaBinaryPath(), "java.exe");
            else
                path = Path.Combine(JavaDetect.JavaPath.GetJavaBinaryPath(), "javaw.exe");

            return path;
        }
        private static Process GetJavaProcess(string U, string P, bool debug = false)
        {
            Process Java = new Process();

            string JPath = debug ? JavaProcessFileName(true) : JavaProcessFileName(false);

            if (JPath != "")
                Java.StartInfo.FileName = JPath;
            else
                return null;

            string MaxRam = Environment.Is64BitOperatingSystem ? "-Xmx2048M " : "-Xmx512M";

            Java.StartInfo.Arguments = 
                MaxRam + " " +
                "-Xincgc " +
                "-Xms256M " +
                "-cp \"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl_util.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\jinput.jar\" " +
                "-Djava.library.path=\"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\natives\" " +
                "net.minecraft.client.Minecraft " +
                U + " " + P;

            Java.StartInfo.EnvironmentVariables.Remove("APPDATA");
            Java.StartInfo.EnvironmentVariables.Add("APPDATA", Updater.appdata + "\\.mercraft\\ModPack");

            Java.StartInfo.UseShellExecute = false;

            return Java;
        }
        public static void Launch(string U, string P)
        {
            Run(U, P, null);
        }
        public static void LaunchAfterUpdate(LaunchForm LF, string U, string P)
        {
            Run(U, P, LF);
        }
        static bool Run(string U, string P, LaunchForm LF = null)
        {
            IntPtr originalHandle;
            IntPtr mainHandle = IntPtr.Zero;
            try
            {
                Process Java = null;
                if (LF != null)
                {
                    LF.lblCurrentAction.Text = "Opening MerCraft...";
                    LF.Close();
                }
                Java = Options.debug ? GetJavaProcess(U, P, true) : GetJavaProcess(U, P, false);
                Java.Start();
                while (mainHandle == IntPtr.Zero)
                {
                    Java.WaitForInputIdle(1 * 1000);
                    Java.Refresh();

                    if (Java.HasExited)
                        return false;
                    if (!Java.MainWindowTitle.Contains("Hello"))
                        mainHandle = Java.MainWindowHandle;
                }

                GameForm gameForm = new GameForm();
                gameForm.Show();
                while (!gameForm.panel1.IsHandleCreated)
                {
                }
                originalHandle = WinAPI.SetParent(mainHandle, gameForm.panel1.Handle);
                int style = WinAPI.GetWindowLong(mainHandle, WinAPI.GWL_STYLE);
                WinAPI.MoveWindow(mainHandle, 0, 0, gameForm.panel1.Width, gameForm.panel1.Height, true);
                WinAPI.SetWindowLong(mainHandle, WinAPI.GWL_STYLE, (style & ~(int)WinAPI.WS.WS_SYSMENU));
                WinAPI.SetWindowLong(mainHandle, WinAPI.GWL_STYLE, (style & ~(int)WinAPI.WS.WS_CAPTION));
                gameForm.childHandle = mainHandle;

                Program.M.Hide(); 
            }
            catch (InvalidOperationException ex)
            {
                //We're in debug!
            }
            catch (Exception ex)
            {
                MessageBox.Show("You might not have java installed in the right place for MerCraft. Try asking Merbo about it.");
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
    }
}
