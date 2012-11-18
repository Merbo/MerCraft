using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace MerCraft
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// What happens when button2 is clicked.
        /// </summary>
        /// <param name="sender">Sender. Usually of type Button.</param>
        /// <param name="e">Event arguments.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Options O = new Options();
            O.Show();
        }

        /// <summary>
        /// What happens when button1 is clicked.
        /// </summary>
        /// <param name="sender">Sender. Usually of type Button.</param>
        /// <param name="e">Event arguments.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Updater.UpToDate())
                {
                    if (Updater.CorrectJar())
                    {
                        Launcher.Launch(textBox1.Text, textBox2.Text);
                    }
                }
                else
                {
                    Updater U = new Updater();
                    U.UpdateVersion(textBox1.Text, textBox2.Text);
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("It seems as if the server is offline. Check your connection.");
                Launcher.Launch(textBox1.Text, textBox2.Text);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("You don't seem to have MerCraft fully installed. Try again after I reset your install dir for you.");
                FileIO.Uninstall();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                MessageBox.Show("You don't seem to have MerCraft fully installed. Try again after I reset your install dir for you.");
                FileIO.Uninstall();
            }
        }

        /// <summary>
        /// What happens when button3 is clicked.
        /// </summary>
        /// <param name="sender">Sender. Usually of type Button.</param>
        /// <param name="e">Event arguments.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(Updater.appdata + "\\.mercraft") &&
                    Directory.Exists(Updater.appdata + "\\.mercraft\\ModPack") &&
                    Directory.Exists(Updater.appdata + "\\.mercraft\\ModPack\\bin") &&
                    Directory.Exists(Updater.appdata + "\\.mercraft\\ModPack\\bin\\natives") &&
                    File.Exists(Updater.appdata + "\\.mercraft\\ModPack\\bin\\minecraft.jar") &&
                    File.Exists(Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl.jar") &&
                    File.Exists(Updater.appdata + "\\.mercraft\\ModPack\\bin\\lwjgl_util.jar") &&
                    File.Exists(Updater.appdata + "\\.mercraft\\ModPack\\bin\\jinput.jar"))
                {
                    Launcher.Launch(textBox1.Text, textBox2.Text);
                }
                else
                {
                    MessageBox.Show("You must have MerCraft installed in order to play offline. To install, launch in Online mode at least once.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
