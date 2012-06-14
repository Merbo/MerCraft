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
        public Options()
        {
            InitializeComponent();
        }

        public static string Jar = "";

        private void Options_Load(object sender, EventArgs e)
        {
            StreamReader Reader = null;
            if (File.Exists(Updater.appdata + "\\.mercraft\\config"))
            {
                Reader = new StreamReader(Updater.appdata + "\\.mercraft\\config");
                Jar = Reader.ReadLine();
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter Writer = null;
            FileStream file = File.Open(Updater.appdata + "\\.mercraft\\config", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            Writer = new StreamWriter(file, Encoding.UTF8);

            if (radioButton1.Checked)
            {
                Writer.WriteLine("Vanilla");
            }
            else if (radioButton2.Checked)
            {
                Writer.WriteLine("OptiFine");
            }
            else if (radioButton3.Checked)
            {
                Writer.WriteLine("Shaders");
            }
            else
            {
                Writer.WriteLine("Vanilla");
            }

            Writer.Close();
            Writer = null;

            this.Close();
        }

    }
}
