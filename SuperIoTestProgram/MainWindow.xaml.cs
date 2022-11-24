using SuperIo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperIoTestProgram
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DebugLogMainThread(string s) {
            LabelLog.Text += s + "\n";
        }
        private void DebugLogMainThread()
        {
            DebugLogMainThread("");
        }

        private void DebugLog(string s)
        {
            Action<string> updateAction = new Action<string>(DebugLogMainThread);
            Dispatcher.BeginInvoke(updateAction, s);
        }
        private void DebugLog()
        {
            Action updateAction = new Action(DebugLogMainThread);
            Dispatcher.BeginInvoke(updateAction);
        }

        private void BtnKeyboard_Click(object sender, RoutedEventArgs e)
        {
            TbTest.Focus();
            Task.Run(() =>
            {
                DebugLog();

                bool flagKb = SuperKeyboard.Instance.IsInitialized;
                

                DebugLog("SuperKeyboard initialize: " + flagKb);
                if (flagKb)
                {
                    DebugLog("SuperKeyboard Test start in 3 seconds.");
                    Thread.Sleep(3000);

                    DebugLog("- input A");
                    SuperKeyboard.Instance.KeyPress(SuperIo.SuperIo.Key.VK_A);
                    Thread.Sleep(1000);

                    DebugLog("- input B");
                    SuperKeyboard.Instance.KeyPress(SuperIo.SuperIo.Key.VK_B);
                    Thread.Sleep(1000);

                    DebugLog("- input Shift+M");
                    SuperKeyboard.Instance.KeyPress(SuperIo.SuperIo.Key.VK_M, SuperIo.SuperIo.ModKey.SHIFT);
                    // or
                    //SuperKeyboard.KeyCombSeq(25, SuperKeyboard.Key.VK_SHIFT, SuperKeyboard.Key.VK_M);
                    Thread.Sleep(1000);

                    DebugLog("- input C");
                    SuperKeyboard.Instance.KeyPress(SuperIo.SuperIo.Key.VK_C);
                    Thread.Sleep(1000);

                    DebugLog("- backspace");
                    SuperKeyboard.Instance.KeyPress(SuperIo.SuperIo.Key.VK_BACK);
                    Thread.Sleep(1000);

                    DebugLog("- press Ctrl+Alt+A");
                    SuperKeyboard.Instance.KeyPress(SuperIo.SuperIo.Key.VK_A, SuperIo.SuperIo.ModKey.CTRL | SuperIo.SuperIo.ModKey.ALT);
                    Thread.Sleep(1000);

                    DebugLog("SuperKeyboard Test done.");
                }
            });
        }

        private void BtnMouse_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                DebugLog();

                bool flagMs = SuperMouse.Instance.IsInitialized;
                DebugLog("SuperMouse initialize: " + flagMs);

                if (flagMs)
                {
                    DebugLog("SuperMouse Test start in 3 seconds");
                    Thread.Sleep(3000);

                    DebugLog("- move right");
                    SuperMouse.Instance.MoveRelative(50, 0);
                    Thread.Sleep(1000);

                    DebugLog("- move left-down");
                    SuperMouse.Instance.MoveRelative(-50, 50);
                    Thread.Sleep(1000);

                    DebugLog("- move to the center of the screen");
                    SuperMouse.Instance.MoveTo(960, 540);
                    Thread.Sleep(1000);

                    DebugLog("- scroll down");
                    SuperMouse.Instance.ScrollDown();
                    Thread.Sleep(1000);

                    DebugLog("- double click");
                    SuperMouse.Instance.LButtonClick(2);
                    Thread.Sleep(1000);

                    DebugLog("- right click.");
                    SuperMouse.Instance.RButtonClick();
                }
            });
        }

        private void BtnScreen_Click(object sender, RoutedEventArgs e)
        {
            DebugLog();

            bool flagSc = SuperScreen.Instance.IsInitialized;
            DebugLog("SuperScreen initialize: " + flagSc); 

            if (flagSc)
            {
                Color color = SuperScreen.Instance.GetPixelColor(960, 540);
                DebugLog("Color at (960,540) is: " + color.R + "," + color.G + "," + color.B);
                Color black = Color.FromArgb(0, 0, 0);
                DebugLog("Difference between this color * (0,0,0) is: " + SuperScreen.Instance.ColorDifference(color, black));

                System.Drawing.Point pos = SuperScreen.Instance.SearchColor(black, SuperScreen.SearchDirection.FromLeftTop);
                DebugLog("Find color * (0,0,0) in Position (" + pos.X + "," + pos.Y + ")");
                if (SuperMouse.Instance.IsInitialized)
                {
                    SuperMouse.Instance.MoveTo(pos.X, pos.Y);
                }
            }
        }

        private void BtnEventKey_Click(object sender, RoutedEventArgs e)
        {
            DebugLog();

            bool flagEvt = SuperEvent.Instance.IsInitialized;
            DebugLog("SuperEvent initialize: " + flagEvt);

            if (flagEvt)
            {
                SuperEvent.Instance.RegisterKey(
                    ctrl: true,
                    key: SuperIo.SuperIo.Key.VK_Q,
                    keyDownHandler: delegate ()
                    {
                        DebugLog("down: Ctrl+Q");
                    },
                    keyUpHandler: delegate ()
                    {
                        DebugLog("up: Ctrl+Q");
                    }
                );
                DebugLog("Hooked on \"Ctrl+Q\".");

                SuperEvent.Instance.AddGlobalKeyHandler(
                    delegate (byte key, bool isKeyDown, bool isKeyUp)
                    {
                        DebugLog("GlobalKeyHandler: " + key + "," + (isKeyDown ? "KeyDown," : "") + (isKeyUp ? "KeyUp" : ""));
                        return true;
                    }
                );

            }
        }

        private void BtnEventMouse_Click(object sender, RoutedEventArgs e)
        {
            DebugLog();

            bool flagEvt = SuperEvent.Instance.IsInitialized;
            DebugLog("SuperEvent initialize: " + flagEvt);

            if (flagEvt)
            {
                SuperEvent.Instance.RegisterMouse(
                    mouseEvent: SuperIo.SuperIo.Mouse.XBUTTON1DOWN,
                    handler: delegate ()
                    {
                        DebugLog("XB1 down");
                    }
                );
                DebugLog("Hooked on XB1_down");

                SuperEvent.Instance.AddGlobalMouseHandler(
                    delegate (byte mouseEvent)
                    {
                        if (mouseEvent != SuperIo.SuperIo.Mouse.MOUSEMOVE)
                        {
                            SuperEvent.POINT position = SuperEvent.Instance.MouseLastPosition;
                            DebugLog("mouse event code: " + mouseEvent + " at (" + position.x + "," + position.y + ")");
                        }
                        return true;
                    }
                );
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
