using System;
using System.Windows.Forms;

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

        private void notifyIcon_enable_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon_enable.Visible = false;
            this.notifyIcon_disable.Visible = true;
            keyboard.StopKeyboardHook();
        }

        private void notifyIcon_disable_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon_disable.Visible = false;
            this.notifyIcon_enable.Visible = true;
            keyboard.StartKeyboardHook();
        }
    }
}
