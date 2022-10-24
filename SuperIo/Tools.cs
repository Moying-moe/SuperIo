/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperIo
{
    internal static class Tools
    {
        #region DllImport
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
            IntPtr hdc, // handle to DC
            int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();
        #endregion

        /// <summary>
        /// Get the actual size of the primary screen. Regardless of the *Screen Scale*.
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetSreenRealSize()
        {
            var hdc = GetDC(GetDesktopWindow());
            int nWidth = GetDeviceCaps(hdc, DESKTOPHORZRES);
            int nHeight = GetDeviceCaps(hdc, DESKTOPVERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
            Rectangle result = new Rectangle(0, 0, nWidth, nHeight);
            return result;
        }
    }
}
