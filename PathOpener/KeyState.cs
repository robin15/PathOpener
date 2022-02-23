using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PathOpener
{
    interface IKeyState
    {
        IKeyState CtrlPressed(KeyHook kh);
        IKeyState CtrlReleased(KeyHook kh);
        IKeyState CPressed(KeyHook kh);
        IKeyState CReleased(KeyHook kh);
        IKeyState AnyPressed(KeyHook kh);
    }

    class Neutral : IKeyState
    {
        public IKeyState CtrlPressed(KeyHook kh) { return new CtrlLocked(); }
        public IKeyState CtrlReleased(KeyHook kh) { return this; }
        public IKeyState CPressed(KeyHook kh) { return this; }
        public IKeyState CReleased(KeyHook kh) { return this; }
        public IKeyState AnyPressed(KeyHook kh) { return this; }
    }

    class CtrlLocked : IKeyState
    {
        public IKeyState CtrlPressed(KeyHook kh) { return this; }
        public IKeyState CtrlReleased(KeyHook kh) { return new Neutral(); }
        public IKeyState CPressed(KeyHook kh) { return new Copied(); }
        public IKeyState CReleased(KeyHook kh) { return this; }
        public IKeyState AnyPressed(KeyHook kh) { return this; }
    }

    class Copied : IKeyState
    {
        public IKeyState CtrlPressed(KeyHook kh) { return this; }
        public IKeyState CtrlReleased(KeyHook kh) { return new Neutral(); }
        public IKeyState CPressed(KeyHook kh) { return this; }
        public IKeyState CReleased(KeyHook kh)
        {
            kh.StartTimeoutTimer();
            return new ReadyToOpen();
        }
        public IKeyState AnyPressed(KeyHook kh) { return this; }
    }

    class ReadyToOpen : IKeyState
    {
        public IKeyState CtrlPressed(KeyHook kh) { return this; }
        public IKeyState CtrlReleased(KeyHook kh)
        {
            kh.StopTimeoutTimer();
            return new Neutral();
        }
        public IKeyState CPressed(KeyHook kh)
        {
            kh.StopTimeoutTimer();
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
        public IKeyState CReleased(KeyHook kh) { return this; }
        public IKeyState AnyPressed(KeyHook kh)
        {
            kh.StopTimeoutTimer();
            return new CtrlLocked();
        }
    }

    class PathOpened : IKeyState
    {
        public IKeyState CtrlPressed(KeyHook kh) { return this; }
        public IKeyState CtrlReleased(KeyHook kh) { return new Neutral(); }
        public IKeyState CPressed(KeyHook kh) { return this; }
        public IKeyState CReleased(KeyHook kh) { return new CtrlLocked(); }
        public IKeyState AnyPressed(KeyHook kh) { return this; }
    }

    class InvalidPath : IKeyState
    {
        public IKeyState CtrlPressed(KeyHook kh) { return this; }
        public IKeyState CtrlReleased(KeyHook kh) { return new Neutral(); }
        public IKeyState CPressed(KeyHook kh) { return this; }
        public IKeyState CReleased(KeyHook kh)
        {
            kh.PopupToolTip("invalid path");
            return new CtrlLocked();
        }
        public IKeyState AnyPressed(KeyHook kh) { return this; }
    }
}
