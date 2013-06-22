using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace MerCraft
{
    class Launcher
    {
        public static string path;

        /// <summary>
        /// Get the filesname of the java process.
        /// </summary>
        /// <param name="debug">Whether or not to launch in debug mode.</param>
        /// <returns>The filename of the java process.</returns>
        private static string JavaProcessFileName(bool debug = false)
        {
            if (debug)
                path = Path.Combine(JavaDetect.JavaPath.GetJavaBinaryPath(), "java.exe");
            else
                path = Path.Combine(JavaDetect.JavaPath.GetJavaBinaryPath(), "javaw.exe");

            return path;
        }

        /// <summary>
        /// Get the process of java, with arguments.
        /// </summary>
        /// <param name="U">Passed username.</param>
        /// <param name="P">Passed password.</param>
        /// <param name="debug">Whther or not we're in debug mode.</param>
        /// <returns>A Process for launching, runs to Java.</returns>
        private static Process GetJavaProcess(string U, string P, bool debug = false)
        {
            Process Java = new Process();

            string JPath = debug ? JavaProcessFileName(true) : JavaProcessFileName(false);

            if (JPath != "")
                Java.StartInfo.FileName = JPath;
            else
                return null;

            //string MaxRam = Environment.Is64BitOperatingSystem ? "-Xmx1024M" : "-Xmx256M";

            Java.StartInfo.Arguments = 
                "-Xmx" + Program.M.Opts.Config.GetConfigVarString("MaxHeap") + " " +
                "-Xincgc " +
                "-Xms" + Program.M.Opts.Config.GetConfigVarString("InitHeap") + " " +
                "-cp \"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl_util.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\jinput.jar\" " +
                "-Djava.library.path=\"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\natives\" " +
                "net.minecraft.client.Minecraft " +
                U + " " + P;

            Java.StartInfo.EnvironmentVariables.Remove("APPDATA");
            Java.StartInfo.EnvironmentVariables.Add("APPDATA", Updater.appdata + "\\.mercraft\\ModPack");

            Java.StartInfo.UseShellExecute = false;

            return Java;
        }

        /// <summary>
        /// Launch the game normally.
        /// </summary>
        /// <param name="U">Username.</param>
        /// <param name="P">Password.</param>
        public static void Launch(string U, string P)
        {
            Run(U, P, null);
        }

        /// <summary>
        /// Launch the game after an update.
        /// </summary>
        /// <param name="LF">The LaunchForm.</param>
        /// <param name="U">Username.</param>
        /// <param name="P">Password.</param>
        public static void LaunchAfterUpdate(LaunchForm LF, string U, string P)
        {
            Run(U, P, LF);
        }

        /// <summary>
        /// Runs mercraft and compresses all the windows together.
        /// </summary>
        /// <param name="U">Username.</param>
        /// <param name="P">Password.</param>
        /// <param name="LF">LaunchForm if we updated, null if not.</param>
        /// <returns>Successful</returns>
        static bool Run(string U, string P, LaunchForm LF = null)
        {
            IntPtr originalHandle;
            IntPtr mainHandle = IntPtr.Zero;
            try
            {
                Process Java = null;
                if (LF != null)
                {
                    //LF.lblCurrentAction.Text = "Opening MerCraft...";

                    LF.Close();
                }
                Java = Program.M.Opts.Config.GetConfigVarBool("Debug") ? GetJavaProcess(U, P, true) : GetJavaProcess(U, P, false);
                Java.Start();
                if (Program.M.Opts.Config.GetConfigVarBool("WinAPI"))
                {
                    while (mainHandle == IntPtr.Zero)
                    {
                        Java.WaitForInputIdle(10);
                        Java.Refresh();

                        if (Java.HasExited)
                            return false;
                        if (Java.MainWindowTitle != "Minecraft")
                            continue;
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
                    gameForm.javaProcess = Java;
                    gameForm.javaProcess.Exited += gameForm.javaProcess_Exited;
                    gameForm.hasTriedHandle = true;
                    Program.M.Hide();
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (InvalidOperationException)
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
