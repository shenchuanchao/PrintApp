namespace PrintApp
{
    partial class PrintingApp
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenu;
        private System.Windows.Forms.ToolStripMenuItem 服务设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 服务启动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 连接状态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 检查更新ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 重启ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem2;
        private ContextMenuStrip dgvContextMenu;


        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintingApp));
            menuStrip1 = new MenuStrip();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            检查更新ToolStripMenuItem = new ToolStripMenuItem();
            重启ToolStripMenuItem = new ToolStripMenuItem();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            打印机ToolStripMenuItem = new ToolStripMenuItem();
            默认打印机ToolStripMenuItem = new ToolStripMenuItem();
            清除列表ToolStripMenuItem = new ToolStripMenuItem();
            模板ToolStripMenuItem = new ToolStripMenuItem();
            设计ToolStripMenuItem = new ToolStripMenuItem();
            预览ToolStripMenuItem = new ToolStripMenuItem();
            帮助ToolStripMenuItem = new ToolStripMenuItem();
            notifyIcon1 = new NotifyIcon(components);
            notifyIconMenu = new ContextMenuStrip(components);
            服务设置ToolStripMenuItem = new ToolStripMenuItem();
            服务启动ToolStripMenuItem = new ToolStripMenuItem();
            连接状态ToolStripMenuItem = new ToolStripMenuItem();
            检查更新ToolStripMenuItem2 = new ToolStripMenuItem();
            重启ToolStripMenuItem2 = new ToolStripMenuItem();
            退出ToolStripMenuItem2 = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            menuStrip1.SuspendLayout();
            notifyIconMenu.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 打印机ToolStripMenuItem, 模板ToolStripMenuItem, 帮助ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(809, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 检查更新ToolStripMenuItem, 重启ToolStripMenuItem, 退出ToolStripMenuItem });
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 21);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // 检查更新ToolStripMenuItem
            // 
            检查更新ToolStripMenuItem.Name = "检查更新ToolStripMenuItem";
            检查更新ToolStripMenuItem.Size = new Size(124, 22);
            检查更新ToolStripMenuItem.Text = "检查更新";
            检查更新ToolStripMenuItem.Click += 检查更新ToolStripMenuItem_Click;
            // 
            // 重启ToolStripMenuItem
            // 
            重启ToolStripMenuItem.Name = "重启ToolStripMenuItem";
            重启ToolStripMenuItem.Size = new Size(124, 22);
            重启ToolStripMenuItem.Text = "重启";
            重启ToolStripMenuItem.Click += 重启ToolStripMenuItem_Click;
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(124, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // 打印机ToolStripMenuItem
            // 
            打印机ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 默认打印机ToolStripMenuItem, 清除列表ToolStripMenuItem });
            打印机ToolStripMenuItem.Name = "打印机ToolStripMenuItem";
            打印机ToolStripMenuItem.Size = new Size(56, 21);
            打印机ToolStripMenuItem.Text = "打印机";
            // 
            // 默认打印机ToolStripMenuItem
            // 
            默认打印机ToolStripMenuItem.Name = "默认打印机ToolStripMenuItem";
            默认打印机ToolStripMenuItem.Size = new Size(136, 22);
            默认打印机ToolStripMenuItem.Text = "默认打印机";
            // 
            // 清除列表ToolStripMenuItem
            // 
            清除列表ToolStripMenuItem.Name = "清除列表ToolStripMenuItem";
            清除列表ToolStripMenuItem.Size = new Size(136, 22);
            清除列表ToolStripMenuItem.Text = "清除列表";
            清除列表ToolStripMenuItem.Click += 清除列表ToolStripMenuItem_Click;
            // 
            // 模板ToolStripMenuItem
            // 
            模板ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 设计ToolStripMenuItem, 预览ToolStripMenuItem });
            模板ToolStripMenuItem.Name = "模板ToolStripMenuItem";
            模板ToolStripMenuItem.Size = new Size(44, 21);
            模板ToolStripMenuItem.Text = "模板";
            // 
            // 设计ToolStripMenuItem
            // 
            设计ToolStripMenuItem.Name = "设计ToolStripMenuItem";
            设计ToolStripMenuItem.Size = new Size(100, 22);
            设计ToolStripMenuItem.Text = "设计";
            设计ToolStripMenuItem.Click += 设计ToolStripMenuItem_Click;
            // 
            // 预览ToolStripMenuItem
            // 
            预览ToolStripMenuItem.Name = "预览ToolStripMenuItem";
            预览ToolStripMenuItem.Size = new Size(100, 22);
            预览ToolStripMenuItem.Text = "预览";
            预览ToolStripMenuItem.Click += 预览ToolStripMenuItem_Click;
            // 
            // 帮助ToolStripMenuItem
            // 
            帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            帮助ToolStripMenuItem.Size = new Size(44, 21);
            帮助ToolStripMenuItem.Text = "帮助";
            帮助ToolStripMenuItem.Click += 帮助ToolStripMenuItem_Click;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = notifyIconMenu;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "打印监听组件";
            notifyIcon1.Visible = true;
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            // 
            // notifyIconMenu
            // 
            notifyIconMenu.Items.AddRange(new ToolStripItem[] { 服务设置ToolStripMenuItem, 服务启动ToolStripMenuItem, 连接状态ToolStripMenuItem, 检查更新ToolStripMenuItem2, 重启ToolStripMenuItem2, 退出ToolStripMenuItem2 });
            notifyIconMenu.Name = "notifyIconMenu";
            notifyIconMenu.Size = new Size(125, 136);
            // 
            // 服务设置ToolStripMenuItem
            // 
            服务设置ToolStripMenuItem.Name = "服务设置ToolStripMenuItem";
            服务设置ToolStripMenuItem.Size = new Size(124, 22);
            服务设置ToolStripMenuItem.Text = "服务设置";
            服务设置ToolStripMenuItem.Click += 服务设置ToolStripMenuItem_Click;
            // 
            // 服务启动ToolStripMenuItem
            // 
            服务启动ToolStripMenuItem.Name = "服务启动ToolStripMenuItem";
            服务启动ToolStripMenuItem.Size = new Size(124, 22);
            服务启动ToolStripMenuItem.Text = "服务启动";
            服务启动ToolStripMenuItem.Click += 服务启动ToolStripMenuItem_Click;
            // 
            // 连接状态ToolStripMenuItem
            // 
            连接状态ToolStripMenuItem.Name = "连接状态ToolStripMenuItem";
            连接状态ToolStripMenuItem.Size = new Size(124, 22);
            连接状态ToolStripMenuItem.Text = "连接状态";
            // 
            // 检查更新ToolStripMenuItem2
            // 
            检查更新ToolStripMenuItem2.Name = "检查更新ToolStripMenuItem2";
            检查更新ToolStripMenuItem2.Size = new Size(124, 22);
            检查更新ToolStripMenuItem2.Text = "检查更新";
            检查更新ToolStripMenuItem2.Click += 检查更新ToolStripMenuItem2_Click;
            // 
            // 重启ToolStripMenuItem2
            // 
            重启ToolStripMenuItem2.Name = "重启ToolStripMenuItem2";
            重启ToolStripMenuItem2.Size = new Size(124, 22);
            重启ToolStripMenuItem2.Text = "重启";
            重启ToolStripMenuItem2.Click += 重启ToolStripMenuItem2_Click;
            // 
            // 退出ToolStripMenuItem2
            // 
            退出ToolStripMenuItem2.Name = "退出ToolStripMenuItem2";
            退出ToolStripMenuItem2.Size = new Size(124, 22);
            退出ToolStripMenuItem2.Text = "退出";
            退出ToolStripMenuItem2.Click += 退出ToolStripMenuItem2_Click;
            // 
            // tabControl1
            // 
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 25);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(809, 376);
            tabControl1.TabIndex = 1;
            // 
            // PrintingApp
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(809, 401);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "PrintingApp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "打印监听组件";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            notifyIconMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem 文件ToolStripMenuItem;
        private ToolStripMenuItem 检查更新ToolStripMenuItem;
        private ToolStripMenuItem 重启ToolStripMenuItem;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private ToolStripMenuItem 打印机ToolStripMenuItem;
        private ToolStripMenuItem 默认打印机ToolStripMenuItem;
        private ToolStripMenuItem 清除列表ToolStripMenuItem;
        private ToolStripMenuItem 模板ToolStripMenuItem;
        private ToolStripMenuItem 设计ToolStripMenuItem;
        private ToolStripMenuItem 预览ToolStripMenuItem;
        private ToolStripMenuItem 帮助ToolStripMenuItem;
        private TabControl tabControl1;
    }
}
