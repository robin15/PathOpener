using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace popener
{
    public partial class Form1 : Form
    {
        KeyboardHook keyboard;
        public Form1()
        {
            InitializeComponent();
            //this.Hide();
            //this.WindowState = FormWindowState.Minimized;
            //Control.CheckForIllegalCrossThreadCalls = false;
            keyboard = new KeyboardHook(this);
            keyboard.StartKeyboardHook();
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon_enable.Visible = false;
            this.notifyIcon_disable.Visible = true;
            keyboard.StopKeyboardHook();
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon_disable.Visible = false;
            this.notifyIcon_enable.Visible = true;
            keyboard.StartKeyboardHook();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon_enable.Visible = false;
            this.notifyIcon_disable.Visible = false;
            Application.Exit();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.notifyIcon_enable.Visible = false;
            this.notifyIcon_disable.Visible = false;
            Application.Exit();
        }
    }

    interface IKeyState
    {
        IKeyState CtrlPressed(KeyboardHook kbh);
        IKeyState CtrlReleased(KeyboardHook kbh);
        IKeyState CPressed(KeyboardHook kbh);
        IKeyState CReleased(KeyboardHook kbh);
        IKeyState AnyPressed(KeyboardHook kbh);
    }

    class Normal : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) {
            Console.WriteLine("Ctrl pressed  -> CtrlLocked");
            return new CtrlLocked(); }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return this; }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class CtrlLocked : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) {
            Console.WriteLine("Ctrl released -> Normal");
            return new Normal(); }
        public IKeyState CPressed(KeyboardHook kbh) {
            Console.WriteLine("C    pressed  -> Copied");
            return new Copied(); }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class Copied : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) {
            Console.WriteLine("Ctrl released -> Normal");
            return new Normal(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) {
            kbh.StartTimeoutTimer();
            Console.WriteLine("C   released  -> Ready to open");
            return new ReadyToOpen();
        }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class ReadyToOpen : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) {
            kbh.StopTimeoutTimer();
            Console.WriteLine("Ctrl released -> Normal");
            return new Normal();
        }
        public IKeyState CPressed(KeyboardHook kbh)
        {
            kbh.StopTimeoutTimer();
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                if (data.GetDataPresent(typeof(string)))
                {
                    string textData = (string)data.GetData(DataFormats.Text);
                    if (Regex.IsMatch(textData, @"^\\\\"))
                    {
                        //CMDウインドウ非表示　但し、エクスプローラ最前面化されない
                        //Process p = new Process();
                        //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //p.StartInfo.FileName = "cmd.exe";
                        //p.StartInfo.Arguments = "/C explorer.exe " + textData;
                        //kbh.form.Activate();
                        //p.Start();

                        string command = "/C explorer.exe " + textData;
                        Process.Start("cmd.exe", command);
                        Console.WriteLine("C    pressed  -> PathOpened");
                        return new PathOpened();
                    }
                    else
                    {
                        Console.WriteLine("Invalid Path");
                        //kbh._icon.BalloonTipTitle = "Invalid Path";
                        //kbh._icon.BalloonTipText = (textData == "" ? "clipboard is empty" : textData);
                        //kbh._icon.BalloonTipIcon = ToolTipIcon.Info;
                        //kbh._icon.ShowBalloonTip(500);
                        Console.WriteLine("C    pressed  -> IllegalPath Opened");
                        return new IllegalPathOpened();

                    }
                }
            }
            return new PathOpened();
        }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) {
            kbh.StopTimeoutTimer();
            return new CtrlLocked();
        }
    }

    class PathOpened : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) {
            Console.WriteLine("Ctrl released -> Normal");
            return new Normal(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) {
            Console.WriteLine("C    released -> CtrlLocked");
            return new CtrlLocked();
        }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class IllegalPathOpened : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh)
        {
            Console.WriteLine("Ctrl released -> Normal");
            return new Normal();
        }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh)
        {
            kbh.PopupToolTip("invalid path");
            Console.WriteLine("C    released -> CtrlLocked");
            return new CtrlLocked();
        }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }

    }

    public class KeyboardHook
    {
        private IntPtr hook;
        private IntPtr hMod;
        private IKeyState keyState;
        private HookHandler hookDelegate;
        private System.Timers.Timer timeoutTimer;

        public Form form;
        public delegate int HookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void PopupToolTipHandler(string msg);
        public delegate void RemoveToolTipHandler();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int hookType, HookHandler hookDelegate, IntPtr module, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hook, int nCode, IntPtr wParam, IntPtr lParam);

        #region Win32 Constants
        protected const int WH_KEYBOARD_LL = 0x000D;
        protected const int WM_KEYDOWN = 0x0100;
        protected const int WM_KEYUP = 0x0101;
        protected const int VK_LCONTROL = 0x00A2;
        protected const int VK_RCONTROL = 0x00A3;
        protected const int VK_C = 0x0043;
        #endregion

        #region Win32API Structures
        [StructLayout(LayoutKind.Sequential)]
        protected class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        protected enum KBDLLHOOKSTRUCTFlags : uint
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_SCANCODE = 0x0008,
            KEYEVENTF_UNICODE = 0x0004,
        }
        #endregion

        public KeyboardHook(Form f)
        {
            form = f;
            keyState = new Normal();
            hookDelegate = new HookHandler(OnHook);
            hMod = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
        }

        public void StartKeyboardHook()
        {
            hook = SetWindowsHookEx(WH_KEYBOARD_LL, hookDelegate, hMod, 0);
            if (hook == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
            timeoutTimer = new System.Timers.Timer(1500);
            timeoutTimer.Elapsed += (sender, e) =>
            {
                timeoutTimer.Enabled = false;
                form.Invoke(new PopupToolTipHandler(PopupToolTip), new object[] { "Timeout" });
            };
            timeoutTimer.Start();
            timeoutTimer.Enabled = false;
        }

        public void StopKeyboardHook()
        {
            UnhookWindowsHookEx(hook);
            timeoutTimer.Enabled = false;
            timeoutTimer.Stop();
            timeoutTimer.Dispose();
        }

        public void StartTimeoutTimer()
        {
            timeoutTimer.Enabled = true;
        }

        public void StopTimeoutTimer()
        {
            timeoutTimer.Enabled = false;
        }

        public void PopupToolTip(string msg)
        {
            Point p = new Point(Control.MousePosition.X + 38, Control.MousePosition.Y + 15);
            Help.ShowPopup(form, msg, p);
            RemovePopupAfter(2500);
        }

        private void RemovePopupAfter(double interval)
        {
            System.Timers.Timer removeTimer1;
            removeTimer1 = new System.Timers.Timer(interval);
            removeTimer1.Elapsed += (sender, e) =>
            {
                removeTimer1.Enabled = false;
                Console.WriteLine("remove popup");
                form.Invoke(new RemoveToolTipHandler(ActivateForm), new object[] { });
            };
            removeTimer1.Start();
        }

        private void ActivateForm()
        {
            form.Activate();
        }

        private int OnHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            var vkCode = (int)kb.vkCode;
            switch (vkCode)
            {
                case VK_LCONTROL:
                case VK_RCONTROL:
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        keyState = keyState.CtrlPressed(this);
                    }
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        keyState = keyState.CtrlReleased(this);
                    }
                    break;
                case VK_C:
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        keyState = keyState.CPressed(this);
                    }
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        keyState = keyState.CReleased(this);
                    }
                    break;
                default:
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        keyState = keyState.AnyPressed(this);
                    }
                    break;
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }
    }
}
