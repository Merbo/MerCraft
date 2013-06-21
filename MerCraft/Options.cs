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
    /// <summary>
    /// Options form
    /// </summary>
    public partial class Options : Form
    {

        public MerCraftConfig Config;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Options()
        {
            InitializeComponent();

            if (!Directory.Exists(Updater.appdata + "\\.mercraft"))
                Directory.CreateDirectory(Updater.appdata + "\\.mercraft");
            Config = new MerCraftConfig(Updater.appdata + "\\.mercraft\\config");
        }

        

        /// <summary>
        /// Load up the options and apply them to the form.
        /// </summary>
        /// <param name="sender">The object that caused this. Usually of type Form.</param>
        /// <param name="e">Event Arguments.</param>
        private void Options_Load(object sender, EventArgs e)
        {
            string Jar = Config.GetConfigVarString("Jar");
            switch (Jar)
            {
                case "Vanilla":
                    radioButton1.Checked = true;
                    break;
                case "Optifine":
                    radioButton2.Checked = true;
                    break;
                case "Shaders":
                    radioButton3.Checked = true;
                    break;
                default:
                    checkBox1.Checked = true;
                    break;
            }

            string MCVersion = Config.GetConfigVarString("MCVersion");
            textBox1.Text = MCVersion == null ? "Not downloaded" : MCVersion;
            string Version = Config.GetConfigVarString("Version");
            textBox2.Text = Version == null ? "Not downloaded" : Version;

            bool Debug = Config.GetConfigVarBool("Debug");
            checkBox1.Checked = Debug;
        }

        /// <summary>
        /// What to do when button1 is clicked.
        /// </summary>
        /// <param name="sender">The object that caused this. Should be of type Button.</param>
        /// <param name="e">Event Arguments.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                Config.SetConfigVar("Jar", "Vanilla");
            else if (radioButton2.Checked)
                Config.SetConfigVar("Jar", "Optifine");
            else if (radioButton3.Checked)
                Config.SetConfigVar("Jar", "Shaders");

            Config.SetConfigVar("Debug", checkBox1.Checked);

            this.Hide();
        }
    }

    public class MerCraftConfig
    {
        public string ConfigPath;
        public MerCraftConfig(string FilePath)
        {
            ConfigPath = FilePath;
        }

        public bool GetConfigVarBool(string name)
        {
            StreamReader Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
            string fullConf = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            string[] AllOptions = fullConf.Replace("\r", "").Split('\n');
            foreach (string Option in AllOptions)
            {
                if (Option.Split(':')[0].Replace(" ", "") == name)
                {
                    string Value = Option.Split(':')[1].ToLower();
                    return Value == "true" || Value == "yes" || Value == "on"; 
                }
            }

            return false;
        }
        public int GetConfigVarInt(string name)
        {
            StreamReader Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
            string fullConf = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            string[] AllOptions = fullConf.Replace("\r", "").Split('\n');
            foreach (string Option in AllOptions)
            {
                if (Option.Split(':')[0].Replace(" ", "") == name)
                {
                    return Convert.ToInt32(Option.Split(':')[1]);
                }
            }

            return 0;
        }
        public string GetConfigVarString(string name)
        {
            StreamReader Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
            string fullConf = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            string[] AllOptions = fullConf.Replace("\r", "").Split('\n');
            foreach (string Option in AllOptions)
            {
                if (Option.Split(':')[0].Replace(" ", "") == name)
                {
                    return Option.Split(':')[1];
                }
            }

            return null;
        }
        public float GetConfigVarFloat(string name)
        {
            StreamReader Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
            string fullConf = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            string[] AllOptions = fullConf.Replace("\r", "").Split('\n');
            foreach (string Option in AllOptions)
            {
                if (Option.Split(':')[0].Replace(" ", "") == name)
                {
                    return Convert.ToSingle(Option.Split(':')[1]);
                }
            }

            return 0.0f;
        }
        public double GetConfigVarDouble(string name)
        {
            StreamReader Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
            string fullConf = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            string[] AllOptions = fullConf.Replace("\r", "").Split('\n');
            foreach (string Option in AllOptions)
            {
                if (Option.Split(':')[0].Replace(" ", "") == name)
                {
                    return Convert.ToDouble(Option.Split(':')[1]);
                }
            }

            return 0.0;
        }
        public void SetConfigVar(string name, object value)
        {
            StreamWriter Writer = null;
            StreamReader Reader = null;
            string fullConf = null;
            if (GetConfigVarString(name) != null)
            {
                Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
                fullConf = Reader.ReadToEnd();
                Reader.Close();
                Reader = null;
                string Value = null;
                string[] AllOptions = fullConf.Replace("\r", "").Split('\n');
                foreach (string Option in AllOptions)
                {
                    if (Option.Split(':')[0].Replace(" ", "") == name)
                    {
                        Value = Option.Split(':')[1];
                    }
                }

                fullConf = fullConf.Replace(name + ":" + Value, name + ":" + value);
                fullConf += Environment.NewLine;

                fullConf = fullConf.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

                File.Delete(ConfigPath);
                Writer = new StreamWriter(File.Open(ConfigPath, FileMode.OpenOrCreate));
                Writer.Write(fullConf);
                Writer.Close();
                Writer = null;
                return;
            }

            Reader = new StreamReader(File.Open(ConfigPath, FileMode.OpenOrCreate));
            fullConf = Reader.ReadToEnd();
            Reader.Close();
            Reader = null;

            fullConf += name + ":" + value + Environment.NewLine;

            fullConf = fullConf.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            Writer = new StreamWriter(File.Open(ConfigPath, FileMode.OpenOrCreate));
            Writer.Write(fullConf);
            Writer.Close();
            Writer = null;
        }
    }
}
