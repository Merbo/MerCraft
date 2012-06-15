using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MerCraft
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Java check
            try
            {
                string java = JavaDetect.JavaPath.GetJavaVersion();
            }
            catch (JavaDetect.JavaNotFoundException)
            {
                MessageBox.Show("You need to have Java installed to use MerCraft!", "MerCraft", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
