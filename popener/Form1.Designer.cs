namespace popener
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon_enable = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu_enable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.disableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon_disable = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu_disable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.enableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenu_enable.SuspendLayout();
            this.contextMenu_disable.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon_enable
            // 
            this.notifyIcon_enable.ContextMenuStrip = this.contextMenu_enable;
            this.notifyIcon_enable.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_enable.Icon")));
            this.notifyIcon_enable.Text = "POpener - enabled";
            this.notifyIcon_enable.Visible = true;
            this.notifyIcon_enable.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_enable_MouseDoubleClick);
            // 
            // contextMenu_enable
            // 
            this.contextMenu_enable.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_enable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disableToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenu_enable.Name = "contextMenu_enable";
            this.contextMenu_enable.Size = new System.Drawing.Size(129, 52);
            // 
            // disableToolStripMenuItem
            // 
            this.disableToolStripMenuItem.Name = "disableToolStripMenuItem";
            this.disableToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.disableToolStripMenuItem.Text = "Disable";
            this.disableToolStripMenuItem.Click += new System.EventHandler(this.disableToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // notifyIcon_disable
            // 
            this.notifyIcon_disable.ContextMenuStrip = this.contextMenu_disable;
            this.notifyIcon_disable.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_disable.Icon")));
            this.notifyIcon_disable.Text = "POpener - disabled";
            this.notifyIcon_disable.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_disable_MouseDoubleClick);
            // 
            // contextMenu_disable
            // 
            this.contextMenu_disable.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_disable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableToolStripMenuItem,
            this.exitToolStripMenuItem1});
            this.contextMenu_disable.Name = "contextMenu_disable";
            this.contextMenu_disable.Size = new System.Drawing.Size(124, 52);
            // 
            // enableToolStripMenuItem
            // 
            this.enableToolStripMenuItem.Name = "enableToolStripMenuItem";
            this.enableToolStripMenuItem.Size = new System.Drawing.Size(123, 24);
            this.enableToolStripMenuItem.Text = "Enable";
            this.enableToolStripMenuItem.Click += new System.EventHandler(this.enableToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(123, 24);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 0);
            this.Enabled = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "PathOpener";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.contextMenu_enable.ResumeLayout(false);
            this.contextMenu_disable.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon_enable;
        private System.Windows.Forms.ContextMenuStrip contextMenu_enable;
        private System.Windows.Forms.ToolStripMenuItem disableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon_disable;
        private System.Windows.Forms.ContextMenuStrip contextMenu_disable;
        private System.Windows.Forms.ToolStripMenuItem enableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

