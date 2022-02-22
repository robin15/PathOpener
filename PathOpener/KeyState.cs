using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PathOpener
{
    interface IKeyState
    {
        IKeyState CtrlPressed(KeyboardHook kbh);
        IKeyState CtrlReleased(KeyboardHook kbh);
        IKeyState CPressed(KeyboardHook kbh);
        IKeyState CReleased(KeyboardHook kbh);
        IKeyState AnyPressed(KeyboardHook kbh);
    }

    class Neutral : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return new CtrlLocked(); }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return this; }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class CtrlLocked : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Neutral(); }
        public IKeyState CPressed(KeyboardHook kbh) { return new Copied(); }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class Copied : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Neutral(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh)
        {
            kbh.StartTimeoutTimer();
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
            return new Neutral();
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
                    if (Regex.IsMatch(textData.Replace("\"", ""), @"^\\\\") || Regex.IsMatch(textData.Replace("\"", ""), "^file:", RegexOptions.IgnoreCase))
                    {
                        string command = "/C explorer.exe " + textData;
                        Process.Start("cmd.exe", command);
                        return new PathOpened();
                    }
                    else
                    {
                        return new InvalidPath();
                    }
                }
            }
            return new PathOpened();
        }
        public IKeyState CReleased(KeyboardHook kbh) { return this; }
        public IKeyState AnyPressed(KeyboardHook kbh)
        {
            kbh.StopTimeoutTimer();
            return new CtrlLocked();
        }
    }

    class PathOpened : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Neutral(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh) { return new CtrlLocked(); }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }

    class InvalidPath : IKeyState
    {
        public IKeyState CtrlPressed(KeyboardHook kbh) { return this; }
        public IKeyState CtrlReleased(KeyboardHook kbh) { return new Neutral(); }
        public IKeyState CPressed(KeyboardHook kbh) { return this; }
        public IKeyState CReleased(KeyboardHook kbh)
        {
            kbh.PopupToolTip("invalid path");
            return new CtrlLocked();
        }
        public IKeyState AnyPressed(KeyboardHook kbh) { return this; }
    }
}
