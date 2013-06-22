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

        /// <summary>
        /// Our configuration.
        /// </summary>
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

            string InitHeap = Config.GetConfigVarString("InitHeap");
            comboBox1.SelectedIndex = comboBox1.FindString(InitHeap);

            string MaxHeap = Config.GetConfigVarString("MaxHeap");
            comboBox2.SelectedIndex = comboBox2.FindString(MaxHeap);
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

            Config.SetConfigVar("InitHeap", comboBox1.SelectedItem.ToString());
            Config.SetConfigVar("MaxHeap", comboBox2.SelectedItem.ToString());

            this.Hide();
        }
    }

    /// <summary>
    /// Configuration system.
    /// </summary>
    public class MerCraftConfig
    {
        /// <summary>
        /// The string to the config file.
        /// </summary>
        public string ConfigPath;

        /// <summary>
        /// Initialize a new config.
        /// </summary>
        /// <param name="FilePath">Path to the config file.</param>
        public MerCraftConfig(string FilePath)
        {
            ConfigPath = FilePath;
        }

        /// <summary>
        /// Read a config variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>ConVar.</returns>
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
                    return Value == "true" || Value == "yes" || Value == "on" || Value == "1"; 
                }
            }

            return false;
        }

        /// <summary>
        /// Read a config variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>ConVar.</returns>
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

        /// <summary>
        /// Read a config variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>ConVar.</returns>
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

        /// <summary>
        /// Read a config variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>ConVar.</returns>
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

        /// <summary>
        /// Read a config variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>ConVar.</returns>
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

        /// <summary>
        /// Set a config variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="value">Variable value.</param>
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
