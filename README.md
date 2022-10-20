# SuperIo

[![License-GPL-3.0](https://img.shields.io/badge/license-GPL--3.0-orange)](https://github.com/Moying-moe/SuperIo/blob/master/LICENSE)
![version v0.1.0](https://img.shields.io/badge/version-v0.1.0--alpha-green)

---

## Introduction

*SuperIo* is a IO library for *DotNet Framework*.

With *SuperIo*, you can create an automatic tool that can control your mouse and keyboard.

*SuperIo* consists of four modules:

- SuperKeyboard: Provides the ability to manipulate the keyboard through code.
- SuperMouse: Provides the ability to manipulate the mouse through code.
- SuperScreen: Provides the ability to get the specified pixel's color of the screen and color comparison
- SuperKeyHook: Provides the ability about global hotkeys.

---

## Import to your project

CAUTION: **SuperIo currently only runs on 64-bit systems. And will support 32-bit systems in the future.**

You can download the compiled DLL file in [Github Releases Page](https://github.com/Moying-moe/SuperIo/releases). Or you can also clone the repository and build it by yourself.

It contains 3 files. `SuperIo.dll`, `WinRing0x64.dll` and `WinRing0x64.sys`. You need to copy these 3 files to your project's root.

(WinRing0 files can be found in [This Repository](https://github.com/QCute/WinRing0) if you build these files by yourself.)

Then, reference `SuperIo.dll` in your project. Import it with these codes.

```C#
using SuperIo;
```

When your project is built, copy `WinRing0x64.dll` and `WinRing0x64.sys` to your application's root.

Or you can also add these files into your project if you are using *Visual Studio* or other IDE. It will automatically copy these files.

---

## Usage

*Also see in [SuperIoTestProgram](https://github.com/Moying-moe/SuperIo/tree/master/SuperIoTestProgram).

### SuperKeyboard

`SuperKeyboard` use *WinRing0* to implement its functions. So it works on most apps and games.

You need to initialize `SuperKeyboard` before actual use.

```C#
using SuperIo;

// ......
if (SuperKeyboard.Initialize())
{
    Console.WriteLine("Initialization success.");
}
else
{
    Console.WriteLine("Initialization fail.");
}
// ......
```

It is recommended to initialize at the beginning of the program. Because `SuperKeyboard` need about 0.2 - 1.0s to initialize.

You can call `KeyPress(byte)` to simulate a keyboard press.

```C#
SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_A);
```

In this case, we used the keycode constant in `SuperKeyboard.Key`. The program will simulate a press on the key **A**.

`SuperKeyboard` simulates the native input of the PS/2 keyboard. So, if you want to input an **uppercase A**, here is the code.

```C#
SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_CAPITAL);
SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_A);
SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_CAPITAL);
// or
SuperKeyboard.KeyDown(SuperKeyboard.Key.VK_SHIFT);
SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_A);
SuperKeyboard.KeyUp(SuperKeyboard.Key.VK_SHIFT);
```

Here are 2 new methods! `KeyDown(byte)` and `KeyUp(byte)`. Which literally means to press down a key or release a key. In this case, we pressed down **Shift** and hold it, then pressed **A** , and then released **Shift**.

KeyPress is implemented by calling KeyDown and KeyUp. *SuperIo* will automatically add a delay between these two action. The delay defaults to 50 milliseconds. You can set it by calling `SetKeyPressDelay(int)`.

```C#
SuperKeyboard.SetKeyPressDelay(20);
Console.WriteLine(SuperKeyboard.GetKeyPressDelay()); // => 20
```

### SuperMouse

`SuperMouse` use *user32.mouse_event* to implement its functions. So it works on majority of apps and games.

Like `SuperKeyboard`, you need to initialize `SuperMouse` module before actual use.

```C#
using SuperIo;

// ......
if (SuperMouse.Initialize())
{
    Console.WriteLine("Initialization success.");
}
else
{
    Console.WriteLine("Initialization fail.");
}
// ......
```

You can call `MoveRelative(int dx, int dy)` to move your mouse relatively. The distance unit is pixel.

```C#
SuperMouse.MoveRelative(50, 50); // Move the mouse down to the right
```

Or, you can call `MoveTo(int x, int y)` to move your mouse to a specified position.

```C#
SuperMouse.MoveTo(960, 540);
```

Method `MoveTo` need to know the size of the screen so that it can move the mouse to the position correctly.

When `SuperMouse` initialized, it will try to get the size of the screen. In most situation, it works properly. If you find out that your mouse move to a wrong position when you call `MoveTo`, you can specify the screen size at initialization.

```C#
// ......
SuperMouse.Initialize(1920, 1080);
// ......
```

You can call `LButtonClick()` to simulate a mouse's left button click. You can also call `LButtonDown()` and `LButtonUp()` to hold the left button or release it.

```C#
SuperMouse.LButtonClick();      // Simply left-click.

// Drag
SuperMouse.LButtonDown();
SuperMouse.MoveRelative(50, 0);
SuperMouse.LButtonUp();
```

With `LButtonClick(int)`, you can create multiple clicks with just one line of code.

```C#
SuperMouse.LButtonClick(2);     // Double Click
```

It will be a delay between two clicks. Default to 50ms. Call `SetMultClickDelay(int)` to change this delay.

```C#
SetMultClickDelay(20);
Console.WriteLine(GetMultClickDelay()); // => 20
```

There are also right button and middle button(mouse wheel) version of these methods.

- RButtonClick(), RButtonClick(int)
- RButtonDown()
- RButtonUp()
- MButtonClick(), MButtonClick(int)
- MButtonDown()
- MButtonUp()

You can call `Scroll(int)` to scroll the page with the mouse wheel. Also, `ScrollUp()` and `ScrollDown()` give you a shortcut to code a simple scroll action.

```C#
SuperMouse.Scroll(120);     // Scroll upwards(Towards away from your body)
SuperMouse.ScrollUp();      // Same as code above

SuperMouse.Scroll(-120);    // Scroll downwards(Towards your body)
SuperMouse.ScrollDown();    // Same as code above
```

### SuperScreen

`SuperScreen` use *user32* and *gdi32* to implement its functions. So it works on most apps and games.

Unlike modules above, `SuperScreen` does not need to initialized before use.

Call `GetPixelColor(int x, int y)` to get the color of the pixel at (x, y).

```C#
using System.Drawing;

// ......
Color color = SuperScreen.GetPixelColor(960, 540);
Console.WriteLine(
    color.R + ", " +
    color.G + ", " +
    color.B
);
// Possible output: 255, 90, 90
// ......
```

Call `ColorDifference(Color, Color)` to compare 2 colors.

```C#
Color black = Color.FromArgb(0,0,0);
Color white = Color.FromArgb(255,255,255);
Color prettyRed = Color.FromArgb(255,90,90);

SuperScreen.ColorDifference(black, black);      // => 0.0
SuperScreen.ColorDifference(black, white);      // => 1.0
SuperScreen.ColorDifference(white, prettyRed);  // => 0.528321...
```

Call `IsColorAt(int x, int y, Color target)` or `IsColorAt(int x, int y, Color target, double similarity)` to get if the color at (x, y) is similar to (or same as) target color.

```C#
// For example, the color at (960, 540) is 255,85,85
Color prettyRed = Color.FromArgb(255,90,90);

SuperScreen.IsColorAt(960, 540, prettyRed);         // => false
SuperScreen.IsColorAt(960, 540, prettyRed, 0.95d);  // => true
```

### SuperKeyHook

`SuperKeyHook` use Windows Hook to implement its functions. So it works on most apps and games.

Unlike modules above, you need to **instantiate** `SuperKeyHook` to use it.

```C#
using SuperIo;

// ......
SuperKeyHook hotkeyManager = new SuperKeyHook();
// ......
```

Notice that you need to keep this object alive. If you instantiate this object as a local variable, it may be removed by garbage collection (GC). Try to declare it as a class attribute, or use `GC.KeepAlive(hotkeyManager);`

Then, register hotkeys you want to bind.

```C#
SuperKeyHook.KeyHookHandlerStruct handler =
        new SuperKeyHook.KeyHookHandlerStruct()
            {
                OnKeyDown = delegate ()
                {
                    Console.WriteLine("Q down");
                },
                OnKeyUp = delegate ()
                {
                    Console.WriteLine("Q up");
                }
            };
hotkeyManager.Register(SuperKeyHook.Key.Q, handler);

// or

hotkeyManager.Register(
    SuperKeyHook.Key.Q,
    delegate ()
    {
        Console.WriteLine("Q down");
    },
    delegate ()
    {
        Console.WriteLine("Q up");
    }
);
```

`OnKeyDown` Event will triggered only one time. In other words, `OnKeyDown` is triggered at the top edge. Similarly, `OnKeyUp` is triggered at the bottom edge.

**WARNING!!!**

Because `SuperKeyboard` can generate native input of the PS2 keyboard. So `SuperKeyboard.KeyPress` **WILL TRIGGER** `SuperKeyHook`'s hook! This may case **UNEXPECT recursive call**!

```C#
hotkeyManager.Register(
    SuperKeyHook.Key.Q,
    delegate ()
    {
        Console.WriteLine("You press the hotkey");
        // want to input "quit" and return
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_Q); // WILL CAUSE ENDLESS RECURSIVE CALL!!!
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_U);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_I);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_T);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_RETURN);
    },
    delegate () { }
);
```

It can be avoided by using combination key as hotkey. But combination key is not supported native in current version.

You can use the following code as a substitute.

```C#
bool IsShiftDown = false;
hotkeyManager.Register(
    SuperKeyHook.Key.SHIFT,
    delegate ()
    {
        IsShiftDown = true;
    },
    delegate ()
    {
        IsShiftDown = false;
    }
);
hotkeyManager.Register(
    SuperKeyHook.Key.Q,
    delegate () { },
    delegate ()
    {
        if (IsShiftDown) {
            Console.WriteLine("You press the hotkey");
            SuperKeyboard.KeyUp(SuperKeyboard.Key.SHIFT);
            SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_Q);
            SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_U);
            SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_I);
            SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_T);
            SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_RETURN);
        }
    }
);
```
