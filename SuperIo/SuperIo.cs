/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the MIT license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperIo
{
    /// <summary>
    /// Super IO module
    /// </summary>
    public static class SuperIo
    {
        /// <summary>
        /// SuperIo version
        /// </summary>
        public static readonly string VERSION = "v2.0.0-alpha";

        /// <summary>
        /// Modifier Keys
        /// </summary>
        public static class ModKey
        {
            public static readonly int CTRL = 0b000001;
            public static readonly int ALT = 0b000010;
            public static readonly int SHIFT = 0b000100;
            public static readonly int R_CTRL = 0b001000;
            public static readonly int R_ALT = 0b010000;
            public static readonly int R_SHIFT = 0b100000;
        }

        /// <summary>
        /// <para>Key code</para>
        /// <para>Some may not work depend on your physical device type.</para>
        /// </summary>
        public static class Key
        {
            /// <summary>
            /// 鼠标左键
            /// </summary>
            public static readonly byte VK_LBUTTON = 0x1;
            /// <summary>
            /// 鼠标右键
            /// </summary>
            public static readonly byte VK_RBUTTON = 0x2;
            /// <summary>
            /// Cancel
            /// </summary>
            public static readonly byte VK_CANCEL = 0x3;
            /// <summary>
            /// 鼠标中键
            /// </summary>
            public static readonly byte VK_MBUTTON = 0x4;
            /// <summary>
            /// 鼠标后退键
            /// </summary>
            public static readonly byte VK_XBUTTON1 = 0x5;
            /// <summary>
            /// 鼠标前进键
            /// </summary>
            public static readonly byte VK_XBUTTON2 = 0x6;
            /// <summary>
            /// Backspace
            /// </summary>
            public static readonly byte VK_BACK = 0x8;
            /// <summary>
            /// Tab
            /// </summary>
            public static readonly byte VK_TAB = 0x9;
            /// <summary>
            /// Clear
            /// </summary>
            public static readonly byte VK_CLEAR = 0xc;
            /// <summary>
            /// Enter
            /// </summary>
            public static readonly byte VK_RETURN = 0xd;
            /// <summary>
            /// Shift
            /// </summary>
            public static readonly byte VK_SHIFT = 0x10;
            /// <summary>
            /// Ctrl
            /// </summary>
            public static readonly byte VK_CONTROL = 0x11;
            /// <summary>
            /// Alt
            /// </summary>
            public static readonly byte VK_MENU = 0x12;
            /// <summary>
            /// Pause
            /// </summary>
            public static readonly byte VK_PAUSE = 0x13;
            /// <summary>
            /// Caps Lock
            /// </summary>
            public static readonly byte VK_CAPITAL = 0x14;
            /// <summary>
            /// IME Kana 模式
            /// </summary>
            public static readonly byte VK_KANA = 0x15;
            /// <summary>
            /// IME Hanguel 模式
            /// </summary>
            public static readonly byte VK_HANGUL = 0x15;
            /// <summary>
            /// IME Junja 模式
            /// </summary>
            public static readonly byte VK_JUNJA = 0x17;
            /// <summary>
            /// IME 最终模式
            /// </summary>
            public static readonly byte VK_FINAL = 0x18;
            /// <summary>
            /// IME Hanja 模式
            /// </summary>
            public static readonly byte VK_HANJA = 0x19;
            /// <summary>
            /// Esc
            /// </summary>
            public static readonly byte VK_ESCAPE = 0x1b;
            /// <summary>
            /// IME 转换
            /// </summary>
            public static readonly byte VK_CONVERT = 0x1c;
            /// <summary>
            /// IME 不转换
            /// </summary>
            public static readonly byte VK_NONCONVERT = 0x1d;
            /// <summary>
            /// IME 接受
            /// </summary>
            public static readonly byte VK_ACCEPT = 0x1e;
            /// <summary>
            /// IME 模式更改请求
            /// </summary>
            public static readonly byte VK_MODECHANGE = 0x1f;
            /// <summary>
            /// Space
            /// </summary>
            public static readonly byte VK_SPACE = 0x20;
            /// <summary>
            /// Page Up
            /// </summary>
            public static readonly byte VK_PRIOR = 0x21;
            /// <summary>
            /// Page Down
            /// </summary>
            public static readonly byte VK_NEXT = 0x22;
            /// <summary>
            /// End
            /// </summary>
            public static readonly byte VK_END = 0x23;
            /// <summary>
            /// Home
            /// </summary>
            public static readonly byte VK_HOME = 0x24;
            /// <summary>
            /// Left Arrow
            /// </summary>
            public static readonly byte VK_LEFT = 0x25;
            /// <summary>
            /// Up Arrow
            /// </summary>
            public static readonly byte VK_UP = 0x26;
            /// <summary>
            /// Right Arrow
            /// </summary>
            public static readonly byte VK_RIGHT = 0x27;
            /// <summary>
            /// Down Arrow
            /// </summary>
            public static readonly byte VK_DOWN = 0x28;
            /// <summary>
            /// Select
            /// </summary>
            public static readonly byte VK_SELECT = 0x29;
            /// <summary>
            /// Print
            /// </summary>
            public static readonly byte VK_PRINT = 0x2a;
            /// <summary>
            /// Execute
            /// </summary>
            public static readonly byte VK_EXECUTE = 0x2b;
            /// <summary>
            /// Snapshot
            /// </summary>
            public static readonly byte VK_SNAPSHOT = 0x2c;
            /// <summary>
            /// Insert
            /// </summary>
            public static readonly byte VK_INSERT = 0x2d;
            /// <summary>
            /// Delete
            /// </summary>
            public static readonly byte VK_DELETE = 0x2e;
            /// <summary>
            /// Help
            /// </summary>
            public static readonly byte VK_HELP = 0x2f;
            /// <summary>
            /// 0
            /// </summary>
            public static readonly byte VK_0 = 0x30;
            /// <summary>
            /// 1
            /// </summary>
            public static readonly byte VK_1 = 0x31;
            /// <summary>
            /// 2
            /// </summary>
            public static readonly byte VK_2 = 0x32;
            /// <summary>
            /// 3
            /// </summary>
            public static readonly byte VK_3 = 0x33;
            /// <summary>
            /// 4
            /// </summary>
            public static readonly byte VK_4 = 0x34;
            /// <summary>
            /// 5
            /// </summary>
            public static readonly byte VK_5 = 0x35;
            /// <summary>
            /// 6
            /// </summary>
            public static readonly byte VK_6 = 0x36;
            /// <summary>
            /// 7
            /// </summary>
            public static readonly byte VK_7 = 0x37;
            /// <summary>
            /// 8
            /// </summary>
            public static readonly byte VK_8 = 0x38;
            /// <summary>
            /// 9
            /// </summary>
            public static readonly byte VK_9 = 0x39;
            /// <summary>
            /// A
            /// </summary>
            public static readonly byte VK_A = 0x41;
            /// <summary>
            /// B
            /// </summary>
            public static readonly byte VK_B = 0x42;
            /// <summary>
            /// C
            /// </summary>
            public static readonly byte VK_C = 0x43;
            /// <summary>
            /// D
            /// </summary>
            public static readonly byte VK_D = 0x44;
            /// <summary>
            /// E
            /// </summary>
            public static readonly byte VK_E = 0x45;
            /// <summary>
            /// F
            /// </summary>
            public static readonly byte VK_F = 0x46;
            /// <summary>
            /// G
            /// </summary>
            public static readonly byte VK_G = 0x47;
            /// <summary>
            /// H
            /// </summary>
            public static readonly byte VK_H = 0x48;
            /// <summary>
            /// I
            /// </summary>
            public static readonly byte VK_I = 0x49;
            /// <summary>
            /// J
            /// </summary>
            public static readonly byte VK_J = 0x4a;
            /// <summary>
            /// K
            /// </summary>
            public static readonly byte VK_K = 0x4b;
            /// <summary>
            /// L
            /// </summary>
            public static readonly byte VK_L = 0x4c;
            /// <summary>
            /// M
            /// </summary>
            public static readonly byte VK_M = 0x4d;
            /// <summary>
            /// N
            /// </summary>
            public static readonly byte VK_N = 0x4e;
            /// <summary>
            /// O
            /// </summary>
            public static readonly byte VK_O = 0x4f;
            /// <summary>
            /// P
            /// </summary>
            public static readonly byte VK_P = 0x50;
            /// <summary>
            /// Q
            /// </summary>
            public static readonly byte VK_Q = 0x51;
            /// <summary>
            /// R
            /// </summary>
            public static readonly byte VK_R = 0x52;
            /// <summary>
            /// S
            /// </summary>
            public static readonly byte VK_S = 0x53;
            /// <summary>
            /// T
            /// </summary>
            public static readonly byte VK_T = 0x54;
            /// <summary>
            /// U
            /// </summary>
            public static readonly byte VK_U = 0x55;
            /// <summary>
            /// V
            /// </summary>
            public static readonly byte VK_V = 0x56;
            /// <summary>
            /// W
            /// </summary>
            public static readonly byte VK_W = 0x57;
            /// <summary>
            /// X
            /// </summary>
            public static readonly byte VK_X = 0x58;
            /// <summary>
            /// Y
            /// </summary>
            public static readonly byte VK_Y = 0x59;
            /// <summary>
            /// Z
            /// </summary>
            public static readonly byte VK_Z = 0x5a;
            /// <summary>
            /// 左WIN键
            /// </summary>
            public static readonly byte VK_LWIN = 0x5b;
            /// <summary>
            /// 右WIN键
            /// </summary>
            public static readonly byte VK_RWIN = 0x5c;
            /// <summary>
            /// 应用程序键
            /// </summary>
            public static readonly byte VK_APPS = 0x5d;
            /// <summary>
            /// 睡眠键
            /// </summary>
            public static readonly byte VK_SLEEP = 0x5f;
            /// <summary>
            /// 小键盘 0
            /// </summary>
            public static readonly byte VK_NUMPAD0 = 0x60;
            /// <summary>
            /// 小键盘 1
            /// </summary>
            public static readonly byte VK_NUMPAD1 = 0x61;
            /// <summary>
            /// 小键盘 2
            /// </summary>
            public static readonly byte VK_NUMPAD2 = 0x62;
            /// <summary>
            /// 小键盘 3
            /// </summary>
            public static readonly byte VK_NUMPAD3 = 0x63;
            /// <summary>
            /// 小键盘 4
            /// </summary>
            public static readonly byte VK_NUMPAD4 = 0x64;
            /// <summary>
            /// 小键盘 5
            /// </summary>
            public static readonly byte VK_NUMPAD5 = 0x65;
            /// <summary>
            /// 小键盘 6
            /// </summary>
            public static readonly byte VK_NUMPAD6 = 0x66;
            /// <summary>
            /// 小键盘 7
            /// </summary>
            public static readonly byte VK_NUMPAD7 = 0x67;
            /// <summary>
            /// 小键盘 8
            /// </summary>
            public static readonly byte VK_NUMPAD8 = 0x68;
            /// <summary>
            /// 小键盘 9
            /// </summary>
            public static readonly byte VK_NUMPAD9 = 0x69;
            /// <summary>
            /// 小键盘 *
            /// </summary>
            public static readonly byte VK_MULTIPLY = 0x6a;
            /// <summary>
            /// 小键盘 +
            /// </summary>
            public static readonly byte VK_ADD = 0x6b;
            /// <summary>
            /// 小键盘 Enter
            /// </summary>
            public static readonly byte VK_SEPARATOR = 0x6c;
            /// <summary>
            /// 小键盘 -
            /// </summary>
            public static readonly byte VK_SUBTRACT = 0x6d;
            /// <summary>
            /// 小键盘 .
            /// </summary>
            public static readonly byte VK_DECIMAL = 0x6e;
            /// <summary>
            /// 小键盘 /
            /// </summary>
            public static readonly byte VK_DIVIDE = 0x6f;
            /// <summary>
            /// F1
            /// </summary>
            public static readonly byte VK_F1 = 0x70;
            /// <summary>
            /// F2
            /// </summary>
            public static readonly byte VK_F2 = 0x71;
            /// <summary>
            /// F3
            /// </summary>
            public static readonly byte VK_F3 = 0x72;
            /// <summary>
            /// F4
            /// </summary>
            public static readonly byte VK_F4 = 0x73;
            /// <summary>
            /// F5
            /// </summary>
            public static readonly byte VK_F5 = 0x74;
            /// <summary>
            /// F6
            /// </summary>
            public static readonly byte VK_F6 = 0x75;
            /// <summary>
            /// F7
            /// </summary>
            public static readonly byte VK_F7 = 0x76;
            /// <summary>
            /// F8
            /// </summary>
            public static readonly byte VK_F8 = 0x77;
            /// <summary>
            /// F9
            /// </summary>
            public static readonly byte VK_F9 = 0x78;
            /// <summary>
            /// F10
            /// </summary>
            public static readonly byte VK_F10 = 0x79;
            /// <summary>
            /// F11
            /// </summary>
            public static readonly byte VK_F11 = 0x7a;
            /// <summary>
            /// F12
            /// </summary>
            public static readonly byte VK_F12 = 0x7b;
            /// <summary>
            /// F13
            /// </summary>
            public static readonly byte VK_F13 = 0x7c;
            /// <summary>
            /// F14
            /// </summary>
            public static readonly byte VK_F14 = 0x7d;
            /// <summary>
            /// F15
            /// </summary>
            public static readonly byte VK_F15 = 0x7e;
            /// <summary>
            /// F16
            /// </summary>
            public static readonly byte VK_F16 = 0x7f;
            /// <summary>
            /// F17
            /// </summary>
            public static readonly byte VK_F17 = 0x80;
            /// <summary>
            /// F18
            /// </summary>
            public static readonly byte VK_F18 = 0x81;
            /// <summary>
            /// F19
            /// </summary>
            public static readonly byte VK_F19 = 0x82;
            /// <summary>
            /// F20
            /// </summary>
            public static readonly byte VK_F20 = 0x83;
            /// <summary>
            /// F21
            /// </summary>
            public static readonly byte VK_F21 = 0x84;
            /// <summary>
            /// F22
            /// </summary>
            public static readonly byte VK_F22 = 0x85;
            /// <summary>
            /// F23
            /// </summary>
            public static readonly byte VK_F23 = 0x86;
            /// <summary>
            /// F24
            /// </summary>
            public static readonly byte VK_F24 = 0x87;
            /// <summary>
            /// F25
            /// </summary>
            public static readonly byte VK_F25 = 0x88;
            /// <summary>
            /// Num Lock
            /// </summary>
            public static readonly byte VK_NUMLOCK = 0x90;
            /// <summary>
            /// Scroll Lock
            /// </summary>
            public static readonly byte VK_SCROLL = 0x91;
            /// <summary>
            /// 左Shift
            /// </summary>
            public static readonly byte VK_LSHIFT = 0xa0;
            /// <summary>
            /// 右Shift
            /// </summary>
            public static readonly byte VK_RSHIFT = 0xa1;
            /// <summary>
            /// 左Ctrl
            /// </summary>
            public static readonly byte VK_LCONTROL = 0xa2;
            /// <summary>
            /// 右Ctrl
            /// </summary>
            public static readonly byte VK_RCONTROL = 0xa3;
            /// <summary>
            /// 左Alt
            /// </summary>
            public static readonly byte VK_LMENU = 0xa4;
            /// <summary>
            /// 右Alt
            /// </summary>
            public static readonly byte VK_RMENU = 0xa5;
            /// <summary>
            /// 浏览器后退键
            /// </summary>
            public static readonly byte VK_BROWSER_BACK = 0xa6;
            /// <summary>
            /// 浏览器前进键
            /// </summary>
            public static readonly byte VK_BROWSER_FORWARD = 0xa7;
            /// <summary>
            /// 浏览器刷新键
            /// </summary>
            public static readonly byte VK_BROWSER_REFRESH = 0xa8;
            /// <summary>
            /// 浏览器停止键
            /// </summary>
            public static readonly byte VK_BROWSER_STOP = 0xa9;
            /// <summary>
            /// 浏览器搜索键
            /// </summary>
            public static readonly byte VK_BROWSER_SEARCH = 0xaa;
            /// <summary>
            /// 浏览器收藏键
            /// </summary>
            public static readonly byte VK_BROWSER_FAVORITES = 0xab;
            /// <summary>
            /// 浏览器“开始”和“主页”键
            /// </summary>
            public static readonly byte VK_BROWSER_HOME = 0xac;
            /// <summary>
            /// VolumeMute
            /// </summary>
            public static readonly byte VK_VOLUME_MUTE = 0xad;
            /// <summary>
            /// VolumeDown
            /// </summary>
            public static readonly byte VK_VOLUME_DOWN = 0xae;
            /// <summary>
            /// VolumeUp
            /// </summary>
            public static readonly byte VK_VOLUME_UP = 0xaf;
            /// <summary>
            /// 下一曲目键
            /// </summary>
            public static readonly byte VK_MEDIA_NEXT_TRACK = 0xb0;
            /// <summary>
            /// 上一曲目键
            /// </summary>
            public static readonly byte VK_MEDIA_PREV_TRACK = 0xb1;
            /// <summary>
            /// 停止媒体键
            /// </summary>
            public static readonly byte VK_MEDIA_STOP = 0xb2;
            /// <summary>
            /// 播放/暂停媒体键
            /// </summary>
            public static readonly byte VK_MEDIA_PLAY_PAUSE = 0xb3;
            /// <summary>
            /// 启动邮件键
            /// </summary>
            public static readonly byte VK_LAUNCH_MAIL = 0xb4;
            /// <summary>
            /// 选择媒体键
            /// </summary>
            public static readonly byte VK_LAUNCH_MEDIA_SELECT = 0xb5;
            /// <summary>
            /// 启动应用程序 1 键
            /// </summary>
            public static readonly byte VK_LAUNCH_APP1 = 0xb6;
            /// <summary>
            /// 启动应用程序 2 键
            /// </summary>
            public static readonly byte VK_LAUNCH_APP2 = 0xb7;
            /// <summary>
            /// ; :
            /// </summary>
            public static readonly byte VK_OEM_1 = 0xba;
            /// <summary>
            /// +
            /// </summary>
            public static readonly byte VK_OEM_PLUS = 0xbb;
            /// <summary>
            /// ,
            /// </summary>
            public static readonly byte VK_OEM_COMMA = 0xbc;
            /// <summary>
            /// -
            /// </summary>
            public static readonly byte VK_OEM_MINUS = 0xbd;
            /// <summary>
            /// .
            /// </summary>
            public static readonly byte VK_OEM_PERIOD = 0xbe;
            /// <summary>
            /// / ?
            /// </summary>
            public static readonly byte VK_OEM_2 = 0xbf;
            /// <summary>
            /// ` ~
            /// </summary>
            public static readonly byte VK_OEM_3 = 0xc0;
            /// <summary>
            /// [ {
            /// </summary>
            public static readonly byte VK_OEM_4 = 0xdb;
            /// <summary>
            /// \ |
            /// </summary>
            public static readonly byte VK_OEM_5 = 0xdc;
            /// <summary>
            /// ] }
            /// </summary>
            public static readonly byte VK_OEM_6 = 0xdd;
            /// <summary>
            /// ' "
            /// </summary>
            public static readonly byte VK_OEM_7 = 0xde;
            /// <summary>
            /// 其他OEM
            /// </summary>
            public static readonly byte VK_OEM_8 = 0xdf;
            /// <summary>
            /// &lt; &gt;
            /// </summary>
            public static readonly byte VK_OEM_102 = 0xe2;
            /// <summary>
            /// IME PROCESS 键
            /// </summary>
            public static readonly byte VK_PROCESSKEY = 0xe5;
            /// <summary>
            /// 用于将 Unicode 字符当作键击传递。 该 VK_PACKET 键是用于非键盘输入方法的 32 位虚拟键值的低字。
            /// </summary>
            public static readonly byte VK_PACKET = 0xe7;
            /// <summary>
            /// Attn 键
            /// </summary>
            public static readonly byte VK_ATTN = 0xf6;
            /// <summary>
            /// CrSel 键
            /// </summary>
            public static readonly byte VK_CRSEL = 0xf7;
            /// <summary>
            /// ExSel 键
            /// </summary>
            public static readonly byte VK_EXSEL = 0xf8;
            /// <summary>
            /// 删除EOF键
            /// </summary>
            public static readonly byte VK_EREOF = 0xf9;
            /// <summary>
            /// 播放键
            /// </summary>
            public static readonly byte VK_PLAY = 0xfa;
            /// <summary>
            /// 缩放键
            /// </summary>
            public static readonly byte VK_ZOOM = 0xfb;
            /// <summary>
            /// 预留
            /// </summary>
            public static readonly byte VK_NONAME = 0xfc;
            /// <summary>
            /// PA1 键
            /// </summary>
            public static readonly byte VK_PA1 = 0xfd;
            /// <summary>
            /// 清除键
            /// </summary>
            public static readonly byte VK_OEM_CLEAR = 0xfe;
        }

        /// <summary>
        /// <para>Mouse code</para>
        /// </summary>
        public static class Mouse
        {
            /// <summary>
            /// mouse move event
            /// </summary>
            public static readonly byte MOUSEMOVE = 1;
            /// <summary>
            /// left button down
            /// </summary>
            public static readonly byte LBUTTONDOWN = 2;
            /// <summary>
            /// right button down
            /// </summary>
            public static readonly byte RBUTTONDOWN = 3;
            /// <summary>
            /// middle button (mouse wheel) down
            /// </summary>
            public static readonly byte MBUTTONDOWN = 4;
            /// <summary>
            /// left button up
            /// </summary>
            public static readonly byte LBUTTONUP = 5;
            /// <summary>
            /// right button up
            /// </summary>
            public static readonly byte RBUTTONUP = 6;
            /// <summary>
            /// middle button (mouse wheel) up
            /// </summary>
            public static readonly byte MBUTTONUP = 7;
            /// <summary>
            /// scroll the mouse wheel
            /// </summary>
            public static readonly byte MOUSEWHEEL = 8;
            /// <summary>
            /// extra button 1 down
            /// </summary>
            public static readonly byte XBUTTON1DOWN = 9;
            /// <summary>
            /// extra button 1 up
            /// </summary>
            public static readonly byte XBUTTON1UP = 10;
            /// <summary>
            /// extra button 2 down
            /// </summary>
            public static readonly byte XBUTTON2DOWN = 11;
            /// <summary>
            /// extra button 2 up
            /// </summary>
            public static readonly byte XBUTTON2UP = 12;
        }
    }
}
