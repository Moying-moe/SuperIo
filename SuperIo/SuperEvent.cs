/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static SuperIo.SuperIo;

namespace SuperIo
{
    /// <summary>
    /// Provide the ability to get user's keyboard or mouse input
    /// </summary>
    public sealed class SuperEvent
    {
        #region Singleton
        private static readonly Lazy<SuperEvent> lazy = new Lazy<SuperEvent>(() => new SuperEvent());
        /// <summary>
        /// Instance
        /// </summary>
        public static SuperEvent Instance { get { return lazy.Value; } }
        #endregion

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

        [StructLayout(LayoutKind.Sequential)]
        private class tagKBDLLHOOKSTRUCT
        {
            public int vkCode;//虚拟键盘码
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 鼠标结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class tagMSLLHOOKSTRUCT
        {
            public POINT pt; // 鼠标位置
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 鼠标位置结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        const int WM_KEYDOWN = 0x0100;//键盘按键按下对应的值
        const int WM_KEYUP = 0x0101;//键盘按键抬起对应的值

        private const int WM_MOUSEMOVE = 0x200;// 鼠标移动
        private const int WM_LBUTTONDOWN = 0x201;// 鼠标左键按下
        private const int WM_RBUTTONDOWN = 0x204;// 鼠标右键按下
        private const int WM_MBUTTONDOWN = 0x207;// 鼠标中键按下
        private const int WM_LBUTTONUP = 0x202;// 鼠标左键抬起
        private const int WM_RBUTTONUP = 0x205;// 鼠标右键抬起
        private const int WM_MBUTTONUP = 0x208;// 鼠标中键抬起
        private const int WM_MOUSEWHEEL = 0x20a;// 鼠标滚轮
        private const int WM_XBUTTONDOWN = 0x20b;// 侧键按下
        private const int WM_XBUTTONUP = 0x20c;// 侧键松开
        private const int WM_XBUTTONDBLCLK = 0x20d;// 侧键双击
        private const int WM_XBUTTONDOWN_A = 0x0ab;// 侧键按下（另一情况 下同）
        private const int WM_XBUTTONUP_A = 0x0ac;// 侧键松开
        private const int WM_XBUTTONDBLCLK_A = 0x0ad;// 侧键双击

        private const int MK_XB1 = 0x00010000;// XB1触发了按键
        private const int MK_XB2 = 0x00020000;// XB2触发了按键
        #endregion

        #region Initialization
        private bool _initialized = false;        // 模块是否已经初始化

        /// <summary>
        /// Is the module initialized successfully
        /// </summary>
        public bool IsInitialized { get => _initialized; }

        /// 键盘hook
        private int _setWindowsHookExReturnKeyBoard;
        private HOOKPROC _keyboardProc;
        private int _keyboardInvokeId = 0;
        private int _keyboardEventId = 0;

        // 鼠标hook
        private int _setWindowsHookExReturnMouse;
        private HOOKPROC _mouseProc;
        private int _mouseInvokeId = 0;
        private int _mouseEventId = 0;

        /// <summary>
        /// Initialization
        /// </summary>
        private SuperEvent()
        {
            _keyboardProc = new HOOKPROC(HookProcMathodKeyBoard);//创建委托变量
            Process curProcess = Process.GetCurrentProcess();//获取窗体句柄
            ProcessModule curModule = curProcess.MainModule;
            _setWindowsHookExReturnKeyBoard = SetWindowsHookEx(13, _keyboardProc, GetModuleHandle(curModule.ModuleName), 0);

            _mouseProc = new HOOKPROC(MouseHookProc);//创建委托变量
            _setWindowsHookExReturnMouse = SetWindowsHookEx(14, _mouseProc, GetModuleHandle(curModule.ModuleName), 0);

            _initialized = true;
        }

        private void CheckInitialization()
        {
            if (!_initialized)
            {
                throw new Exception("SuperEvent has not initialized yet.");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        ~SuperEvent()
        {
            Dispose();
        }

        /// <summary>
        /// Call in application quit event.
        /// </summary>
        public void Dispose()
        {
            // 解除键盘钩子
            CheckInitialization();
            _initialized = false;

            UnhookWindowsHookEx(_setWindowsHookExReturnKeyBoard);
            UnhookWindowsHookEx(_setWindowsHookExReturnMouse);
        }

        #endregion


        // 修饰键按压状态
        private ushort _ctrlHolding = 0;
        private ushort _altHolding = 0;
        private ushort _shiftHolding = 0;

        /// <summary>
        /// Event handler
        /// </summary>
        public delegate void EventHandler();

        #region KeyEvent
        /// <summary>
        /// Global input event handler
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isKeyDown"></param>
        /// <param name="isKeyUp"></param>
        /// <returns></returns>
        public delegate bool GlobalKeyHandler(byte key, bool isKeyDown, bool isKeyUp);

        /// <summary>
        /// A event info struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class KeyEventHandlerStruct
        {
            /// <summary>
            /// Monitored key
            /// </summary>
            public byte Key;
            /// <summary>
            /// If button ctrl needs to be held
            /// </summary>
            public bool Ctrl = false;
            /// <summary>
            /// If button alt needs to be held
            /// </summary>
            public bool Alt = false;
            /// <summary>
            /// If button shift needs to be held
            /// </summary>
            public bool Shift = false;
            /// <summary>
            /// Handler for key down event
            /// </summary>
            public EventHandler OnKeyDown;
            /// <summary>
            /// Handler for key up event
            /// </summary>
            public EventHandler OnKeyUp;
        }

        private Dictionary<byte, Dictionary<int, KeyEventHandlerStruct>> _registeredKeyEvents = new Dictionary<byte, Dictionary<int, KeyEventHandlerStruct>>(); // 注册的事件
        private Dictionary<int, byte> _keyEventIdToKey = new Dictionary<int, byte>(); // eventId到key的映射
        private Dictionary<int, GlobalKeyHandler> _invokeKeyMethods = new Dictionary<int, GlobalKeyHandler>(); // 注入的方法
        private HashSet<byte> _keyStatus = new HashSet<byte>(); // 每个键的按压状态

        private byte lastPressKey = 0;

        /// <summary>
        /// Last pressed key
        /// </summary>
        public byte LastPressKey { get => lastPressKey; }

        private int HookProcMathodKeyBoard(int code, int wParam, IntPtr lParam)
        {
            // 处理键盘事件
            CheckInitialization();

            tagKBDLLHOOKSTRUCT v = (tagKBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(tagKBDLLHOOKSTRUCT));//将捕获的键盘信息存储到存储键盘信息的结构体中

            byte key = (byte)v.vkCode;

            if (code >= 0)//如果code的值大于0说明获取到了按键输入
            {
                bool isKeyDown = false;
                bool isKeyUp = false;

                if (wParam == WM_KEYDOWN)
                {
                    if (!_keyStatus.Contains(key))
                    {
                        _keyStatus.Add(key);
                        isKeyDown = true;
                    }
                }
                if (wParam == WM_KEYUP)
                {
                    if (_keyStatus.Contains(key))
                    {
                        _keyStatus.Remove(key);
                        isKeyUp = true;
                    }
                }

                if (!isKeyDown && !isKeyUp)
                {
                    // 如果只是Holding事件 则直接结束
                    return CallNextHookEx(_setWindowsHookExReturnKeyBoard, code, wParam, lParam);
                }
                else if (isKeyDown)
                {
                    // 更新last press key
                    lastPressKey = key;
                }


                // invoke methods
                foreach (KeyValuePair<int, GlobalKeyHandler> pair in _invokeKeyMethods)
                {
                    bool handlerResult = pair.Value(key, isKeyDown, isKeyUp);
                    if (! handlerResult)
                    {
                        // 如果GlobalKeyHandler返回了false 则阻止它激活接下来的逻辑
                        return CallNextHookEx(_setWindowsHookExReturnKeyBoard, code, wParam, lParam);
                    }
                }

                #region 功能键按压情况
                if (key == Key.VK_CONTROL || key == Key.VK_LCONTROL || key == Key.VK_RCONTROL)
                {
                    if (isKeyDown)
                    {
                        _ctrlHolding++;
                    }
                    if (isKeyUp)
                    {
                        if (_ctrlHolding > 0)
                        {
                            _ctrlHolding--;
                        }
                    }
                }
                if (key == Key.VK_MENU || key == Key.VK_LMENU || key == Key.VK_RMENU)
                {
                    if (isKeyDown)
                    {
                        _altHolding++;
                    }
                    if (isKeyUp)
                    {
                        if (_altHolding > 0)
                        {
                            _altHolding--;
                        }
                    }
                }
                if (key == Key.VK_SHIFT || key == Key.VK_LSHIFT || key == Key.VK_RSHIFT)
                {
                    if (isKeyDown)
                    {
                        _shiftHolding++;
                    }
                    if (isKeyUp)
                    {
                        if (_shiftHolding > 0)
                        {
                            _shiftHolding--;
                        }
                    }
                }
                #endregion

                Dictionary<int, KeyEventHandlerStruct> handlerDict;
                if (_registeredKeyEvents.TryGetValue(key, out handlerDict))
                {
                    foreach (KeyValuePair<int, KeyEventHandlerStruct> pair in handlerDict)
                    {
                        KeyEventHandlerStruct handler = pair.Value;

                        if (!((handler.Ctrl && _ctrlHolding == 0) ||
                              (handler.Alt && _altHolding == 0) ||
                              (handler.Shift && _shiftHolding == 0)))
                        {
                            // 如果应当按下的功能键没有按下 那么此handler实际上没有被激活 反之 就是激活了
                            if (isKeyDown)//检测到键盘按下
                            {
                                handler.OnKeyDown();
                            }
                            if (isKeyUp)//检测到键盘抬起
                            {
                                handler.OnKeyUp();
                            }
                        }
                    }
                }
            }
            return CallNextHookEx(_setWindowsHookExReturnKeyBoard, code, wParam, lParam);//将按键值传
                                                                                        //递给下一个钩子
        }

        

        /// <summary>
        /// <para>Register a key event.</para>
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperEvent!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <returns>Return the registered hotkey id.</returns>
        public int RegisterKey(KeyEventHandlerStruct handler)
        {
            CheckInitialization();

            byte key = handler.Key;

            int newEventId = _keyboardEventId;
            _keyboardEventId++;

            if (!_registeredKeyEvents.ContainsKey(key))
            {
                _registeredKeyEvents.Add(key, new Dictionary<int, KeyEventHandlerStruct>());
            }
            _registeredKeyEvents[key].Add(newEventId, handler);
            _keyEventIdToKey.Add(newEventId, key);

            return newEventId;
        }
        /// <summary>
        /// Register a key event.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperEvent!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="key">Key that will trigger the handler</param>
        /// <param name="keyDownHandler">Key down handler</param>
        /// <param name="keyUpHandler">Key up handler</param>
        /// <returns>Return the registered hotkey id.</returns>
        public int RegisterKey(byte key, EventHandler keyDownHandler, EventHandler keyUpHandler)
        {
            CheckInitialization();

            return RegisterKey(new KeyEventHandlerStruct()
            {
                Key = key,
                OnKeyDown = keyDownHandler,
                OnKeyUp = keyUpHandler
            });
        }
        /// <summary>
        /// Register a key event.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperEvent!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="key">Key that will trigger the handler</param>
        /// <param name="keyDownHandler">Key down handler</param>
        /// <param name="keyUpHandler">Key up handler</param>
        /// <param name="ctrl"></param>
        /// <param name="alt"></param>
        /// <param name="shift"></param>
        /// <returns>Return the registered hotkey id.</returns>
        public int RegisterKey(byte key, EventHandler keyDownHandler, EventHandler keyUpHandler,
            bool ctrl = false, bool alt = false, bool shift = false)
        {
            CheckInitialization();

            return RegisterKey(new KeyEventHandlerStruct()
            {
                Key = key,
                Ctrl = ctrl,
                Alt = alt,
                Shift = shift,
                OnKeyDown = keyDownHandler,
                OnKeyUp = keyUpHandler
            });
        }
        /// <summary>
        /// Unregister an exist key event.
        /// </summary>
        /// <param name="eventId">Event id that `RegisterKey` returned</param>
        /// <returns></returns>
        public bool UnregisterKey(int eventId)
        {
            CheckInitialization();

            byte key;
            if (!_keyEventIdToKey.TryGetValue(eventId, out key))
            {
                // 没有这个event id
                return false;
            }

            _keyEventIdToKey.Remove(eventId);
            _registeredKeyEvents[key].Remove(eventId);
            if (_registeredKeyEvents[key].Count == 0)
            {
                _registeredKeyEvents.Remove(key);
            }
            return true;
        }
        /// <summary>
        /// Unregister all key events.
        /// </summary>
        public void UnregisterAllKeys()
        {
            _keyEventIdToKey.Clear();
            _registeredKeyEvents.Clear();
        }

        /// <summary>
        /// Add a global key handler. Which will triggered everytime user press a key.
        /// </summary>
        /// <param name="newHandler"></param>
        /// <returns>handler id</returns>
        public int AddGlobalKeyHandler(GlobalKeyHandler newHandler)
        {
            CheckInitialization();

            int newInvokeId = _keyboardInvokeId;
            _keyboardInvokeId++;
            _invokeKeyMethods.Add(newInvokeId, newHandler);
            return newInvokeId;
        }
        /// <summary>
        /// Remove a global key handler with its id.
        /// </summary>
        /// <param name="handlerId"></param>
        /// <returns>Return false if handler which handlerId specified is not exists.</returns>
        public bool RemoveGlobalKeyHandler(int handlerId)
        {
            CheckInitialization();

            return _invokeKeyMethods.Remove(handlerId);
        }
        /// <summary>
        /// Remove all global key handlers.
        /// </summary>
        public void RemoveAllGlobalKeyHandlers()
        {
            CheckInitialization();

            _invokeKeyMethods.Clear();
        }

        #endregion


        #region MouseEvent

        /// <summary>
        /// Global input event handler
        /// </summary>
        /// <param name="mouseEvent"></param>
        /// <returns></returns>
        public delegate bool GlobalMouseHandler(byte mouseEvent);

        /// <summary>
        /// A event info struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseEventHandlerStruct
        {
            /// <summary>
            /// Monitored event
            /// </summary>
            public byte MouseEvent;
            /// <summary>
            /// If button ctrl needs to be held
            /// </summary>
            public bool Ctrl = false;
            /// <summary>
            /// If button alt needs to be held
            /// </summary>
            public bool Alt = false;
            /// <summary>
            /// If button shift needs to be held
            /// </summary>
            public bool Shift = false;
            /// <summary>
            /// Handler for this mouse event
            /// </summary>
            public EventHandler Handler;

        }

        private Dictionary<byte, Dictionary<int, MouseEventHandlerStruct>> _registeredMouseEvents = new Dictionary<byte, Dictionary<int, MouseEventHandlerStruct>>(); // 注册的事件
        private Dictionary<int, byte> _mouseEventIdToMouse = new Dictionary<int, byte>(); // event id到mouse的映射
        private Dictionary<int, GlobalMouseHandler> _invokeMouseMethods = new Dictionary<int, GlobalMouseHandler>(); // 注入的方法
        private HashSet<byte> _mouseStatus = new HashSet<byte>(); // 每个键的按压状态

        private POINT mouseLastPosition = new POINT
        {
            x = 0,
            y = 0
        };

        /// <summary>
        /// Mouse's last position
        /// </summary>
        public POINT MouseLastPosition { get => mouseLastPosition; }

        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // 鼠标事件
            CheckInitialization();

            if ((nCode >= 0))
            {
                tagMSLLHOOKSTRUCT hookStruct = (tagMSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(tagMSLLHOOKSTRUCT));

                byte eventValue = 0;

                switch (wParam)
                {
                    case WM_MOUSEMOVE:
                        eventValue = Mouse.MOUSEMOVE;
                        mouseLastPosition = hookStruct.pt;
                        break;
                    case WM_LBUTTONDOWN:
                        eventValue = Mouse.LBUTTONDOWN;
                        break;
                    case WM_RBUTTONDOWN:
                        eventValue = Mouse.RBUTTONDOWN;
                        break;
                    case WM_MBUTTONDOWN:
                        eventValue = Mouse.MBUTTONDOWN;
                        break;
                    case WM_LBUTTONUP:
                        eventValue = Mouse.LBUTTONUP;
                        break;
                    case WM_RBUTTONUP:
                        eventValue = Mouse.RBUTTONUP;
                        break;
                    case WM_MBUTTONUP:
                        eventValue = Mouse.MBUTTONUP;
                        break;
                    case WM_MOUSEWHEEL:
                        eventValue = Mouse.MOUSEWHEEL;
                        break;
                    case WM_XBUTTONDOWN:
                    case WM_XBUTTONDOWN_A:
                    case WM_XBUTTONDBLCLK:
                    case WM_XBUTTONDBLCLK_A:
                        if ((hookStruct.mouseData & MK_XB1) > 0)
                        {
                            eventValue = Mouse.XBUTTON1DOWN;
                        }
                        else if ((hookStruct.mouseData & MK_XB2) > 0)
                        {
                            eventValue = Mouse.XBUTTON2DOWN;
                        }
                        break;
                    case WM_XBUTTONUP:
                    case WM_XBUTTONUP_A:
                        if ((hookStruct.mouseData & MK_XB1) > 0)
                        {
                            eventValue = Mouse.XBUTTON1UP;
                        }
                        else if ((hookStruct.mouseData & MK_XB2) > 0)
                        {
                            eventValue = Mouse.XBUTTON2UP;
                        }
                        break;
                    default:
                        eventValue = 0;
                        break;
                }

                // invoke methods
                foreach (KeyValuePair<int, GlobalMouseHandler> pair in _invokeMouseMethods)
                {
                    bool handlerResult = pair.Value(eventValue);
                    if (!handlerResult)
                    {
                        // 如果GlobalKeyHandler返回了false 则阻止它激活接下来的逻辑
                        return CallNextHookEx(_setWindowsHookExReturnMouse, nCode, wParam, lParam);
                    }
                }

                Dictionary<int, MouseEventHandlerStruct> handlerDict;
                if (_registeredMouseEvents.TryGetValue(eventValue, out handlerDict))
                {
                    foreach (KeyValuePair<int, MouseEventHandlerStruct> pair in handlerDict)
                    {
                        MouseEventHandlerStruct handler = pair.Value;
                        if (!((handler.Ctrl && _ctrlHolding == 0) ||
                              (handler.Alt && _altHolding == 0) ||
                              (handler.Shift && _shiftHolding == 0)))
                        {
                            handler.Handler();
                        }
                    }
                }
            }

            return CallNextHookEx(_setWindowsHookExReturnMouse, nCode, wParam, lParam);
        }

        /// <summary>
        /// <para>Register a mouse event.</para>
        /// <para><b>WARNING: SuperMouse's simulation will also trigger SuperEvent!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <returns>Return the registered mouse event id.</returns>
        public int RegisterMouse(MouseEventHandlerStruct handler)
        {
            CheckInitialization();

            byte mEvent = handler.MouseEvent;
            int newEventId = _mouseEventId;
            _mouseEventId++;

            if (!_registeredMouseEvents.ContainsKey(mEvent))
            {
                _registeredMouseEvents.Add(mEvent, new Dictionary<int, MouseEventHandlerStruct>());
            }
            _registeredMouseEvents[mEvent].Add(newEventId, handler);
            _mouseEventIdToMouse.Add(newEventId, mEvent);
            return newEventId;
        }
        /// <summary>
        /// Register a mouse event.
        /// <para><b>WARNING: SuperMouse's simulation will also trigger SuperEvent!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="mouseEvent">Mouse event that will trigger the handler</param>
        /// <param name="handler">Event handler</param>
        /// <returns>Return the registered mouse event id.</returns>
        public int RegisterMouse(byte mouseEvent, EventHandler handler)
        {
            CheckInitialization();

            return RegisterMouse(new MouseEventHandlerStruct()
            {
                MouseEvent = mouseEvent,
                Handler = handler
            });
        }
        /// <summary>
        /// Register a mouse event.
        /// <para><b>WARNING: SuperMouse's simulation will also trigger SuperEvent!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="mouseEvent">Mouse event that will trigger the handler</param>
        /// <param name="handler">Event handler</param>
        /// <param name="ctrl"></param>
        /// <param name="alt"></param>
        /// <param name="shift"></param>
        /// <returns>Return the registered mouse event id.</returns>
        public int RegisterMouse(byte mouseEvent, EventHandler handler, 
            bool ctrl = false, bool alt = false, bool shift = false)
        {
            CheckInitialization();

            return RegisterMouse(new MouseEventHandlerStruct()
            {
                MouseEvent = mouseEvent,
                Ctrl = ctrl,
                Alt = alt,
                Shift = shift,
                Handler = handler
            });
        }
        /// <summary>
        /// Unregister an exist mouse event.
        /// </summary>
        /// <param name="eventId">Event it that `RegisterMouse` returned</param>
        /// <returns></returns>
        public bool UnregisterMouse(int eventId)
        {
            CheckInitialization();


            byte mEvent;
            if (!_mouseEventIdToMouse.TryGetValue(eventId, out mEvent))
            {
                // 没有这个event id
                return false;
            }

            _mouseEventIdToMouse.Remove(eventId);
            _registeredMouseEvents[mEvent].Remove(eventId);
            if (_registeredMouseEvents[mEvent].Count == 0)
            {
                _registeredMouseEvents.Remove(mEvent);
            }
            return true;
        }
        /// <summary>
        /// Unregister all mouse events.
        /// </summary>
        public void UnregisterAllMouses()
        {
            _registeredMouseEvents.Clear();
        }

        /// <summary>
        /// Add a global mouse handler. Which will triggered everytime user use his mouse.
        /// </summary>
        /// <param name="newHandler"></param>
        /// <returns>handler id</returns>
        public int AddGlobalMouseHandler(GlobalMouseHandler newHandler)
        {
            CheckInitialization();

            int newInvokeId = _mouseInvokeId;
            _mouseInvokeId++;
            _invokeMouseMethods.Add(newInvokeId, newHandler);
            return newInvokeId;
        }
        /// <summary>
        /// Remove a global mouse handler with its id.
        /// </summary>
        /// <param name="handlerId"></param>
        /// <returns>Return false if handler which handlerId specified is not exists.</returns>
        public bool RemoveGlobalMouseHandler(int handlerId)
        {
            CheckInitialization();

            return _invokeMouseMethods.Remove(handlerId);
        }
        /// <summary>
        /// Remove all global mouse handlers.
        /// </summary>
        public void RemoveAllGlobalMouseHandlers()
        {
            CheckInitialization();

            _invokeMouseMethods.Clear();
        }

        #endregion
    }
}
