using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace MerCraft
{
    class FileIO
    {
        /// <summary>
        /// Uninstalls mercraft by deleting it and stuff.
        /// </summary>
        public static void Uninstall()
        {
            try
            {
                if (Directory.Exists(Updater.appdata + "\\.mercraft"))
                    Directory.Delete(Updater.appdata + "\\.mercraft", true);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(
                    "It appears the install dir is in use. Please terminate any programs using it. \r\n" +
                    "You may need to delete \"" + Updater.appdata + "\\.mercraft\" by yourself.");
            }
        }

        /// <summary>
        /// Copies an entire directory and its contents.
        /// </summary>
        /// <param name="SourcePath">Directory to copy.</param>
        /// <param name="DestinationPath">Directory to output to.</param>
        /// <param name="overwriteexisting">Overwrite?</param>
        /// <returns>Successfyl</returns>
        public static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = false;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch
            {
                ret = false;
            }
            return ret;
        }  
    }
}
