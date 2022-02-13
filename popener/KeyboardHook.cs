using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace popener
{
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

        public KeyboardHook(Form frm)
        {
            form = frm;
            keyState = new Neutral();
            hookDelegate = new HookHandler(OnHook);
            hMod = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            PopupToolTip("PathOpener start");
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
            Point p = new Point(Control.MousePosition.X + 38, Control.MousePosition.Y + 20);
            Help.ShowPopup(form, msg, p);
            RemovePopupAfter(2500);
        }

        private void RemovePopupAfter(double interval)
        {
            System.Timers.Timer removeTimer = new System.Timers.Timer(interval);
            removeTimer.Elapsed += (sender, e) =>
            {
                removeTimer.Enabled = false;
                form.Invoke(new RemoveToolTipHandler(ActivateForm), new object[] { });
            };
            removeTimer.Start();
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
