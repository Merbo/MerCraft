using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MerCraft
{
    static class Program
    {
        /// <summary>
        /// The main form of the application.
        /// </summary>
        public static MainForm M;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

#if DEBUG
            Debugger.Launch();
#endif

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
            M = new MainForm();
            Application.Run(M);
        }
    }
}
