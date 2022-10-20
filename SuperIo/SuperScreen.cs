/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SuperIo
{
    public static class SuperScreen
    {
        #region DllImport
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        #endregion

        /// <summary>
        /// Get the color of the pixel at (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        /// <summary>
        /// <para>Return the color distance between 2 colors.</para>
        /// <para>Formula: Diff = (R1 - R2)^2 + (G1 - G2)^2 + (B1 - B2)^2 / (255^2 * 3)</para>
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>Difference. Range from 0 to 1.</returns>
        public static double ColorDifference(Color c1, Color c2)
        {
            double dR = Math.Pow(c1.R - c2.R, 2);
            double dG = Math.Pow(c1.G - c2.G, 2);
            double dB = Math.Pow(c1.B - c2.B, 2);
            return Math.Sqrt((dR + dG + dB) / 195075d);
        }

        /// <summary>
        /// <para>Return if the pixel at (x, y) is the same color as argument `target` given.</para>
        /// <para>Equivalent to `IsColorAt(x, y, target, 1.0d)`</para>
        /// </summary>
        /// <param name="x">Pixel position x</param>
        /// <param name="y">Pixel position y</param>
        /// <param name="target">Color for comparison</param>
        /// <returns></returns>
        public static bool IsColorAt(int x, int y, Color target)
        {
            Color color = GetPixelColor(x, y);
            return color == target;
        }
        /// <summary>
        /// Return if the pixel at (x, y) is similar to the color that argument `target` given.
        /// </summary>
        /// <param name="x">Pixel position x</param>
        /// <param name="y">Pixel position y</param>
        /// <param name="target">Color for comparison</param>
        /// <param name="similarity">Color similarity limit. Range from 0 to 1. (1 means just the same color)</param>
        /// <returns></returns>
        public static bool IsColorAt(int x, int y, Color target, double similarity)
        {
            Color color = GetPixelColor(x, y);
            return ColorDifference(color, target) <= (1-similarity);
        }
    }
}
