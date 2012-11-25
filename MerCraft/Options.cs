using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;
using System.IO;

namespace MerCraft
{
    public partial class Options : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Options()
        {
            InitializeComponent();
        }

        public static string Jar = "";
        public static bool debug = false;
        public static bool SMP = true;
        public static bool SMPChanged = false;
        public static bool Sound = false;
        public static bool SoundChanged = false;

        /// <summary>
        /// Load up the options and apply them to the form.
        /// </summary>
        /// <param name="sender">The object that caused this. Usually of type Form.</param>
        /// <param name="e">Event Arguments.</param>
        private void Options_Load(object sender, EventArgs e)
        {
            StreamReader Reader = null;
            if (Directory.Exists(Updater.appdata + "\\.mercraft"))
            {
                if (File.Exists(Updater.appdata + "\\.mercraft\\config"))
                {
                    Reader = new StreamReader(Updater.appdata + "\\.mercraft\\config");
                    Jar = Reader.ReadLine();
                    string d = Reader.ReadLine();
                    switch (d)
                    {
                        case "true":
                            debug = true;
                            break;
                        default:
                            debug = false;
                            break;
                    }
                    string smp = Reader.ReadLine();
                    switch (smp)
                    {
                        case "true":
                            SMP = true;
                            break;
                        case "false":
                            SMP = false;
                            break;
                        default:
                            SMP = true;
                            break;
                           
                    }
                    string sound = Reader.ReadLine();
                    switch (sound)
                    {
                        case "true":
                            Sound = true;
                            break;
                        default:
                            Sound = false;
                            break;
                    }
                    Reader.Close();
                    Reader = null;
                }

                switch (Jar)
                {
                    case "Vanilla":
                        radioButton1.Checked = true;
                        break;
                    case "OptiFine":
                        radioButton2.Checked = true;
                        break;
                    case "Shaders":
                        radioButton3.Checked = true;
                        break;
                }
                checkBox1.Checked = debug ? true : false;
                checkBox2.Checked = SMP ? true : false;
                checkBox3.Checked = Sound ? true : false;
            }
        }

        /// <summary>
        /// What to do when button1 is clicked.
        /// </summary>
        /// <param name="sender">The object that caused this. Should be of type Button.</param>
        /// <param name="e">Event Arguments.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter Writer = null;
            if (!Directory.Exists(Updater.appdata + "\\.mercraft"))
                Directory.CreateDirectory(Updater.appdata + "\\.mercraft");
            FileStream file = File.Open(Updater.appdata + "\\.mercraft\\config", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            Writer = new StreamWriter(file, Encoding.UTF8);

            if (radioButton1.Checked)
                Writer.WriteLine("Vanilla");
            else if (radioButton2.Checked)
                Writer.WriteLine("OptiFine");
            else if (radioButton3.Checked)
                Writer.WriteLine("Shaders");
            else
                Writer.WriteLine("Vanilla");

            if (checkBox1.Checked)
            {
                Writer.WriteLine("true");
                debug = true;
            }
            else
            {
                Writer.WriteLine("false");
                debug = false;
            }

            if (checkBox2.Checked)
            {
                Writer.WriteLine("true");
                SMP = true;
            }
            else
            {
                Writer.WriteLine("false");
                SMP = false;
            }

            if (checkBox3.Checked)
            {
                Writer.WriteLine("true");
                Sound = true;
            }
            else
            {
                Writer.WriteLine("false");
                Sound = false;
            }

            Writer.Close();
            Writer = null;

            this.Close();
        }

        /// <summary>
        /// What happens when checkbox2 is changed.
        /// </summary>
        /// <param name="sender">The sender of it. Usually a CheckBox type.</param>
        /// <param name="e">Event Arguments.</param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SMPChanged = true;
            if (!checkBox2.Checked)
            {
                radioButton1.Checked = true;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
            }
            else
            {
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SoundChanged = true;
        }
    }
}
