/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace SuperIo
{
    public static class SuperMouse
    {
        #region DllImport
        [DllImport("user32")]
        static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //移动鼠标 
        const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //模拟鼠标滚轮滚动操作，必须配合dwData参数
        const int MOUSEEVENTF_WHEEL = 0x0800;
        #endregion

        private static bool _initialized = false;        // 模块是否已经初始化
        private static int _multClickDelay = 50;       // 连续点击时，每次之间的间隔
        private static int _screenWidth = -1;
        private static int _screenHeight = -1;

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
                Rectangle bound = Tools.GetSreenRealSize();
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

            SuperMouse._screenWidth = _screenWidth;
            SuperMouse._screenHeight = _screenHeight;

            _initialized = true;
            return _initialized;
        }

        /// <summary>
        /// Delay between two clicks (or more).
        /// </summary>
        /// <returns>Multiple Click Delay</returns>
        public static int GetMultClickDelay()
        {
            return _multClickDelay;
        }
        /// <summary>
        /// Delay between two clicks (or more).
        /// </summary>
        /// <param name="delay">New delay</param>
        public static void SetMultClickDelay(int delay)
        {
            _multClickDelay = delay;
        }

        private static void CheckInitialization()
        {
            if (!_initialized)
            {
                throw new Exception("SuperMouse has not initialized yet. Or initialization failed. Try to call `SuperMouse.Initialize()` first.");
            }
        }

        public static int GetScreenWidth()
        {
            return _screenWidth;
        }
        public static int GetScreenHeight()
        {
            return _screenHeight;
        }
        public static Rectangle GetScreenSize()
        {
            return new Rectangle(0, 0, _screenWidth, _screenHeight);
        }
        #endregion

        /// <summary>
        /// Move mouse relatively.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public static void MoveRelative(int dx, int dy)
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }

        /// <summary>
        /// Move mouse to the absolute position (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void MoveTo(int x, int y)
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, x*65535/_screenWidth, y*65535/_screenHeight, 0, 0);
        }

        /// <summary>
        /// Left button presses down.
        /// </summary>
        public static void LButtonDown()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
        /// <summary>
        /// Left button releases.
        /// </summary>
        public static void LButtonUp()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Left button clicks.(one time)
        /// </summary>
        public static void LButtonClick()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Left button clicks multiple times.
        /// </summary>
        /// <param name="times"></param>
        public static void LButtonClick(int times)
        {
            CheckInitialization();
            for (int i = 0; i < times; i++) 
            {
                if (i != 0)
                {
                    Thread.Sleep(_multClickDelay);
                }
                LButtonClick();
            }
        }

        /// <summary>
        /// Right button presses down.
        /// </summary>
        public static void RButtonDown()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }
        /// <summary>
        /// Right button releases.
        /// </summary>
        public static void RButtonUp()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Right button clicks.(one time)
        /// </summary>
        public static void RButtonClick()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Right button clicks multiple times.
        /// </summary>
        /// <param name="times"></param>
        public static void RButtonClick(int times)
        {
            CheckInitialization();
            for (int i = 0; i < times; i++)
            {
                if (i != 0)
                {
                    Thread.Sleep(_multClickDelay);
                }
                RButtonClick();
            }
        }

        /// <summary>
        /// Middle button(mouse wheel) pressed down.
        /// </summary>
        public static void MButtonDown()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
        }
        /// <summary>
        /// Middle button(mouse wheel) releases.
        /// </summary>
        public static void MButtonUp()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Middle button(mouse wheel) clicks.(one time)
        /// </summary>
        public static void MButtonClick()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Middle button(mouse wheel) clicks multiple times.
        /// </summary>
        /// <param name="times"></param>
        public static void MButtonClick(int times)
        {
            CheckInitialization();
            for (int i = 0; i < times; i++)
            {
                if (i != 0)
                {
                    Thread.Sleep(_multClickDelay);
                }
                MButtonClick();
            }
        }

        /// <summary>
        /// Mouse wheel scrolls up or down. Depends on value given.
        /// </summary>
        /// <param name="value">Scroll up if value is positive. Down if negative</param>
        public static void Scroll(int value)
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, value, 0);
        }
        /// <summary>
        /// Simply scroll up.
        /// </summary>
        public static void ScrollUp()
        {
            CheckInitialization();
            Scroll(120);
        }
        /// <summary>
        /// Simply scroll down.
        /// </summary>
        public static void ScrollDown()
        {
            CheckInitialization();
            Scroll(-120);
        }
    }
}
