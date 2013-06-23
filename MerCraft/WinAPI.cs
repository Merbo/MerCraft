using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MerCraft
{
    class WinAPI
    {
        /// <summary>
        /// Sets the parent window of another.
        /// </summary>
        /// <param name="hWndChild">Handle to the window.</param>
        /// <param name="hWndNewParent">Handle to the new parent.</param>
        /// <returns>Handle to the previous parent, if any.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Allows styles to be changed and such.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nIndex">Style index? Use WinAPI.GWL_STYLE here.</param>
        /// <param name="dwNewLong">The new style. See WinAPI.WS.</param>
        /// <returns>The old style.</returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Moves a window.
        /// </summary>
        /// <param name="Handle">Handle to the window.</param>
        /// <param name="x">New X position on the window. 0 for no change.</param>
        /// <param name="y">New Y position on the window. 0 for no change.</param>
        /// <param name="w">New width. 0 for no change.</param>
        /// <param name="h">New width. 0 for no change.</param>
        /// <param name="repaint">Redraw?</param>
        /// <returns>Successful</returns>
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);

        /// <summary>
        /// Shows a window.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <param name="nCmdShow">Show style.</param>
        /// <returns>Successful</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Brings window to the front of the screen.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <returns>Successful</returns>
        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        /// <summary>
        /// Gets a window's style.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nIndex">Style index. See WinAPI.GWL_STYLE.</param>
        /// <returns>Nonzero if successful.</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Gets a window handle by its caption.
        /// </summary>
        /// <param name="ZeroOnly">Always use IntPtr.Zero here.</param>
        /// <param name="lpWindowName">Caption to find.</param>
        /// <returns>A handle to the first found matching window.</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        /// <summary>
        /// Gets a window's menu.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <returns>Handle to the menu.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        /// <summary>
        /// Gets the amount of items in a menu.
        /// </summary>
        /// <param name="hMenu">Handle to the menu.</param>
        /// <returns>The amount of items found.</returns>
        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        /// <summary>
        /// Redraws the menu bar of a window.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <returns>Successful</returns>
        [DllImport("user32.dll")]
        public static extern bool DrawMenuBar(IntPtr hWnd);

        /// <summary>
        /// Removes a menu button.
        /// </summary>
        /// <param name="hMenu">Handle to the menu.</param>
        /// <param name="uPosition">Position of the button.</param>
        /// <param name="uFlags">Removal flags.</param>
        /// <returns>Successful</returns>
        [DllImport("user32.dll")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// Gets a window handle, but applies flags.
        /// </summary>
        /// <param name="hWnd">Window handle to start with.</param>
        /// <param name="uCmd">Flags.</param>
        /// <returns>The returned handle.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        /// <summary>
        /// Closes a window and kills its application.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <returns>Successful</returns>
        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

        /// <summary>
        /// Gets the window at the top.
        /// </summary>
        /// <param name="OptHWnd">Can be IntPtr.Zero. If specified, examines the handle's children.</param>
        /// <returns>The top window.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr OptHWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string Text);

        /// <summary>
        /// Enum for Menu Flags.
        /// </summary>
        public enum MF : uint
        {
            MF_BYPOSITION = 0x400,
            MF_REMOVE = 0x1000,
        };
        
        /// <summary>
        /// Enum for window states.
        /// </summary>
        public enum WS : int
        {
            WS_CHILD = 0x40000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_SYSMENU = 0x00080000,
        };

        /// <summary>
        /// Enum for Window Hierarchy.
        /// </summary>
        public enum GW : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6,
        };

        /// <summary>
        /// More readable way to write -16. :)
        /// </summary>
        public static int GWL_STYLE = -16;
        
    }
}
