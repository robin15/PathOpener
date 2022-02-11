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
            keyboard = new KeyboardHook(this, notifyIcon_enable);
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon_enable.Visible = false;
            this.notifyIcon_disable.Visible = true;
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon_disable.Visible = false;
            this.notifyIcon_enable.Visible = true;
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
    }

    class Normal : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return new CtrlLocked(); }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return this; }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
    }

    class CtrlLocked : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Normal(); }
        public IKeyState CPressed(KeyboardHook kbh) { return new Copied(); }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
    }
    
    class Copied : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Normal(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) {
            kbh.timer.Enabled = true;
            return new ReadyToOpen();
        }
    }

    class ReadyToOpen : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) {
            kbh.timer.Enabled = false;
            return new Normal();
        }
        public IKeyState CPressed(KeyboardHook kbh)
        {
            kbh.timer.Enabled = false;
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                if (data.GetDataPresent(typeof(string)))
                {
                    string textData = (string)data.GetData(DataFormats.Text);
                    if (Regex.IsMatch(textData, @"^\\\\"))
                    {
                        string command = "/C explorer.exe " + textData;
                        Process.Start("cmd.exe", command);
                    }
                    else
                    {
                        Console.WriteLine("Invalid Path");
                        Point p = new Point(Control.MousePosition.X + 38, Control.MousePosition.Y + 15);
                        //kbh._form.Activate();
                        //Help.ShowPopup(kbh._form, "Invali Path", p);
                        kbh._icon.BalloonTipTitle = "Invalid Path";
                        kbh._icon.BalloonTipText = (textData == "" ? "clipboard is empty" : textData);
                        kbh._icon.BalloonTipIcon = ToolTipIcon.Info;
                        kbh._icon.ShowBalloonTip(500);
                    }
                }
            }
            return new PathOpened();
        }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
    }

    class PathOpened : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Normal(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) { return new CtrlLocked(); }
    }

    public class KeyboardHook
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;
        private const int MOUSEEVENTF_LEFTUP = 0x4;

        private HookHandler hookDelegate;
        private IntPtr hook;
        private IKeyState _keyState = new Normal();
        private IntPtr hMod;

        public System.Timers.Timer timer;
        public Form _form;
        public NotifyIcon _icon;
        public ToolTip ToolTip1 = new ToolTip();

        public delegate int HookHandler(int nCode, IntPtr wParam, IntPtr lParam);

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
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_SCANCODE = 0x0008,
            KEYEVENTF_UNICODE = 0x0004,
        }
        #endregion

        public KeyboardHook(Form form, NotifyIcon icon)
        {
            hookDelegate = new HookHandler(OnHook);
            hMod = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            hook = SetWindowsHookEx(WH_KEYBOARD_LL, hookDelegate, hMod, 0);
            _form = form;
            _icon = icon;
            ToolTip1.ShowAlways = true;
            ToolTip1.IsBalloon = true;
            if (hook == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += (sender, e) =>
            {
                timer.Enabled = false;
                Console.WriteLine("Exp");
                icon.BalloonTipTitle = "Info";
                icon.BalloonTipText = "Timeout";
                icon.BalloonTipIcon = ToolTipIcon.Info;
                icon.ShowBalloonTip(500);
                //Point p = new Point(Control.MousePosition.X + 38, Control.MousePosition.Y + 15);
                //Help.ShowPopup(form, "Timeout", p);
            };
            timer.Start();
            timer.Enabled = false;
        }

        int OnHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            var vkCode = (int)kb.vkCode;
            switch (vkCode)
            {
                case VK_LCONTROL:
                case VK_RCONTROL:
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        _keyState = _keyState.CtrlPressed(this);
                    }
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        _keyState = _keyState.CtrlReleased(this);
                    }
                    break;
                case VK_C:
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        _keyState = _keyState.CPressed(this);
                    }
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        _keyState = _keyState.CReleased(this);
                    }
                    break;
                default:
                    break;
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }
    }
}
