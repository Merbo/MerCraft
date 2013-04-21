using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MerCraft
{
    /// <summary>
    /// Form that the game launches in; Controller of WinAPI
    /// </summary>
    public partial class GameForm : Form
    {
        /// <summary>
        /// Panel1's child. (MerCraft)
        /// </summary>
        public IntPtr childHandle;

        /// <summary>
        /// The java process associated with the child handle.
        /// </summary>
        public Process javaProcess;

        /// <summary>
        /// Whether or not we've tried to change the handle.
        /// </summary>
        public bool hasTriedHandle;

        /// <summary>
        /// The form that will contain the game.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            childHandle = IntPtr.Zero;
            javaProcess = null;
            hasTriedHandle = false;
        }

        /// <summary>
        /// What happens when the form closes.
        /// </summary>
        /// <param name="sender">The sender. Always of type Form.</param>
        /// <param name="e">EventArgs.</param>
        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (childHandle != IntPtr.Zero)
            {
                if (WinAPI.GetWindow(this.panel1.Handle, (uint)WinAPI.GW.GW_CHILD) == childHandle)
                {
                    WinAPI.DestroyWindow(childHandle);
                    Application.Exit();
                }
            }

            if (javaProcess != null)
            {
                if (!javaProcess.HasExited)
                    javaProcess.Kill();
                Application.Exit();
            }

            if (!e.CloseReason.HasFlag(CloseReason.None))
                Application.Exit();
        }

        /// <summary>
        /// What happens when panel1 is resized.
        /// </summary>
        /// <param name="sender">The sender. Will be of type Panel.</param>
        /// <param name="e">EventArgs.</param>
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

        /// <summary>
        /// What happens when Timer1 ticks.
        /// </summary>
        /// <param name="sender">The sender. Will be of type Timer.</param>
        /// <param name="e">EventArgs.</param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (childHandle != IntPtr.Zero)
            {
                if (WinAPI.GetWindow(this.panel1.Handle, (uint)WinAPI.GW.GW_CHILD) != childHandle)
                {
                    childHandle = IntPtr.Zero;
                    Application.Exit();
                }
                else
                    WinAPI.BringWindowToTop(childHandle);
            }
        }

        /// <summary>
        /// What happens when the form detects a keypress.
        /// </summary>
        /// <param name="sender">The sender. Will be of type Form.</param>
        /// <param name="e">EventArgs</param>
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (WinAPI.GetTopWindow(IntPtr.Zero) != childHandle)
            {
                WinAPI.BringWindowToTop(childHandle);
            }
        }

        /// <summary>
        /// What happens when the panel gets focus.
        /// </summary>
        /// <param name="sender">The sender. Will be of type panel.</param>
        /// <param name="e">EventArgs</param>
        private void panel1_Enter(object sender, EventArgs e)
        {
            if (WinAPI.GetTopWindow(IntPtr.Zero) != childHandle)
            {
                WinAPI.BringWindowToTop(childHandle);
            }
        }

        /// <summary>
        /// What happens when the panel gets clicked.
        /// </summary>
        /// <param name="sender">The sender. Will be of type panel.</param>
        /// <param name="e">EventArgs</param>
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (WinAPI.GetTopWindow(IntPtr.Zero) != childHandle)
            {
                WinAPI.BringWindowToTop(childHandle);
            }
        }

        /// <summary>
        /// What happens when the form gets clicked.
        /// </summary>
        /// <param name="sender">The sender. Will be of type form.</param>
        /// <param name="e">EventArgs.</param>
        private void GameForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (WinAPI.GetTopWindow(IntPtr.Zero) != childHandle)
            {
                WinAPI.BringWindowToTop(childHandle);
            }
        }

        /// <summary>
        /// What happens when the form gets focus.
        /// </summary>
        /// <param name="sender">The sender. Will be of type form.</param>
        /// <param name="e">EventArgs.</param>
        private void GameForm_Enter(object sender, EventArgs e)
        {
            if (WinAPI.GetTopWindow(IntPtr.Zero) != childHandle)
            {
                WinAPI.BringWindowToTop(childHandle);
            }
        }
    }
}
