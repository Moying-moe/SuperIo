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
    public static class SuperIo
    {
        public static readonly string VERSION = "v1.0.0-alpha";

        /// <summary>
        /// Initialize all modules.
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            bool keyboardFlag = SuperKeyboard.Initialize();
            bool mouseFlag = SuperMouse.Initialize();
            bool screenFlag = SuperScreen.Initialize();
            bool keyhookFlag = SuperKeyHook.Initialize();
            return keyboardFlag && mouseFlag && screenFlag && keyhookFlag;
        }

        /// <summary>
        /// Dispose all modules.
        /// </summary>
        public static void Dispose()
        {
            SuperKeyHook.Dispose();
        }
    }
}
