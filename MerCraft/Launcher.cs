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
        public static string ProgramFiles = Environment.GetEnvironmentVariable("PROGRAMFILES");
        public static string path;
        private static string JavaProcessFileName(bool debug = false)
        {
            path = "";

            List<string> PathTries     = new List<string>();
            List<string> PathTries_x86 = new List<string>();


            //Java installation defaults
            PathTries.Add("C:\\Program Files\\Java\\jdk1.7.0_04\\bin");
            PathTries.Add("C:\\Program Files\\Java\\jre7\\bin");
            PathTries.Add("C:\\Program Files\\Java\\jre6\\bin");
            PathTries.Add(ProgramFiles + "\\Java\\jdk1.7.0_04\\bin");
            PathTries.Add(ProgramFiles + "\\Java\\jre7\\bin");
            PathTries.Add(ProgramFiles + "\\Java\\jre6\\bin");
            PathTries.Add("C:\\Progs\\Java\\jdk1.7.0_04\\bin");
            PathTries.Add("C:\\Progs\\Java\\jre7\\bin");
            PathTries.Add("C:\\Progs\\Java\\jre6\\bin");

            //On x64 machines, but a x86 java installation
            PathTries_x86.Add("C:\\Program Files (x86)\\Java\\jdk1.7.0_04\\bin");
            PathTries_x86.Add("C:\\Program Files (x86)\\Java\\jre7\\bin");
            PathTries_x86.Add("C:\\Program Files (x86)\\Java\\jre6\\bin");
            PathTries_x86.Add(ProgramFiles + "\\Java\\jdk1.7.0_04\\bin");
            PathTries_x86.Add(ProgramFiles + "\\Java\\jre7\\bin");
            PathTries_x86.Add(ProgramFiles + "\\Java\\jre6\\bin");
            PathTries_x86.Add("C:\\Progs\\Java\\jdk1.7.0_04\\bin");
            PathTries_x86.Add("C:\\Progs\\Java\\jre7\\bin");
            PathTries_x86.Add("C:\\Progs\\Java\\jre6\\bin");

            foreach (string s in PathTries)
            {
                if (Directory.Exists(s))
                {
                    if (debug && File.Exists(s + "\\java.exe"))
                        path = s + "\\java.exe";
                    else if (!debug && File.Exists(s + "\\javaw.exe"))
                        path = s + "\\javaw.exe";
                    else continue;
                }
            }

            if (path == "")
            {
                foreach (string s in PathTries_x86)
                {
                    if (Directory.Exists(s))
                    {
                        if (debug && File.Exists(s + "\\java.exe"))
                            path = s + "\\java.exe";
                        else if (!debug && File.Exists(s + "\\javaw.exe"))
                            path = s + "\\javaw.exe";
                        else continue;
                    }
                }
            }

            if (path == "")
            {

                bool showDialog = false;

                if (!File.Exists(Updater.appdata + "\\.mercraft\\javapath"))
                {
                    showDialog = true;
                }
                else
                {
                    //Read from the file
                    string possiblePath = File.ReadAllText(Updater.appdata + "\\.mercraft\\javapath");
                    if (File.Exists(possiblePath))
                    {
                        path = possiblePath;
                    }
                    else
                    {
                        showDialog = true;
                    }
                }

                if (showDialog)
                {
                    OpenFileDialog O = new OpenFileDialog();
                    O.InitialDirectory = ProgramFiles;
                    O.CheckFileExists = true;
                    O.Title = "Select javaw.exe";

                    if (O.ShowDialog() == DialogResult.OK)
                    {
                        path = O.FileName;

                        if (File.Exists(Updater.appdata + "\\.mercraft\\javapath"))
                        {
                            File.Delete(Updater.appdata + "\\.mercraft\\javapath");
                            //Write to the file
                        }
                        File.WriteAllText(Updater.appdata + "\\.mercraft\\javapath", O.FileName);
                    }



                }
            }

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
            try
            {
                Process Java = null;
                Java = Options.debug ? GetJavaProcess(U, P, true) : GetJavaProcess(U, P, false);
                Java.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("You might not have java installed in the right place for MerCraft. Try asking Merbo about it.");
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
        public static void LaunchAfterUpdate(LaunchForm LF, string U, string P)
        {
            try
            {
                LF.lblCurrentAction.Text = "Opening MerCraft...";
                LF.Close();
                Process Java = null;
                Java = Options.debug ? GetJavaProcess(U, P, true) : GetJavaProcess(U, P, false);
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
