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
        /// The options system.
        /// </summary>
        public Options Opts;

        /// <summary>
        /// Set to true as soon as MerCraft gets version data.
        /// </summary>
        public bool Online;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Opts = new Options();

            // If not running on Windows NT, don't use the Windows API
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                this.checkBox1.Visible = false;
                this.checkBox1.Checked = false;
            }
        }

        /// <summary>
        /// Version of MineCraft base selected 
        /// </summary>
        public string PreferredVersion
        {
            get
            {
                if (!this.Online)
                {
                    return null;
                }

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
            Opts.Show();
        }

        /// <summary>
        /// What happens when button1 is clicked.
        /// </summary>
        /// <param name="sender">Sender. Usually of type Button.</param>
        /// <param name="e">Event arguments.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.Online)
            {
                MessageBox.Show("Please wait for MerCraft to grab the version data.");
                return;
            }

            try
            {
                this.Opts.Config.SetConfigVar("Username", textBox1.Text);
                this.Opts.Config.SetConfigVar("Password", textBox2.Text);
                this.Opts.Config.SetConfigVar("WinAPI", checkBox1.Checked);

                string CurrentMCVersion = "", UpdateMCVersion = "";
                double CurrentVersion = 0, UpdateVersion = 0;

                if (Updater.UpToDate(out CurrentMCVersion, out CurrentVersion, out UpdateMCVersion, out UpdateVersion))
                {
                    if (Updater.CorrectJar())
                        Launcher.Launch(textBox1.Text, textBox2.Text);
                }
                else
                {
                    if (UpdateMCVersion != "")
                    {
                        MessageBox.Show("MC Version Shift! New: " + UpdateMCVersion + "; Old: " + CurrentMCVersion);
                    }
                    else if (UpdateVersion != 0)
                    {
                        MessageBox.Show("MerCraft Update! New: " + UpdateVersion + "; Old: " + CurrentVersion);
                    }

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
            if (!this.Online)
            {
                MessageBox.Show("Please wait for MerCraft to grab the version data.");
                return;
            }

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
            this.Online = false;

            this.comboBox1.SelectedIndex = this.comboBox1.Items.Count-1;

            Thread VersionThread = new Thread(() =>
            {
                HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http://mercraft.merbo.org/MerCraft/Versions/VersionList.txt");
                Req.Method = "GET";
                WebResponse Res = Req.GetResponse();
                StreamReader sr = new StreamReader(Res.GetResponseStream(), Encoding.UTF8);
                string[] ServerVersions = sr.ReadToEnd().Replace("\r", "").Split('\n');
                sr.Close();
                Res.Close();

                SetVersionList(ServerVersions);
            });

            VersionThread.IsBackground = true;
            VersionThread.Start();

            this.textBox1.Text = this.Opts.Config.GetConfigVarString("Username");
            this.textBox2.Text = this.Opts.Config.GetConfigVarString("Password");
            this.checkBox1.Checked = this.Opts.Config.GetConfigVarBool("WinAPI");
        }

        private delegate void SetVersionListCallback(string[] Versions);
        private void SetVersionList(string[] Versions)
        {
            if (this.comboBox1.InvokeRequired)
            {
                SetVersionListCallback d = new SetVersionListCallback(SetVersionList);
                this.Invoke(d, new object[] { Versions });
            }
            else
            {
                this.comboBox1.Items.Clear();
                this.comboBox1.Items.AddRange(Versions);
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count-1;
                this.Online = true;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Opts != null)
            {
                this.Opts.Close();
                this.Opts = null;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private int CurrentImage = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (CurrentImage++)
            {
                case 0:
                    this.BackgroundImage = global::MerCraft.Properties.Resources._2013_06_20_19_32_31;
                    break;
                case 1:
                    this.BackgroundImage = global::MerCraft.Properties.Resources._2013_06_20_20_04_29;
                    break;
                case 2:
                    this.BackgroundImage = global::MerCraft.Properties.Resources._2013_06_20_21_14_43;
                    break;
                case 3:
                    this.BackgroundImage = global::MerCraft.Properties.Resources._2013_06_20_21_15_08;
                    CurrentImage = 0;
                    break;
            }
        }
    }
}
