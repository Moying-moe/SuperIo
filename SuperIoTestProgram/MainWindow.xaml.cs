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

                bool flagKb = SuperKeyboard.IsInitialized;
                

                DebugLog("SuperKeyboard initialize: " + flagKb);
                if (flagKb)
                {
                    DebugLog("SuperKeyboard Test start in 3 seconds.");
                    Thread.Sleep(3000);

                    DebugLog("- input A");
                    SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_A);
                    Thread.Sleep(1000);

                    DebugLog("- input B");
                    SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_B);
                    Thread.Sleep(1000);

                    DebugLog("- input Shift+M");
                    SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_M, SuperKeyboard.ModKey.SHIFT);
                    // or
                    //SuperKeyboard.KeyCombSeq(25, SuperKeyboard.Key.VK_SHIFT, SuperKeyboard.Key.VK_M);
                    Thread.Sleep(1000);

                    DebugLog("- input C");
                    SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_C);
                    Thread.Sleep(1000);

                    DebugLog("- backspace");
                    SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_BACK);
                    Thread.Sleep(1000);

                    DebugLog("- press Ctrl+Alt+A");
                    SuperKeyboard.KeyPress(SuperKeyboard.Key.VK_A, SuperKeyboard.ModKey.CTRL | SuperKeyboard.ModKey.ALT);
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

                bool flagMs = SuperMouse.IsInitialized;
                DebugLog("SuperMouse initialize: " + flagMs);

                if (flagMs)
                {
                    DebugLog("SuperMouse Test start in 3 seconds");
                    Thread.Sleep(3000);

                    DebugLog("- move right");
                    SuperMouse.MoveRelative(50, 0);
                    Thread.Sleep(1000);

                    DebugLog("- move left-down");
                    SuperMouse.MoveRelative(-50, 50);
                    Thread.Sleep(1000);

                    DebugLog("- move to the center of the screen");
                    SuperMouse.MoveTo(960, 540);
                    Thread.Sleep(1000);

                    DebugLog("- scroll down");
                    SuperMouse.ScrollDown();
                    Thread.Sleep(1000);

                    DebugLog("- double click");
                    SuperMouse.LButtonClick(2);
                    Thread.Sleep(1000);

                    DebugLog("- right click.");
                    SuperMouse.RButtonClick();
                }
            });
        }

        private void BtnScreen_Click(object sender, RoutedEventArgs e)
        {
            DebugLog();

            bool flagSc = SuperScreen.IsInitialized;
            DebugLog("SuperScreen initialize: " + flagSc); 

            if (flagSc)
            {
                Color color = SuperScreen.GetPixelColor(960, 540);
                DebugLog("Color at (960,540) is: " + color.R + "," + color.G + "," + color.B);
                Color black = Color.FromArgb(0, 0, 0);
                DebugLog("Difference between this color * (0,0,0) is: " + SuperScreen.ColorDifference(color, black));

                System.Drawing.Point pos = SuperScreen.SearchColor(black, SuperScreen.SearchDirection.FromLeftTop);
                DebugLog("Find color * (0,0,0) in Position (" + pos.X + "," + pos.Y + ")");
                if (SuperMouse.IsInitialized)
                {
                    SuperMouse.MoveTo(pos.X, pos.Y);
                }
            }
        }

        private void BtnKeyHook_Click(object sender, RoutedEventArgs e)
        {
            DebugLog();

            bool flagKh = SuperKeyHook.IsInitialized;
            DebugLog("SuperKeyHook initialize: " + flagKh);

            if (flagKh)
            {
                SuperKeyHook.Register(
                    ctrl: true,
                    keyString: SuperKeyHook.Key.Q,
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

                SuperKeyHook.AddGlobalKeyHandler(
                    delegate (string keyString, bool isKeyDown, bool isKeyUp)
                    {
                        DebugLog("GlobalKeyHandler: " + keyString + "," + (isKeyDown ? "KeyDown," : "") + (isKeyUp ? "KeyUp" : ""));
                        return true;
                    }
                );

            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            SuperIo.SuperIo.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SuperIo.SuperIo.Initialize();
        }
    }
}
