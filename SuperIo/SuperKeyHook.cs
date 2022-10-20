/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace SuperIo
{
    public class SuperKeyHook
    {
        #region DllImport
        //定义SetWindowsHookEx
        [DllImport("user32")]
        //参数列表:idHook-钩子类型,lpfn-钩子函数委托,hmod-窗口句柄，dwThreadId-监控的线程数
        static extern int SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hmod, int dwThreadId);
        //定义CallNextHookEx
        [DllImport("user32")]
        //参数列表:hhk-WindowsHookEx的返回值,nCode-
        static extern int CallNextHookEx(int hhk, int nCode, int wParam, IntPtr lParam);
        //定义UnhookWindowsHookEx
        [DllImport("user32")]
        //参数列表:hhk-WindowsHookEx的返回值
        static extern bool UnhookWindowsHookEx(int hhk);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        //声明一个HOOKPROC类型的委托
        delegate int HOOKPROC(int code, int wParam, IntPtr lParam);


        const int WM_KEYDOWN = 0x0100;//键盘按键按下对应的值
        const int WM_KEYUP = 0x0101;//键盘按键抬起对应的值

        [StructLayout(LayoutKind.Sequential)]
        public class tagKBDLLHOOKSTRUCT
        {
            public int vkCode;//虚拟键盘码
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        #endregion

        int _setWindowsHookExReturnKeyBoard;
        HOOKPROC _keyBoard;

        Dictionary<string, KeyHookHandlerStruct> _registeredHooks = new Dictionary<string, KeyHookHandlerStruct>();

        /// <summary>
        /// Create Key Hook object.
        /// </summary>
        public SuperKeyHook()
        {
            _keyBoard = new HOOKPROC(HookProcMathodKeyBoard);//创建委托变量
            Process curProcess = Process.GetCurrentProcess();//获取窗体句柄
            ProcessModule curModule = curProcess.MainModule;
            _setWindowsHookExReturnKeyBoard = SetWindowsHookEx(13, _keyBoard, GetModuleHandle(curModule.ModuleName), 0);
        }

        ~SuperKeyHook()
        {
            // 解除键盘钩子
            UnhookWindowsHookEx(_setWindowsHookExReturnKeyBoard);
        }

        public int HookProcMathodKeyBoard(int code, int wParam, IntPtr lParam)
        {
            // 处理键盘事件

            tagKBDLLHOOKSTRUCT v = (tagKBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(tagKBDLLHOOKSTRUCT));//将捕获的键盘信息存储到存储键盘信息的结构体中
            
            System.Windows.Input.Key key = KeyInterop.KeyFromVirtualKey(v.vkCode);//将键盘信息转换成C#中可用的键
            if (code >= 0)//如果code的值大于0说明获取到了按键输入
            {
                string keyString = key.ToString();
                KeyHookHandlerStruct handler;
                if (_registeredHooks.TryGetValue(keyString, out handler))
                {
                    if (wParam == WM_KEYDOWN)//检测到键盘按下
                    {
                        if (!handler.IsDown)
                        {
                            handler.IsDown = true;
                            handler.OnKeyDown();
                        }
                    }
                    if (wParam == WM_KEYUP)//检测到键盘抬起
                    {
                        if (handler.IsDown)
                        {
                            handler.IsDown = false;
                            handler.OnKeyUp();
                        }
                    }
                }
            }
            return CallNextHookEx(_setWindowsHookExReturnKeyBoard, code, wParam, lParam);//将按键值传
                                                                                        //递给下一个钩子
        }

        public delegate void KeyHookHandler();

        [StructLayout(LayoutKind.Sequential)]
        public class KeyHookHandlerStruct {
            public bool IsDown = false;
            public KeyHookHandler OnKeyDown;
            public KeyHookHandler OnKeyUp;
        }

        /// <summary>
        /// <para>Register a key hook.</para>
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keyString">Key that will trigger the handler</param>
        /// <param name="handler">Handler</param>
        /// <returns>Return false if given key is already exists.</returns>
        public bool Register(string keyString, KeyHookHandlerStruct handler)
        {
            if (_registeredHooks.ContainsKey(keyString))
            {
                return false;
            }
            _registeredHooks.Add(keyString, handler);
            return true;
        }
        /// <summary>
        /// Register a key hook.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keyString">Key that will trigger the handler</param>
        /// <param name="keyDownHandler">Key down handler</param>
        /// <param name="keyUpHandler">Key up handler</param>
        /// <returns></returns>
        public bool Register(string keyString, KeyHookHandler keyDownHandler, KeyHookHandler keyUpHandler)
        {
            return Register(keyString, new KeyHookHandlerStruct()
            {
                OnKeyDown = keyDownHandler,
                OnKeyUp = keyUpHandler
            });
        }

        public static class Key
        {
            public static readonly string BACK = "Back";                // Backspace
            public static readonly string TAB = "Tab";                  // Tab
            public static readonly string RETURN = "Return";            // Enter
            public static readonly string SHIFT = "LeftShift";          // Shift
            public static readonly string CONTROL = "LeftCtrl";         // Ctrl
            public static readonly string MENU = "LeftAlt";             // Alt
            public static readonly string PAUSE = "Pause";              // Pause
            public static readonly string CAPITAL = "Capital";          // Caps Lock
            public static readonly string ESCAPE = "Escape";            // Esc
            public static readonly string SPACE = "Space";              // Space
            public static readonly string PRIOR = "PageUp";             // Page Up
            public static readonly string NEXT = "Next";                // Page Down
            public static readonly string END = "End";                  // End
            public static readonly string HOME = "Home";                // Home
            public static readonly string LEFT = "Left";                // Left Arrow
            public static readonly string UP = "Up";                    // Up Arrow
            public static readonly string RIGHT = "Right";              // Right Arrow
            public static readonly string DOWN = "Down";                // Down Arrow
            public static readonly string SELECT = "Select";            // Select
            public static readonly string PRINT = "Print";              // Print
            public static readonly string EXECUTE = "Execute";          // Execute
            public static readonly string SNAPSHOT = "Snapshot";        // Snapshot
            public static readonly string INSERT = "Insert";            // Insert
            public static readonly string DELETE = "Delete";            // Delete
            public static readonly string HELP = "Help";                // Help
            public static readonly string NUM0 = "D0";
            public static readonly string NUM1 = "D1";
            public static readonly string NUM2 = "D2";
            public static readonly string NUM3 = "D3";
            public static readonly string NUM4 = "D4";
            public static readonly string NUM5 = "D5";
            public static readonly string NUM6 = "D6";
            public static readonly string NUM7 = "D7";
            public static readonly string NUM8 = "D8";
            public static readonly string NUM9 = "D9";
            public static readonly string A = "A";
            public static readonly string B = "B";
            public static readonly string C = "C";
            public static readonly string D = "D";
            public static readonly string E = "E";
            public static readonly string F = "F";
            public static readonly string G = "G";
            public static readonly string H = "H";
            public static readonly string I = "I";
            public static readonly string J = "J";
            public static readonly string K = "K";
            public static readonly string L = "L";
            public static readonly string M = "M";
            public static readonly string N = "N";
            public static readonly string O = "O";
            public static readonly string P = "P";
            public static readonly string Q = "Q";
            public static readonly string R = "R";
            public static readonly string S = "S";
            public static readonly string T = "T";
            public static readonly string U = "U";
            public static readonly string V = "V";
            public static readonly string W = "W";
            public static readonly string X = "X";
            public static readonly string Y = "Y";
            public static readonly string Z = "Z";
            public static readonly string LWIN = "LWin";                // 左WIN键
            public static readonly string RWIN = "RWin";                // 右WIN键
            public static readonly string APPS = "Apps";                // 应用程序键
            public static readonly string SLEEP = "Sleep";              // 睡眠键
            public static readonly string NUMPAD0 = "NumPad0";          // 小键盘 0
            public static readonly string NUMPAD1 = "NumPad1";          // 小键盘 1
            public static readonly string NUMPAD2 = "NumPad2";          // 小键盘 2
            public static readonly string NUMPAD3 = "NumPad3";          // 小键盘 3
            public static readonly string NUMPAD4 = "NumPad4";          // 小键盘 4
            public static readonly string NUMPAD5 = "NumPad5";          // 小键盘 5
            public static readonly string NUMPAD6 = "NumPad6";          // 小键盘 6
            public static readonly string NUMPAD7 = "NumPad7";          // 小键盘 7
            public static readonly string NUMPAD8 = "NumPad8";          // 小键盘 8
            public static readonly string NUMPAD9 = "NumPad9";          // 小键盘 9
            public static readonly string MULTIPLY = "Multiply";        // 小键盘 *
            public static readonly string ADD = "Add";                  // 小键盘 +
            public static readonly string SEPARATOR = "Return";         // 小键盘 Enter
            public static readonly string SUBTRACT = "Subtract";        // 小键盘 -
            public static readonly string DECIMAL = "Decimal";          // 小键盘 .
            public static readonly string DIVIDE = "Divide";            // 小键盘 /
            public static readonly string F1 = "F1";                    // F1
            public static readonly string F2 = "F2";                    // F2
            public static readonly string F3 = "F3";                    // F3
            public static readonly string F4 = "F4";                    // F4
            public static readonly string F5 = "F5";                    // F5
            public static readonly string F6 = "F6";                    // F6
            public static readonly string F7 = "F7";                    // F7
            public static readonly string F8 = "F8";                    // F8
            public static readonly string F9 = "F9";                    // F9
            public static readonly string F10 = "F10";                  // F10
            public static readonly string F11 = "F11";                  // F11
            public static readonly string F12 = "F12";                  // F12
            public static readonly string NUMLOCK = "NumLock";          // Num Lock
            public static readonly string SCROLL = "Scroll";            // Scroll
            public static readonly string LSHIFT = "LeftShift";         // 左shift
            public static readonly string RSHIFT = "RightShift";        // 右shift
            public static readonly string LCONTROL = "LeftCtrl";
            public static readonly string RCONTROL = "RightCtrl";
            public static readonly string LMENU = "LeftAlt";
            public static readonly string RMENU = "RightAlt";
            public static readonly string OEM_1 = "Oem1";               // ; :
            public static readonly string OEM_PLUS = "OemPlus";         // = +
            public static readonly string OEM_COMMA = "OemComma";       // ,
            public static readonly string OEM_MINUS = "OemMinus";       // - _
            public static readonly string OEM_PERIOD = "OemPeriod";     // .
            public static readonly string OEM_2 = "OemQuestion";        // / ?
            public static readonly string OEM_3 = "Oem3";               // ` ~
            public static readonly string OEM_4 = "OemOpenBrackets";    // [ {
            public static readonly string OEM_5 = "Oem5";               // \ |
            public static readonly string OEM_6 = "Oem6";               // ] }
            public static readonly string OEM_7 = "OemQuotes";		    // ' "
            public static readonly string OEM_8 = "";
            public static readonly string OEM_102 = "OemBackslash";
            public static readonly string OEM_CLEAR = "";
        }
    }
}
