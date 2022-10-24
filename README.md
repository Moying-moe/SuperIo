# SuperIo

[![License-GPL-3.0](https://img.shields.io/badge/license-GPL--3.0-orange)](https://github.com/Moying-moe/SuperIo/blob/master/LICENSE)
![version v0.3.0](https://img.shields.io/badge/version-v0.3.0--alpha-green)

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

You can download the compiled DLL file in [Github Releases Page](https://github.com/Moying-moe/SuperIo/releases). Or you can also clone the repository and build it by yourself.

It contains 5 files. `SuperIo.dll` and 4 *WinRing0* files(`WinRing0.dll`, `WinRing0.sys`, `WinRing0x64.dll`, `WinRing0x64.sys`). You need to copy these 5 files to your project's root.

(WinRing0 files can be found in [This Repository](https://github.com/QCute/WinRing0) if you build these files by yourself.)

Then, reference `SuperIo.dll` in your project. Import it with these codes.

```C#
using SuperIo;
```

When your project is built, copy *WinRing0* files to your application's root.

Or you can also add these files into your project if you are using *Visual Studio* or other IDE. After proper setups, it will automatically copy these files to your application's root.

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

Console.WriteLine(SuperKeyboard.IsInitialized); // => true
```

It is recommended to initialize at the beginning of the program. Because `SuperKeyboard` need about 0.2 - 1.0s to load *WinRing0* Library.

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

You can also call `KeyPress(byte keycode, int modFlags)` if you just want to press a key with Modifier key (Ctrl, Shift or/and Alt) holding.

```C#
// Ctrl + Shift + A
SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_A, SuperKeyboard.ModKey.CTRL | SuperKeyboard.ModKey.SHIFT);
```

There is another way to achieve this function. Try to call `KeyCombSeq(byte, byte, ...)`.

```C#
// Ctrl + Shift + A.
SuperKeyboard.KeyCombSeq(SuperKeyboard.Key.VK_CTRL, SuperKeyboard.Key.VK_SHIFT, SuperKeyboard.Key.VK_A);
```

KeyPress is implemented by calling KeyDown and KeyUp. *SuperIo* will automatically add a delay between these two actions. The delay defaults to 50 milliseconds. You can set it by calling `SetKeyPressDelay(int)`.

```C#
SuperKeyboard.SetKeyPressDelay(20);
Console.WriteLine(SuperKeyboard.GetKeyPressDelay()); // => 20
```

Also, `KeyCombSeq` will add this Delay between each key. You can also specified this delay with argument `interval`.

```C#
// What will happen:
// Press down Ctrl. Wait 25ms. Press down Shift. Wait 25ms.
// Press A. Wait 25 ms.
// Release Shift. Wait 25ms. Release Ctrl. Method return.
SuperKeyboard.KeyCombSeq(25, SuperKeyboard.Key.VK_CTRL, SuperKeyboard.Key.VK_SHIFT, SuperKeyboard.Key.VK_A);
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


Console.WriteLine(SuperMouse.IsInitialized); // => true
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

Like other modules, you need to initialize `SuperScreen` module before actual use.

```C#
using SuperIo;

// ......
if (SuperScreen.Initialize())
{
    Console.WriteLine("Initialization success.");
}
else
{
    Console.WriteLine("Initialization fail.");
}
// ......

Console.WriteLine(SuperScreen.IsInitialized); // => true
```

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
SuperScreen.ColorDifference(white, prettyRed);  // => 0.528436...
```

Call `IsColorAt(int x, int y, Color target)` or `IsColorAt(int x, int y, Color target, double similarity)` to get if the color at (x, y) is similar to (or same as) target color.

```C#
// For example, the color at (960, 540) is 255,85,85
Color prettyRed = Color.FromArgb(255,90,90);

SuperScreen.IsColorAt(960, 540, prettyRed);         // => false
SuperScreen.IsColorAt(960, 540, prettyRed, 0.95d);  // => true
```

Call `SearchColor` to search a specified color on the screen.

```C#
Color black = Color.FromArgb(0,0,0);
Rectangle area = new Rectangle(100, 100, 800, 600);

// example 1
SuperScreen.SearchColor(black, SuperScreen.SearchDirection.FromLeftTop, area);
// example 2
SuperScreen.SearchColor(black, SuperScreen.SearchDirection.FromRightBottom, area, 0.95d);
// example 3
SuperScreen.SearchColor(black, SuperScreen.SearchDirection.LeftToRight);
// example 4
SuperScreen.SearchColor(black, SuperScreen.SearchDirection.TopToBottom, 0.95d);
```

In example 1, it will search and return first pixel which is black in area (100, 100, 800, 600). It will search from the left-top corner. Searched zone will look like a triangle:

```plain
XXXXOOO
XXXOOOO
XXOOOOO
XOOOOOO
OOOOOOO

X: pixel has been searched
O: pixel has NOT been searched
```

There are plenty of directions: `FromLeftTop`,`FromRightTop`,`FromLeftBottom`,`FromRightBottom`,`LeftToRight`,`RightToLeft`,`TopToBottom`,`BottomToTop` and `FromCenter`(not implemented yet).

In example 2, it will search and return first pixel which is similar to black in the given area. The similarity algorithm is the same as `ColorDifference` uses.

In example 3 and 4, we didn't specify the searching area. So it will be default to the full screen.

### SuperKeyHook

`SuperKeyHook` use Windows Hook to implement its functions. So it works on most apps and games.

Like other modules, you need to initialize `SuperKeyHook` module before actual use.

```C#
using SuperIo;

// ......
if (SuperKeyHook.Initialize())
{
    Console.WriteLine("Initialization success.");
}
else
{
    Console.WriteLine("Initialization fail.");
}
// ......

Console.WriteLine(SuperKeyHook.IsInitialized); // => true
```

Then, register hotkeys you want to bind.

```C#
SuperKeyHook.KeyHookHandlerStruct handler =
        new SuperKeyHook.KeyHookHandlerStruct()
            {
                OnKeyDown = delegate ()
                {
                    Console.WriteLine("Ctrl+Q down");
                },
                OnKeyUp = delegate ()
                {
                    Console.WriteLine("Ctrl+Q up");
                },
                Ctrl = true,
                Alt = false,
                Shift = false
            };
SuperKeyHook.Register(SuperKeyHook.Key.Q, handler);

// or

SuperKeyHook.Register(
    ctrl: true,
    keyString: SuperKeyHook.Key.Q,
    keyDownHandler: delegate ()
                    {
                        Console.WriteLine("Ctrl+Q down");
                    },
    keyUpHandler: delegate ()
                  {
                      Console.WriteLine("Ctrl+Q up");
                  }
);
```

`OnKeyDown` Event will triggered only one time. In other words, `OnKeyDown` is triggered at the top edge. Similarly, `OnKeyUp` is triggered at the bottom edge.

**WARNING!!!**

Because `SuperKeyboard` can generate native input of the PS/2 keyboard. So `SuperKeyboard.KeyPress` **WILL TRIGGER** `SuperKeyHook`'s hook! This may case **UNEXPECT recursive call**!

```C#
SuperKeyHook.Register(
    keyString: SuperKeyHook.Key.Q,
    keyDownHandler: delegate ()
    {
        Console.WriteLine("You press the hotkey");
        // want to input "quit" and return
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_Q); // WILL CAUSE ENDLESS RECURSIVE CALL!!!
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_U);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_I);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_T);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_RETURN);
    },
    keyUpHandler: delegate () { }
);
```

It can be avoided by using combination key as hotkey. Try to use hotkeys like **Ctrl+Q** instead of a simple **Q**.

Also, you can create a *Hotkey Lock*. When hotkey event is running, prevent handling this hotkey again. Try this code:

```C#
private bool hotkeyLock_Q = false;

SuperKeyHook.Register(
    keyString: SuperKeyHook.Key.Q,
    keyDownHandler: delegate ()
    {
        // hotkey lock
        lock (hotkeyLock_Q)
        {
            if (hotkeyLock_Q)
            {
                return;
            }
            hotkeyLock_Q = true;
        }

        Console.WriteLine("You press the hotkey");
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_Q);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_U);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_I);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_T);
        SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_RETURN);

        hotkeyLock_Q = false;
    },
    keyUpHandler: delegate () { }
);
```

It will be a native function in the future.

If you want to deal with all inputs that user triggered, you can use `AddGlobalKeyHandler(GlobalKeyHandler)` and `RemoveGlobalKeyHandler(int)`.

```C#
private int handlerId;

// ......
handlerId = SuperKeyHook.AddGlobalKeyHandler(
    delegate (string keyString, bool isKeyDown, bool isKeyUp)
    {
        Console.WriteLine("GlobalKeyHandler: " + keyString + "," + (isKeyDown ? "KeyDown," : "") + (isKeyUp ? "KeyUp" : ""));
        return true;
    }
);
// ......

// in app close event
SuperKeyHook.RemoveGlobalKeyHandler(handlerId);
```

There is a `return true;` at the end of the delegate. It means `SuperKeyHook` can continue to handle this input event. So this input event may trigger a hotkey.

If you returns `false` in the delegate, it will prevent `SuperKeyHook` from handling this input event. Here is the example.

```C#
SuperKeyHook.Register(
    keyString: SuperKeyHook.Key.Q,
    keyDownHandler: delegate ()
    {
        Console.WriteLine("hotkey triggered");
    },
    keyUpHandler: delegate () { }
);

SuperKeyHook.AddGlobalKeyHandler(
    delegate (string keyString, bool isKeyDown, bool isKeyUp)
    {
        if (keyString == SuperKeyHook.Key.Q)
        {
            return false;
        }
        return true;
    }
);
```

In this example, when user press **Q**, nothing will happen.
