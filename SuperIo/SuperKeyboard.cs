/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Threading;

namespace SuperIo
{
    public static class SuperKeyboard
    {
        private static bool _initialized = false;        // 模块是否已经初始化
        private static int _keyPressDelay = 50;        // 调用KeyPress时，按下到弹起的时间间隔

        #region ArgumentsSetup
        /// <summary>
        /// Initialize the SuperKeyboard module.
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            if (_initialized)
            {
                return true;
            }
            _initialized = WinRing0.init();
            return _initialized;
        }

        /// <summary>
        /// The delay between a key's *down* and *up* when method `KeyPress` is called.
        /// </summary>
        /// <returns></returns>
        public static int GetKeyPressDelay()
        {
            return _keyPressDelay;
        }
        /// <summary>
        /// The delay between a key's *down* and *up* when method `KeyPress` is called.
        /// </summary>
        /// <param name="delay">New delay</param>
        public static void SetKeyPressDelay(int delay)
        {
            _keyPressDelay = delay;
        }

        private static void CheckInitialization()
        {
            if (!_initialized)
            {
                throw new Exception("SuperKeyboard has not initialized yet. Or initialization failed. Try to call `SuperKeyboard.Initialize()` first.");
            }
        }
        #endregion

        /// <summary>
        /// Press down the key.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        public static void KeyDown(byte keycode)
        {
            CheckInitialization();
            WinRing0.KeyDown(keycode);
        }

        /// <summary>
        /// Release the key.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        public static void KeyUp(byte keycode)
        {
            CheckInitialization();
            WinRing0.KeyUp(keycode);
        }

        /// <summary>
        /// Press the key one time.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        public static void KeyPress(byte keycode)
        {
            CheckInitialization();
            WinRing0.KeyDown(keycode);
            Thread.Sleep(_keyPressDelay);
            WinRing0.KeyUp(keycode);
        }

        /// <summary>
        /// Press the key one time.
        /// <para>With keys in argument `with` holding.</para>
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        /// <param name="with">CmdKey flag(s).</param>
        public static void KeyPress(byte keycode, int with)
        {
            CheckInitialization();

            bool ctrl = (with & CmdKey.CTRL) > 0;
            bool alt = (with & CmdKey.ALT) > 0;
            bool shift = (with & CmdKey.SHIFT) > 0;
            bool rctrl = (with & CmdKey.R_CTRL) > 0;
            bool ralt = (with & CmdKey.R_ALT) > 0;
            bool rshift = (with & CmdKey.R_SHIFT) > 0;
            bool hasWith = ctrl || alt || shift || rctrl || ralt || rshift;

            if (hasWith)
            {
                if (ctrl)
                {
                    WinRing0.KeyDown(Key.VK_LCONTROL);
                }
                if (alt)
                {
                    WinRing0.KeyDown(Key.VK_LMENU);
                }
                if (shift)
                {
                    WinRing0.KeyDown(Key.VK_LSHIFT);
                }
                if (rctrl)
                {
                    WinRing0.KeyDown(Key.VK_RCONTROL);
                }
                if (ralt)
                {
                    WinRing0.KeyDown(Key.VK_RMENU);
                }
                if (rshift)
                {
                    WinRing0.KeyDown(Key.VK_RSHIFT);
                }

                Thread.Sleep(_keyPressDelay);
            }

            WinRing0.KeyDown(keycode);
            Thread.Sleep(_keyPressDelay);
            WinRing0.KeyUp(keycode);

            if (hasWith)
            {
                Thread.Sleep(_keyPressDelay);

                if (ctrl)
                {
                    WinRing0.KeyUp(Key.VK_LCONTROL);
                }
                if (alt)
                {
                    WinRing0.KeyUp(Key.VK_LMENU);
                }
                if (shift)
                {
                    WinRing0.KeyUp(Key.VK_LSHIFT);
                }
                if (rctrl)
                {
                    WinRing0.KeyUp(Key.VK_RCONTROL);
                }
                if (ralt)
                {
                    WinRing0.KeyUp(Key.VK_RMENU);
                }
                if (rshift)
                {
                    WinRing0.KeyUp(Key.VK_RSHIFT);
                }
            }
        }

        /// <summary>
        /// Apply key combination sequence
        /// <para>For example. If the given sequence is: [A,B,C,D].</para>
        /// <para>It will press down A, then press down B (with the A holding, the same goes for the following), then C, and then D.</para>
        /// <para>Finally, release these keys in order D,C,B,A</para>
        /// <para>If argument `interval` is not given, it will be set to `KeyPressDelay` as default.</para>
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycodes">Key code. Can be found in SuperKeyboard.Key</param>
        public static void KeyCombSeq(params byte[] keycodes)
        {
            KeyCombSeq(_keyPressDelay, keycodes);
        }

        /// <summary>
        /// Apply key combination sequence
        /// <para>For example. If the given sequence is: [A,B,C,D].</para>
        /// <para>It will press down A, then press down B (with the A holding, the same goes for the following), then C, and then D.</para>
        /// <para>Finally, release these keys in order D,C,B,A</para>
        /// <para>If argument `interval` is not given, it will be set to `KeyPressDelay` as default.</para>
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycodes">Key code. Can be found in SuperKeyboard.Key</param>
        /// <param name="interval">Interval between two keys.</param>
        public static void KeyCombSeq(int interval, params byte[] keycodes)
        {
            int count = keycodes.Length;
            for (int i = 0; i < count; i++)
            {
                WinRing0.KeyDown(keycodes[i]);
                Thread.Sleep(interval);
            }
            for (int i = count - 1; i >= 0; i--)
            {
                WinRing0.KeyUp(keycodes[i]);
                if (i != 0)
                {
                    Thread.Sleep(interval);
                }
            }
        }

        /// <summary>
        /// Command Keys
        /// </summary>
        public static class CmdKey
        {
            public static readonly int CTRL    = 0b000001;
            public static readonly int ALT     = 0b000010;
            public static readonly int SHIFT   = 0b000100;
            public static readonly int R_CTRL  = 0b001000;
            public static readonly int R_ALT   = 0b010000;
            public static readonly int R_SHIFT = 0b100000;
        }

        /// <summary>
        /// <para>Key code</para>
        /// <para>Some may not work depend on your physical device type.</para>
        /// </summary>
        public static class Key
        {
            public static readonly byte VK_LBUTTON = 0x1;              // 鼠标左键
            public static readonly byte VK_RBUTTON = 0x2;              // 鼠标右键
            public static readonly byte VK_CANCEL = 0x3;               // Cancel
            public static readonly byte VK_MBUTTON = 0x4;              // 鼠标中键
            public static readonly byte VK_XBUTTON1 = 0x5;             // 鼠标后退键
            public static readonly byte VK_XBUTTON2 = 0x6;             // 鼠标前进键
            public static readonly byte VK_BACK = 0x8;                 // Backspace
            public static readonly byte VK_TAB = 0x9;                  // Tab
            public static readonly byte VK_CLEAR = 0xc;                // Clear
            public static readonly byte VK_RETURN = 0xd;               // Enter
            public static readonly byte VK_SHIFT = 0x10;               // Shift
            public static readonly byte VK_CONTROL = 0x11;             // Ctrl
            public static readonly byte VK_MENU = 0x12;                // Alt
            public static readonly byte VK_PAUSE = 0x13;               // Pause
            public static readonly byte VK_CAPITAL = 0x14;             // Caps Lock
            public static readonly byte VK_KANA = 0x15;
            public static readonly byte VK_HANGUL = 0x15;
            public static readonly byte VK_JUNJA = 0x17;
            public static readonly byte VK_FINAL = 0x18;
            public static readonly byte VK_HANJA = 0x19;
            public static readonly byte VK_ESCAPE = 0x1b;              // Esc
            public static readonly byte VK_CONVERT = 0x1c;
            public static readonly byte VK_NONCONVERT = 0x1d;
            public static readonly byte VK_ACCEPT = 0x1e;
            public static readonly byte VK_MODECHANGE = 0x1f;
            public static readonly byte VK_SPACE = 0x20;               // Space
            public static readonly byte VK_PRIOR = 0x21;               // Page Up
            public static readonly byte VK_NEXT = 0x22;                // Page Down
            public static readonly byte VK_END = 0x23;                 // End
            public static readonly byte VK_HOME = 0x24;                // Home
            public static readonly byte VK_LEFT = 0x25;                // Left Arrow
            public static readonly byte VK_UP = 0x26;                  // Up Arrow
            public static readonly byte VK_RIGHT = 0x27;               // Right Arrow
            public static readonly byte VK_DOWN = 0x28;                // Down Arrow
            public static readonly byte VK_SELECT = 0x29;              // Select
            public static readonly byte VK_PRINT = 0x2a;               // Print
            public static readonly byte VK_EXECUTE = 0x2b;             // Execute
            public static readonly byte VK_SNAPSHOT = 0x2c;            // Snapshot
            public static readonly byte VK_INSERT = 0x2d;              // Insert
            public static readonly byte VK_DELETE = 0x2e;              // Delete
            public static readonly byte VK_HELP = 0x2f;                // Help
            public static readonly byte VK_0 = 0x30;
            public static readonly byte VK_1 = 0x31;
            public static readonly byte VK_2 = 0x32;
            public static readonly byte VK_3 = 0x33;
            public static readonly byte VK_4 = 0x34;
            public static readonly byte VK_5 = 0x35;
            public static readonly byte VK_6 = 0x36;
            public static readonly byte VK_7 = 0x37;
            public static readonly byte VK_8 = 0x38;
            public static readonly byte VK_9 = 0x39;
            public static readonly byte VK_A = 0x41;
            public static readonly byte VK_B = 0x42;
            public static readonly byte VK_C = 0x43;
            public static readonly byte VK_D = 0x44;
            public static readonly byte VK_E = 0x45;
            public static readonly byte VK_F = 0x46;
            public static readonly byte VK_G = 0x47;
            public static readonly byte VK_H = 0x48;
            public static readonly byte VK_I = 0x49;
            public static readonly byte VK_J = 0x4a;
            public static readonly byte VK_K = 0x4b;
            public static readonly byte VK_L = 0x4c;
            public static readonly byte VK_M = 0x4d;
            public static readonly byte VK_N = 0x4e;
            public static readonly byte VK_O = 0x4f;
            public static readonly byte VK_P = 0x50;
            public static readonly byte VK_Q = 0x51;
            public static readonly byte VK_R = 0x52;
            public static readonly byte VK_S = 0x53;
            public static readonly byte VK_T = 0x54;
            public static readonly byte VK_U = 0x55;
            public static readonly byte VK_V = 0x56;
            public static readonly byte VK_W = 0x57;
            public static readonly byte VK_X = 0x58;
            public static readonly byte VK_Y = 0x59;
            public static readonly byte VK_Z = 0x5a;
            public static readonly byte VK_LWIN = 0x5b;                // 左WIN键
            public static readonly byte VK_RWIN = 0x5c;                // 右WIN键
            public static readonly byte VK_APPS = 0x5d;                // 应用程序键
            public static readonly byte VK_SLEEP = 0x5f;               // 睡眠键
            public static readonly byte VK_NUMPAD0 = 0x60;             // 小键盘 0
            public static readonly byte VK_NUMPAD1 = 0x61;             // 小键盘 1
            public static readonly byte VK_NUMPAD2 = 0x62;             // 小键盘 2
            public static readonly byte VK_NUMPAD3 = 0x63;             // 小键盘 3
            public static readonly byte VK_NUMPAD4 = 0x64;             // 小键盘 4
            public static readonly byte VK_NUMPAD5 = 0x65;             // 小键盘 5
            public static readonly byte VK_NUMPAD6 = 0x66;             // 小键盘 6
            public static readonly byte VK_NUMPAD7 = 0x67;             // 小键盘 7
            public static readonly byte VK_NUMPAD8 = 0x68;             // 小键盘 8
            public static readonly byte VK_NUMPAD9 = 0x69;             // 小键盘 9
            public static readonly byte VK_MULTIPLY = 0x6a;            // 小键盘 *
            public static readonly byte VK_ADD = 0x6b;                 // 小键盘 +
            public static readonly byte VK_SEPARATOR = 0x6c;           // 小键盘 Enter
            public static readonly byte VK_SUBTRACT = 0x6d;            // 小键盘 -
            public static readonly byte VK_DECIMAL = 0x6e;             // 小键盘 .
            public static readonly byte VK_DIVIDE = 0x6f;              // 小键盘 /
            public static readonly byte VK_F1 = 0x70;                  // F1
            public static readonly byte VK_F2 = 0x71;                  // F2
            public static readonly byte VK_F3 = 0x72;                  // F3
            public static readonly byte VK_F4 = 0x73;                  // F4
            public static readonly byte VK_F5 = 0x74;                  // F5
            public static readonly byte VK_F6 = 0x75;                  // F6
            public static readonly byte VK_F7 = 0x76;                  // F7
            public static readonly byte VK_F8 = 0x77;                  // F8
            public static readonly byte VK_F9 = 0x78;                  // F9
            public static readonly byte VK_F10 = 0x79;                 // F10
            public static readonly byte VK_F11 = 0x7a;                 // F11
            public static readonly byte VK_F12 = 0x7b;                 // F12
            public static readonly byte VK_NUMLOCK = 0x90;             // Num Lock
            public static readonly byte VK_SCROLL = 0x91;              // Scroll
            public static readonly byte VK_LSHIFT = 0xa0;              // 左shift
            public static readonly byte VK_RSHIFT = 0xa1;              // 右shift
            public static readonly byte VK_LCONTROL = 0xa2;
            public static readonly byte VK_RCONTROL = 0xa3;
            public static readonly byte VK_LMENU = 0xa4;
            public static readonly byte VK_RMENU = 0xa5;
            public static readonly byte VK_BROWSER_BACK = 0xa6;
            public static readonly byte VK_BROWSER_FORWARD = 0xa7;
            public static readonly byte VK_BROWSER_REFRESH = 0xa8;
            public static readonly byte VK_BROWSER_STOP = 0xa9;
            public static readonly byte VK_BROWSER_SEARCH = 0xaa;
            public static readonly byte VK_BROWSER_FAVORITES = 0xab;
            public static readonly byte VK_BROWSER_HOME = 0xac;
            public static readonly byte VK_VOLUME_MUTE = 0xad;         // VolumeMute
            public static readonly byte VK_VOLUME_DOWN = 0xae;         // VolumeDown
            public static readonly byte VK_VOLUME_UP = 0xaf;           // VolumeUp
            public static readonly byte VK_MEDIA_NEXT_TRACK = 0xb0;
            public static readonly byte VK_MEDIA_PREV_TRACK = 0xb1;
            public static readonly byte VK_MEDIA_STOP = 0xb2;
            public static readonly byte VK_MEDIA_PLAY_PAUSE = 0xb3;
            public static readonly byte VK_LAUNCH_MAIL = 0xb4;
            public static readonly byte VK_LAUNCH_MEDIA_SELECT = 0xb5;
            public static readonly byte VK_LAUNCH_APP1 = 0xb6;
            public static readonly byte VK_LAUNCH_APP2 = 0xb7;
            public static readonly byte VK_OEM_1 = 0xba;               // ; :
            public static readonly byte VK_OEM_PLUS = 0xbb;            // = +
            public static readonly byte VK_OEM_COMMA = 0xbc;           // ,
            public static readonly byte VK_OEM_MINUS = 0xbd;           // - _
            public static readonly byte VK_OEM_PERIOD = 0xbe;          // .
            public static readonly byte VK_OEM_2 = 0xbf;               // / ?
            public static readonly byte VK_OEM_3 = 0xc0;               // ` ~
            public static readonly byte VK_OEM_4 = 0xdb;               // [ {
            public static readonly byte VK_OEM_5 = 0xdc;               // \ |
            public static readonly byte VK_OEM_6 = 0xdd;               // ] }
            public static readonly byte VK_OEM_7 = 0xde;		          // ' "
            public static readonly byte VK_OEM_8 = 0xdf;
            public static readonly byte VK_OEM_102 = 0xe2;
            public static readonly byte VK_PACKET = 0xe7;
            public static readonly byte VK_PROCESSKEY = 0xe5;
            public static readonly byte VK_ATTN = 0xf6;
            public static readonly byte VK_CRSEL = 0xf7;
            public static readonly byte VK_EXSEL = 0xf8;
            public static readonly byte VK_EREOF = 0xf9;
            public static readonly byte VK_PLAY = 0xfa;
            public static readonly byte VK_ZOOM = 0xfb;
            public static readonly byte VK_NONAME = 0xfc;
            public static readonly byte VK_PA1 = 0xfd;
            public static readonly byte VK_OEM_CLEAR = 0xfe;
        }
    }
}
