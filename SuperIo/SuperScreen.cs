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
        [DllImportAttribute("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
                                          IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        #endregion

        private static bool _initialized = false;        // 模块是否已经初始化
        private static int _screenWidth = -1;
        private static int _screenHeight = -1;

        public static bool IsInitialized { get => _initialized; }

        #region ArgumentsSetup
        /// <summary>
        /// Initialize the SuperMouse module.
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            if (_initialized)
            {
                return true;
            }
            try
            {
                //Rectangle bound = Screen.PrimaryScreen.Bounds;
                Size bound = Tools.GetSreenRealSize();
                _screenWidth = bound.Width;
                _screenHeight = bound.Height;
            }
            catch
            {
                // Get primary screen size failed.
                return _initialized;
            }

            _initialized = true;
            return _initialized;
        }

        /// <summary>
        /// <para>Initialize the SuperMouse module.</para>
        /// <para>If auto initialization get the wrong screen size, or you have multiple monitor. Please call this method.</para>
        /// </summary>
        /// <param name="_screenWidth">screen width</param>
        /// <param name="_screenHeight">screen height</param>
        /// <returns></returns>
        public static bool Initialize(int _screenWidth, int _screenHeight)
        {
            if (_initialized)
            {
                return true;
            }

            SuperScreen._screenWidth = _screenWidth;
            SuperScreen._screenHeight = _screenHeight;

            _initialized = true;
            return _initialized;
        }

        public static int GetScreenWidth()
        {
            return _screenWidth;
        }
        public static int GetScreenHeight()
        {
            return _screenHeight;
        }
        public static Size GetScreenSize()
        {
            return new Size(_screenWidth, _screenHeight);
        }

        private static void CheckInitialization()
        {
            if (!_initialized)
            {
                throw new Exception("SuperMouse has not initialized yet. Or initialization failed. Try to call `SuperMouse.Initialize()` first.");
            }
        }
        #endregion

        /// <summary>
        /// Get the color of the pixel at (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetPixelColor(int x, int y)
        {
            CheckInitialization();

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
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>Difference. Range from 0 to 1.</returns>
        public static double ColorDifference(Color c1, Color c2)
        {
            CheckInitialization();

            long rmean = ((long)c1.R + (long)c2.R) / 2;
            long r = (long)c1.R - (long)c2.R;
            long g = (long)c1.G - (long)c2.G;
            long b = (long)c1.B - (long)c2.B;
            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8)) / 764.833315173967;
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
            CheckInitialization();

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
            CheckInitialization();

            Color color = GetPixelColor(x, y);
            return ColorDifference(color, target) <= (1-similarity);
        }

        /// <summary>
        /// Get screenshot.
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetScreen()
        {
            CheckInitialization();

            Bitmap img = new Bitmap(1920, 1080);
            Graphics gimg = Graphics.FromImage(img);
            gimg.CopyFromScreen(0, 0, 0, 0, GetScreenSize());
            return img;
        }

        public enum SearchDirection
        {
            FromLeftTop,
            FromRightTop,
            FromLeftBottom,
            FromRightBottom,
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop,
            FromCenter
        }

        private static readonly Point POINT_NOT_FOUND = new Point(-1, -1);

        #region SearchColor
        /// <summary>
        /// Search color on the screen
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static Point SearchColor(Color color, SearchDirection direction, Rectangle area)
        {
            CheckInitialization();

            Bitmap bitmap = GetScreen();
            switch (direction)
            {
                case SearchDirection.FromLeftTop:
                case SearchDirection.FromRightTop:
                case SearchDirection.FromLeftBottom:
                case SearchDirection.FromRightBottom:
                    return SearchFromCorner(bitmap, color, direction, area, true);
                case SearchDirection.LeftToRight:
                case SearchDirection.RightToLeft:
                    return SearchHorizontal(bitmap, color, direction, area, true);
                case SearchDirection.TopToBottom:
                case SearchDirection.BottomToTop:
                    return SearchVertical(bitmap, color, direction, area, true);
                case SearchDirection.FromCenter:
                    throw new NotImplementedException();
            }
            return POINT_NOT_FOUND;
        }

        /// <summary>
        /// Search color on the screen
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <param name="area"></param>
        /// <param name="similarity"></param>
        /// <returns></returns>
        public static Point SearchColor(Color color, SearchDirection direction, Rectangle area, double similarity)
        {
            CheckInitialization();

            Bitmap bitmap = GetScreen();
            switch (direction)
            {
                case SearchDirection.FromLeftTop:
                case SearchDirection.FromRightTop:
                case SearchDirection.FromLeftBottom:
                case SearchDirection.FromRightBottom:
                    return SearchFromCorner(bitmap, color, direction, area, false, similarity);
                case SearchDirection.LeftToRight:
                case SearchDirection.RightToLeft:
                    return SearchHorizontal(bitmap, color, direction, area, false, similarity);
                case SearchDirection.TopToBottom:
                case SearchDirection.BottomToTop:
                    return SearchVertical(bitmap, color, direction, area, false, similarity);
                case SearchDirection.FromCenter:
                    throw new NotImplementedException();
            }
            return POINT_NOT_FOUND;
        }

        /// <summary>
        /// Search color on the screen
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Point SearchColor(Color color, SearchDirection direction)
        {
            CheckInitialization();

            Bitmap bitmap = GetScreen();
            Rectangle area = new Rectangle(0, 0, _screenWidth, _screenHeight);

            switch (direction)
            {
                case SearchDirection.FromLeftTop:
                case SearchDirection.FromRightTop:
                case SearchDirection.FromLeftBottom:
                case SearchDirection.FromRightBottom:
                    return SearchFromCorner(bitmap, color, direction, area, true);
                case SearchDirection.LeftToRight:
                case SearchDirection.RightToLeft:
                    return SearchHorizontal(bitmap, color, direction, area, true);
                case SearchDirection.TopToBottom:
                case SearchDirection.BottomToTop:
                    return SearchVertical(bitmap, color, direction, area, true);
                case SearchDirection.FromCenter:
                    throw new NotImplementedException();
            }
            return POINT_NOT_FOUND;
        }

        /// <summary>
        /// Search color on the screen
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <param name="similarity"></param>
        /// <returns></returns>
        public static Point SearchColor(Color color, SearchDirection direction, double similarity)
        {
            CheckInitialization();

            Bitmap bitmap = GetScreen();
            Rectangle area = new Rectangle(0, 0, _screenWidth, _screenHeight);

            switch (direction)
            {
                case SearchDirection.FromLeftTop:
                case SearchDirection.FromRightTop:
                case SearchDirection.FromLeftBottom:
                case SearchDirection.FromRightBottom:
                    return SearchFromCorner(bitmap, color, direction, area, false, similarity);
                case SearchDirection.LeftToRight:
                case SearchDirection.RightToLeft:
                    return SearchHorizontal(bitmap, color, direction, area, false, similarity);
                case SearchDirection.TopToBottom:
                case SearchDirection.BottomToTop:
                    return SearchVertical(bitmap, color, direction, area, false, similarity);
                case SearchDirection.FromCenter:
                    throw new NotImplementedException();
            }
            return POINT_NOT_FOUND;
        }

        private static Point SearchFromCorner(Bitmap bitmap, Color color, SearchDirection direction, Rectangle area, bool accurate, double similarity=-1)
        {
            if (direction != SearchDirection.FromLeftTop &&
                direction != SearchDirection.FromRightTop &&
                direction != SearchDirection.FromLeftBottom &&
                direction != SearchDirection.FromRightBottom)
            {
                throw new Exception("Wrong direction.");
            }

            int width = area.Width;
            int height = area.Height;
            int startx = area.Left;
            int starty = area.Top;

            int rangeN = width + height - 1;

            double difference = 1 - similarity;

            bool flipX = (direction == SearchDirection.FromRightTop || direction == SearchDirection.FromRightBottom);
            bool flipY = (direction == SearchDirection.FromLeftBottom || direction == SearchDirection.FromRightBottom);

            for (int i = 0; i < rangeN; i++) 
            {
                for (int x = Math.Min(i, width - 1); x >= Math.Max(i - height + 1, 0); x--)
                {
                    int y = i - x;

                    int tx = startx + (flipX ? (width - x - 1) : x);
                    int ty = starty + (flipY ? (height - y - 1) : y);

                    Color target = bitmap.GetPixel(tx, ty);
                    if (accurate)
                    {
                        if (target == color)
                        {
                            return new Point(tx, ty);
                        }
                    }
                    else
                    {
                        if (ColorDifference(target, color) <= difference)
                        {
                            return new Point(tx, ty);
                        }
                    }
                }
            }

            return POINT_NOT_FOUND;
        }

        private static Point SearchHorizontal(Bitmap bitmap, Color color, SearchDirection direction, Rectangle area, bool accurate, double similarity = -1)
        {
            if (direction != SearchDirection.LeftToRight &&
                direction != SearchDirection.RightToLeft)
            {
                throw new Exception("Wrong direction.");
            }

            int width = area.Width;
            int height = area.Height;
            int startx = area.Left;
            int starty = area.Top;

            double difference = 1 - similarity;

            bool flipX = (direction == SearchDirection.RightToLeft);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int tx = startx + (flipX ? (width - x - 1) : x);
                    int ty = starty + y;

                    Color target = bitmap.GetPixel(tx, ty);
                    if (accurate)
                    {
                        if (target == color)
                        {
                            return new Point(tx, ty);
                        }
                    }
                    else
                    {
                        if (ColorDifference(target, color) <= difference)
                        {
                            return new Point(tx, ty);
                        }
                    }
                }
            }

            return POINT_NOT_FOUND;
        }

        private static Point SearchVertical(Bitmap bitmap, Color color, SearchDirection direction, Rectangle area, bool accurate, double similarity = -1)
        {
            if (direction != SearchDirection.TopToBottom &&
                direction != SearchDirection.BottomToTop)
            {
                throw new Exception("Wrong direction.");
            }

            int width = area.Width;
            int height = area.Height;
            int startx = area.Left;
            int starty = area.Top;

            double difference = 1 - similarity;

            bool flipY = (direction == SearchDirection.BottomToTop);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int tx = startx + x;
                    int ty = starty + (flipY ? (height - y - 1) : y);

                    Color target = bitmap.GetPixel(tx, ty);
                    if (accurate)
                    {
                        if (target == color)
                        {
                            return new Point(tx, ty);
                        }
                    }
                    else
                    {
                        if (ColorDifference(target, color) <= difference)
                        {
                            return new Point(tx, ty);
                        }
                    }
                }
            }

            return POINT_NOT_FOUND;
        }
        #endregion
    }
}
