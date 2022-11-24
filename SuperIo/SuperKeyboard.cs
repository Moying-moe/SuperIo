/*
  Copyright (c) Moying-moe All rights reserved. Licensed under the GPL-3.0 license.
  See LICENSE in the project root for license information.
*/

using System;
using System.Threading;
using static SuperIo.SuperIo;

namespace SuperIo
{
    /// <summary>
    /// Provide the ability to control the keyboard
    /// </summary>
    public sealed class SuperKeyboard
    {
        #region Singleton
        private static readonly Lazy<SuperKeyboard> lazy = new Lazy<SuperKeyboard>(() => new SuperKeyboard());
        /// <summary>
        /// Instance
        /// </summary>
        public static SuperKeyboard Instance { get { return lazy.Value; } }
        #endregion

        private bool _initialized = false;        // 模块是否已经初始化
        private int _keyPressDelay = 50;        // 调用KeyPress时，按下到弹起的时间间隔

        /// <summary>
        /// Is the module initialized successfully
        /// </summary>
        public bool IsInitialized { get => _initialized; }

        #region ArgumentsSetup
        /// <summary>
        /// Initialize the SuperKeyboard module.
        /// </summary>
        private SuperKeyboard()
        {
            _initialized = WinRing0.init();
        }

        /// <summary>
        /// The delay between a key's *down* and *up* when method `KeyPress` is called.
        /// </summary>
        /// <returns></returns>
        public int GetKeyPressDelay()
        {
            return _keyPressDelay;
        }
        /// <summary>
        /// The delay between a key's *down* and *up* when method `KeyPress` is called.
        /// </summary>
        /// <param name="delay">New delay</param>
        public void SetKeyPressDelay(int delay)
        {
            _keyPressDelay = delay;
        }

        private void CheckInitialization()
        {
            if (!_initialized)
            {
                throw new Exception("SuperKeyboard initialization failed.");
            }
        }
        #endregion

        /// <summary>
        /// Press down the key.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        public void KeyDown(byte keycode)
        {
            CheckInitialization();
            WinRing0.KeyDown(keycode);
        }

        /// <summary>
        /// Release the key.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        public void KeyUp(byte keycode)
        {
            CheckInitialization();
            WinRing0.KeyUp(keycode);
        }

        /// <summary>
        /// Press the key one time.
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        public void KeyPress(byte keycode)
        {
            CheckInitialization();
            WinRing0.KeyDown(keycode);
            Thread.Sleep(_keyPressDelay);
            WinRing0.KeyUp(keycode);
        }

        /// <summary>
        /// Press the key one time.
        /// <para>With keys in argument `modFlags` holding.</para>
        /// <para><b>WARNING: SuperKeyboard's simulation will also trigger SuperKeyHook!</b> This may cause unexpect recursive call!</para>
        /// </summary>
        /// <param name="keycode">Key code. Can be found in SuperKeyboard.Key</param>
        /// <param name="modFlags">ModKey flag(s).</param>
        public void KeyPress(byte keycode, int modFlags)
        {
            CheckInitialization();

            bool ctrl = (modFlags & ModKey.CTRL) > 0;
            bool alt = (modFlags & ModKey.ALT) > 0;
            bool shift = (modFlags & ModKey.SHIFT) > 0;
            bool rctrl = (modFlags & ModKey.R_CTRL) > 0;
            bool ralt = (modFlags & ModKey.R_ALT) > 0;
            bool rshift = (modFlags & ModKey.R_SHIFT) > 0;
            bool hasMod = ctrl || alt || shift || rctrl || ralt || rshift;

            if (hasMod)
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

            if (hasMod)
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
        public void KeyCombSeq(params byte[] keycodes)
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
        public void KeyCombSeq(int interval, params byte[] keycodes)
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
    }
}
