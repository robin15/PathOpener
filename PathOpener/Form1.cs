using System;
using System.Windows.Forms;

namespace PathOpener
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
            this.notifyIcon.Icon = Properties.Resources.notify_icon_disabled;
            this.notifyIcon.ContextMenuStrip = this.contextMenu_disable;
            this.notifyIcon.Text = "PathOpener - disabled";
            keyboard.StopKeyboardHook();
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon.Icon = Properties.Resources.notify_icon_enabled;
            this.notifyIcon.ContextMenuStrip = this.contextMenu_enable;
            this.notifyIcon.Text = "PathOpener - enabled";
            keyboard.StartKeyboardHook();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
            Application.Exit();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
            Application.Exit();
        }

        private void notifyIcon_enable_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon.Icon = Properties.Resources.notify_icon_disabled;
            this.notifyIcon.ContextMenuStrip = this.contextMenu_disable;
            this.notifyIcon.Text = "PathOpener - disabled";
            keyboard.StopKeyboardHook();
        }

        private void notifyIcon_disable_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon.Icon = Properties.Resources.notify_icon_enabled;
            this.notifyIcon.ContextMenuStrip = this.contextMenu_enable;
            this.notifyIcon.Text = "PathOpener - enabled";
            keyboard.StartKeyboardHook();
        }
    }
}
