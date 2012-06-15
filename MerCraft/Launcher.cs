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
        private static string JavaProcessFileName()
        {
            path = "";

            List<string> PathTries     = new List<string>();
            List<string> PathTries_x86 = new List<string>();


            //Java installation defaults
            PathTries.Add("C:\\Program Files\\Java\\jdk1.7.0_04\\bin\\javaw.exe");
            PathTries.Add("C:\\Program Files\\Java\\jre7\\bin\\javaw.exe");
            PathTries.Add("C:\\Program Files\\Java\\jre6\\bin\\javaw.exe");
            PathTries.Add(ProgramFiles + "\\Java\\jdk1.7.0_04\\bin\\javaw.exe");
            PathTries.Add(ProgramFiles + "\\Java\\jre7\\bin\\javaw.exe");
            PathTries.Add(ProgramFiles + "\\Java\\jre6\\bin\\javaw.exe");
            PathTries.Add("C:\\Progs\\Java\\jdk1.7.0_04\\bin\\javaw.exe");
            PathTries.Add("C:\\Progs\\Java\\jre7\\bin\\javaw.exe");
            PathTries.Add("C:\\Progs\\Java\\jre6\\bin\\javaw.exe");

            //On x64 machines, but a x86 java installation
            PathTries_x86.Add("C:\\Program Files (x86)\\Java\\jdk1.7.0_04\\bin\\javaw.exe");
            PathTries_x86.Add("C:\\Program Files (x86)\\Java\\jre7\\bin\\javaw.exe");
            PathTries_x86.Add("C:\\Program Files (x86)\\Java\\jre6\\bin\\javaw.exe");
            PathTries_x86.Add(ProgramFiles + "\\Java\\jdk1.7.0_04\\bin\\javaw.exe");
            PathTries_x86.Add(ProgramFiles + "\\Java\\jre7\\bin\\javaw.exe");
            PathTries_x86.Add(ProgramFiles + "\\Java\\jre6\\bin\\javaw.exe");
            PathTries_x86.Add("C:\\Progs\\Java\\jdk1.7.0_04\\bin\\javaw.exe");
            PathTries_x86.Add("C:\\Progs\\Java\\jre7\\bin\\javaw.exe");
            PathTries_x86.Add("C:\\Progs\\Java\\jre6\\bin\\javaw.exe");

            foreach (string s in PathTries)
            {
                if (File.Exists(s))
                {
                    path = s;
                }
            }

            if (path == "")
            {
                foreach (string s in PathTries_x86)
                {
                    if (File.Exists(s))
                    {
                        path = s;
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
        private static Process GetJavaProcess(string U, string P)
        {
            Process Java = new Process();

            string JPath = JavaProcessFileName();

            if (JPath != "")
                Java.StartInfo.FileName = JPath;
            else
                return null;

            Java.StartInfo.Arguments = 
                Environment.Is64BitOperatingSystem ? "-Xmx2048M " : "-Xmx1024M" +
                "-Xincgc " +
                "-Xmn256M " +
                "-cp \"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl_util.jar;" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\jinput.jar\" " +
                "-Djava.library.path=\"" + Updater.appdata + "\\.mercraft\\ModPack\\bin\\natives\" " +
                "net.minecraft.client.Minecraft " +
                U + " " +
                P + " " +
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
