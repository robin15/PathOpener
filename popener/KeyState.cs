using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace popener
{
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
        public IKeyState CtrlPressed(KeyboardHook kbh)
        {
            Console.WriteLine("Ctrl pressed  -> CtrlLocked");
            return new CtrlLocked();
        }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return this; }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class CtrlLocked : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh)
        {
            Console.WriteLine("Ctrl released -> Normal");
            return new Normal();
        }
        public IKeyState CPressed(KeyboardHook kbh)
        {
            Console.WriteLine("C    pressed  -> Copied");
            return new Copied();
        }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class Copied : IKeyState
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
            kbh.StartTimeoutTimer();
            Console.WriteLine("C   released  -> Ready to open");
            return new ReadyToOpen();
        }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class ReadyToOpen : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh)
        {
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
                    if (Regex.IsMatch(textData.Replace("\"", ""), @"^\\\\"))
                    {
                        /* CMDウインドウ非表示　但し、エクスプローラ最前面化されない
                        Process p = new Process();
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.Arguments = "/C explorer.exe " + textData;
                        kbh.form.Activate();
                        p.Start();
                        */
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
        public IKeyState AnyPressed(KeyboardHook kbh)
        {
            kbh.StopTimeoutTimer();
            Console.WriteLine("Any  pressed  -> CtrlLocked");
            return new CtrlLocked();
        }
    }

    class PathOpened : IKeyState
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
}
