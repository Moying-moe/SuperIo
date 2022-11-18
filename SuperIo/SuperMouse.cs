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
    /// <summary>
    /// Provide the ability to control the mouse.
    /// </summary>
    public sealed class SuperMouse
    {
        #region Singleton
        private static readonly Lazy<SuperMouse> lazy = new Lazy<SuperMouse>(() => new SuperMouse());
        /// <summary>
        /// Instance
        /// </summary>
        public static SuperMouse Instance { get { return lazy.Value; } }
        #endregion

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

        private bool _initialized = false;        // 模块是否已经初始化
        private int _multClickDelay = 50;       // 连续点击时，每次之间的间隔
        private int _screenWidth = -1;
        private int _screenHeight = -1;

        /// <summary>
        /// Is the module initialized successfully
        /// </summary>
        public bool IsInitialized { get => _initialized; }

        #region ArgumentsSetup
        /// <summary>
        /// Initialize the SuperMouse module.
        /// </summary>
        private SuperMouse()
        {
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
                _initialized = false;
                return;
            }
            
            _initialized = true;
        }

        /// <summary>
        /// <para>Set the screen size.</para>
        /// <para>If auto initialization get the wrong screen size, or you have multiple monitor. Please call this method.</para>
        /// </summary>
        /// <param name="_screenWidth">screen width</param>
        /// <param name="_screenHeight">screen height</param>
        public void SetScreenSize(int _screenWidth, int _screenHeight)
        {
            this._screenWidth = _screenWidth;
            this._screenHeight = _screenHeight;
        }

        /// <summary>
        /// Delay between two clicks (or more).
        /// </summary>
        /// <returns>Multiple Click Delay</returns>
        public int GetMultClickDelay()
        {
            return _multClickDelay;
        }
        /// <summary>
        /// Delay between two clicks (or more).
        /// </summary>
        /// <param name="delay">New delay</param>
        public void SetMultClickDelay(int delay)
        {
            _multClickDelay = delay;
        }

        private void CheckInitialization()
        {
            if (!_initialized)
            {
                throw new Exception("SuperMouse initialization failed.");
            }
        }

        /// <summary>
        /// Get the screen width.
        /// </summary>
        /// <returns></returns>
        public int GetScreenWidth()
        {
            return _screenWidth;
        }
        /// <summary>
        /// Get the screen height.
        /// </summary>
        /// <returns></returns>
        public int GetScreenHeight()
        {
            return _screenHeight;
        }
        /// <summary>
        /// Get the screen size.
        /// </summary>
        /// <returns></returns>
        public Size GetScreenSize()
        {
            return new Size(_screenWidth, _screenHeight);
        }
        #endregion

        /// <summary>
        /// Move mouse relatively.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void MoveRelative(int dx, int dy)
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }

        /// <summary>
        /// Move mouse to the absolute position (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(int x, int y)
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, x*65535/_screenWidth, y*65535/_screenHeight, 0, 0);
        }

        /// <summary>
        /// Left button presses down.
        /// </summary>
        public void LButtonDown()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
        /// <summary>
        /// Left button releases.
        /// </summary>
        public void LButtonUp()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Left button clicks.(one time)
        /// </summary>
        public void LButtonClick()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Left button clicks multiple times.
        /// </summary>
        /// <param name="times"></param>
        public void LButtonClick(int times)
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
        public void RButtonDown()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }
        /// <summary>
        /// Right button releases.
        /// </summary>
        public void RButtonUp()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Right button clicks.(one time)
        /// </summary>
        public void RButtonClick()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Right button clicks multiple times.
        /// </summary>
        /// <param name="times"></param>
        public void RButtonClick(int times)
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
        public void MButtonDown()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
        }
        /// <summary>
        /// Middle button(mouse wheel) releases.
        /// </summary>
        public void MButtonUp()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Middle button(mouse wheel) clicks.(one time)
        /// </summary>
        public void MButtonClick()
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// Middle button(mouse wheel) clicks multiple times.
        /// </summary>
        /// <param name="times"></param>
        public void MButtonClick(int times)
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
        public void Scroll(int value)
        {
            CheckInitialization();
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, value, 0);
        }
        /// <summary>
        /// Simply scroll up.
        /// </summary>
        public void ScrollUp()
        {
            CheckInitialization();
            Scroll(120);
        }
        /// <summary>
        /// Simply scroll down.
        /// </summary>
        public void ScrollDown()
        {
            CheckInitialization();
            Scroll(-120);
        }
    }
}
