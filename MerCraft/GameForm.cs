using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MerCraft
{
    public partial class GameForm : Form
    {
        public IntPtr childHandle;
        public GameForm()
        {
            InitializeComponent();
            childHandle = IntPtr.Zero;
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (childHandle != IntPtr.Zero)
            {
                int style = WinAPI.GetWindowLong(childHandle, WinAPI.GWL_STYLE);
                WinAPI.MoveWindow(childHandle, 0, 0, panel1.Width, panel1.Height, true);
                WinAPI.SetWindowLong(childHandle, WinAPI.GWL_STYLE, (style & ~(int)WinAPI.WS.WS_SYSMENU));
                WinAPI.SetWindowLong(childHandle, WinAPI.GWL_STYLE, (style & ~(int)WinAPI.WS.WS_CAPTION));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (childHandle != IntPtr.Zero)
            {
                if (WinAPI.GetWindow(this.panel1.Handle, (uint)WinAPI.GW.GW_CHILD) != childHandle)
                {
                    childHandle = IntPtr.Zero;
                    Application.Exit();
                }
            }
        }
    }
}
