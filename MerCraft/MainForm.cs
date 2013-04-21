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
using System.Net;

namespace MerCraft
{
    /// <summary>
    /// Main form
    /// </summary>
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
        /// Version of MineCraft base selected 
        /// </summary>
        public string PreferredVersion
        {
            get
            {
                return this.comboBox1.SelectedItem.ToString();
            }
            set
            {
                this.comboBox1.SelectedItem = value;
            }
        }

        /// <summary>
        /// What happens when button2 is clicked.
        /// </summary>
        /// <param name="sender">Sender. Usually of type Button.</param>
        /// <param name="e">Event arguments.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            new Options().Show();
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
                        Launcher.Launch(textBox1.Text, textBox2.Text);
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://173.48.94.88/MerCraft/Versions/VersionList.txt");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string[] ServerVersions = sr.ReadToEnd().Replace("\r", "").Split('\n');
            sr.Close();
            myResponse.Close();

            this.comboBox1.Items.Clear();
            this.comboBox1.Items.AddRange(ServerVersions);
            this.comboBox1.SelectedIndex = this.comboBox1.Items.Count-1;
        }
    }
}
