using System.Net;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using System.Text.Json;
using Fleck;
using FastReport;
using System.Data;
using System.Runtime.InteropServices;
using System.Management;
using System.ComponentModel; // 导出 PDF

namespace PrintApp
{
    public partial class PrintingApp : Form
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);
        private WebSocketServer wsServer;
        private List<IWebSocketConnection> wsClients = new List<IWebSocketConnection>();
        private bool isServiceRunning = false;

        public PrintingApp()
        {
            InitializeComponent();

            //this.Icon = Properties.Resources.favicon;
            //notifyIcon1.Icon = Properties.Resources.favicon;
            // 订阅FormClosing事件
            this.FormClosing += PrintingApp_FormClosing;
            InitializeDgvContextMenu();
        }
        /// <summary>
        /// 窗体加载事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

            // 隐藏主窗体，仅显示托盘图标
            this.ShowInTaskbar = false;
            this.Hide();

            // 绑定本地打印机列表
            BindPrintersToMenu();
            // 绑定本地打印机列表到TabControl
            BindPrintersToTabControl();

            // 程序首次运行时自动启动服务
            服务启动ToolStripMenuItem_Click(null, null);

        }
        /// <summary>
        /// 处理窗体关闭事件，隐藏窗体而不是退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintingApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭时不退出程序，只隐藏窗体
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }
        /// <summary>
        /// 初始化DataGridView的右键菜单
        /// </summary>
        private void InitializeDgvContextMenu()
        {
            dgvContextMenu = new ContextMenuStrip();
            dgvContextMenu.Items.Add("取消打印", null, CancelPrint_Click);
            dgvContextMenu.Items.Add("重新打印", null, Reprint_Click);
            dgvContextMenu.Items.Add("删除记录", null, DeleteRecord_Click);
        }


        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            // 双击托盘图标还原窗体
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }
        /// <summary>
        /// 绑定本地打印机列表到菜单
        /// </summary>
        internal void BindPrintersToMenu()
        {
            默认打印机ToolStripMenuItem.DropDownItems.Clear();
            // 获取当前系统默认打印机
            string defaultPrinter = new System.Drawing.Printing.PrinterSettings().PrinterName;

            // 先添加默认打印机（始终第一行）
            var defaultItem = new ToolStripMenuItem(defaultPrinter)
            {
                Checked = true
            };
            defaultItem.Click += (s, e) => SetDefaultPrinterUI(defaultPrinter);
            // 添加“首选项”子菜单
            var prefItem = new ToolStripMenuItem("首选项");
            prefItem.Click += (s, e) => ShowPrinterProperties(defaultPrinter);
            defaultItem.DropDownItems.Add(prefItem);
            默认打印机ToolStripMenuItem.DropDownItems.Add(defaultItem);

            // 再添加其他打印机（排除默认打印机）
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer == defaultPrinter)
                    continue;

                var item = new ToolStripMenuItem(printer)
                {
                    Checked = false
                };
                item.Click += (s, e) => SetDefaultPrinterUI(printer);

                var prefItem2 = new ToolStripMenuItem("首选项");
                prefItem2.Click += (s, e) => ShowPrinterProperties(printer);
                item.DropDownItems.Add(prefItem2);

                默认打印机ToolStripMenuItem.DropDownItems.Add(item);
            }

        }
        /// <summary>
        /// 显示打印机首选项对话框
        /// </summary>
        /// <param name="printerName"></param>
        private void ShowPrinterProperties(string printerName)
        {
            // 使用rundll32调用打印机属性对话框
            //string args = $"printui.dll,PrintUIEntry /p /n \"{printerName}\"";
            //•	/e 参数表示直接打开“首选项”对话框
            string args = $"printui.dll,PrintUIEntry /e /n \"{printerName}\"";
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法打开打印机首选项窗口：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 绑定本地打印机列表到TabControl
        /// </summary>
        private void BindPrintersToTabControl()
        {
            tabControl1.TabPages.Clear();

            string defaultPrinter = new System.Drawing.Printing.PrinterSettings().PrinterName;
            List<string> printers = new List<string>();

            // 先将默认打印机添加到列表首位
            printers.Add(defaultPrinter);

            // 再添加其他打印机（排除默认打印机）
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer != defaultPrinter)
                    printers.Add(printer);
            }

            foreach (string printer in printers)
            {
                var tabPage = new TabPage(printer);

                // 可选：添加按钮或控件到tabPage.Controls
                // 创建DataGridView
                var dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                // 添加列
                dgv.Columns.Add("clientIp", "来源");
                dgv.Columns.Add("taskId", "任务ID");
                dgv.Columns.Add("taskName", "任务名称");
                dgv.Columns.Add("realName", "模板");
                dgv.Columns.Add("requestTime", "开始时间");
                dgv.Columns.Add("status", "任务状态");

                //绑定菜单
                dgv.ContextMenuStrip = dgvContextMenu;
                dgv.MouseDown += Dgv_MouseDown;

                // 添加数据
                // 创建TextBox
                var txtSearch = new TextBox
                {
                    PlaceholderText = "任务ID",
                    Width = 120,
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                };

                // 创建Button
                var btnSearch = new Button
                {
                    Text = "查找",
                    Width = 60,
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                };

                // 查找事件
                btnSearch.Click += (s, e) =>
                {
                    string searchId = txtSearch.Text.Trim();
                    bool found = false;
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                        if (row.Cells["taskId"].Value?.ToString() == searchId)
                        {
                            row.Selected = true;
                            dgv.CurrentCell = row.Cells["taskId"];
                            found = true;
                        }
                        else
                        {
                            row.Selected = false;
                        }
                    }
                    if (!found)
                    {
                        MessageBox.Show("未找到对应任务ID！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                // 使用Panel布局
                var panel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 40
                };
                txtSearch.Location = new Point(10, 8);
                btnSearch.Location = new Point(140, 6);
                panel.Controls.Add(txtSearch);
                panel.Controls.Add(btnSearch);

                tabPage.Controls.Add(panel);
                tabPage.Controls.Add(dgv);
                tabControl1.TabPages.Add(tabPage);
            }
        }


        /// <summary>  
        /// UI和系统都设置默认打印机 
        /// </summary>  
        /// <param name="printerName"></param>  
        private void SetDefaultPrinterUI(string printerName)
        {
            foreach (ToolStripMenuItem item in 默认打印机ToolStripMenuItem.DropDownItems)
                item.Checked = item.Text == printerName;

            // 如需设置为系统默认打印机，可调用 Win32 API（可选）  
            SetSystemDefaultPrinter(printerName);
        }

        /// <summary>  
        /// 设置系统默认打印机  
        /// </summary>  
        /// <param name="printerName"></param>  
        internal void SetSystemDefaultPrinter(string printerName)
        {
            if (!SetDefaultPrinter(printerName))
            {
                MessageBox.Show("设置默认打印机失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                BindPrintersToMenu();
                MessageBox.Show($"已设置默认打印机：{printerName}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// DataGridView鼠标右键点击事件处理（确保右键时选中当前行）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dgv_MouseDown(object sender, MouseEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (e.Button == MouseButtons.Right)
            {
                var hit = dgv.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    dgv.ClearSelection();
                    dgv.Rows[hit.RowIndex].Selected = true;
                    dgv.CurrentCell = dgv.Rows[hit.RowIndex].Cells[0];
                }
            }
        }


        #region 顶部菜单事件处理
        /// <summary>
        /// 获取配置文件中的模板路径
        /// </summary>
        /// <returns></returns>
        private string GetTemplatePathFromConfig()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetConfigValue("template_path"));
        }
        /// <summary>
        /// 检查更新软件版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 检查更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 替换为你自己的 update.xml 地址
            string updateXmlUrl = GetConfigValue("updateXmlUrl");
            AutoUpdaterDotNET.AutoUpdater.Start(updateXmlUrl);
            //AutoUpdaterDotNET.AutoUpdater.ShowSkipButton = false; // 不允许跳过
            //AutoUpdaterDotNET.AutoUpdater.ShowRemindLaterButton = false; // 不允许稍后提醒
            //AutoUpdaterDotNET.AutoUpdater.RunUpdateAsAdmin = true; // 需要管理员权限
            /*
             * 在你的服务器上放置一个XML文件（如update.xml），内容示例：
               <?xml version="1.0" encoding="utf-8"?>
                <item>
                  <version>1.2.3.4</version>
                  <url>https://yourserver.com/downloads/PrintAppSetup.exe</url>
                  <changelog>https://yourserver.com/changelog.html</changelog>
                  <mandatory>false</mandatory>
                </item>
            •	version：最新版本号
            •	url：新版本安装包下载地址
            •	changelog：更新日志（可选）
            •	mandatory：是否强制更新
            */

        }
        private void 重启ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("确定要重启程序吗？", "确认重启", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 关闭托盘图标
                notifyIcon1.Visible = false;

                // 重新启动程序
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Application.Exit();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 退出程序
            var result = MessageBox.Show("确定要退出窗口吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                notifyIcon1.Visible = false;
                if (wsServer != null)
                {
                    wsServer.Dispose();
                    wsServer = null;
                }
                wsClients.Clear();

                Application.Exit();
                Environment.Exit(0); // 强制终止进程
            }
        }
        /// <summary>
        /// 清除当前选中TabPage中的DataGridView数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 清除列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 获取当前选中的TabPage
            var tab = tabControl1.SelectedTab;
            if (tab == null) return;

            // 查找TabPage中的DataGridView
            var dgv = tab.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dgv != null)
            {
                dgv.Rows.Clear();
            }
        }
        /// <summary>
        /// 设计菜单项点击事件，启动 FastReport 设计器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 设计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string designerPath = GetConfigValue("designer_path");
            string templatePath = GetTemplatePathFromConfig();
            if (string.IsNullOrEmpty(designerPath) || !System.IO.File.Exists(designerPath))
            {
                MessageBox.Show("未找到 FastReport 设计器，请检查 config.ini 配置！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!System.IO.File.Exists(templatePath))
            {
                MessageBox.Show("未找到模板文件，请检查路径！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(designerPath, $"\"{templatePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动 FastReport 设计器失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string viewerPath = GetConfigValue("viewer_path");
            string templatePath = GetTemplatePathFromConfig();
            if (string.IsNullOrEmpty(viewerPath) || !System.IO.File.Exists(viewerPath))
            {
                MessageBox.Show("未找到 FastReport 预览器，请检查 config.ini 配置！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!System.IO.File.Exists(templatePath))
            {
                MessageBox.Show("未找到模板文件，请检查路径！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(viewerPath, $"\"{templatePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动 FastReport 预览器失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 帮助菜单项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new Form())
            {
                form.Text = "帮助";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(350, 120);
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label
                {
                    Text = "请联系开发者沈先生：",
                    Location = new Point(15, 15),
                    AutoSize = true
                };
                form.Controls.Add(label);

                var txtEmail = new TextBox
                {
                    Text = "wxscc@foxmail.com",
                    Location = new Point(15, 40),
                    Width = 200,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.FixedSingle
                };
                form.Controls.Add(txtEmail);

                var btnCopy = new Button
                {
                    Text = "复制邮箱",
                    Location = new Point(220, 38),
                    Width = 80
                };
                btnCopy.Click += (s, ev) =>
                {
                    Clipboard.SetText(txtEmail.Text);
                    MessageBox.Show("邮箱地址已复制！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                form.Controls.Add(btnCopy);

                var btnMail = new Button
                {
                    Text = "发送邮件",
                    Location = new Point(15, 75),
                    Width = 80
                };
                btnMail.Click += (s, ev) =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = $"mailto:{txtEmail.Text}",
                        UseShellExecute = true
                    });
                };
                form.Controls.Add(btnMail);

                var btnClose = new Button
                {
                    Text = "关闭",
                    Location = new Point(220, 75),
                    Width = 80,
                    DialogResult = DialogResult.OK
                };
                form.Controls.Add(btnClose);

                form.AcceptButton = btnClose;
                form.ShowDialog(this);
            }
        }
        #endregion

        #region 托盘菜单事件处理
        private void 服务设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SettingForm())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this);
            }
        }

        private void 服务启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isServiceRunning)
                {
                    服务启动ToolStripMenuItem.Checked = true; // 显示对号
                    return;
                }
                // 读取 config.ini 中 socket_port
                string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CONFIG.ini");
                int port = int.Parse(GetConfigValue("socket_port"));

                if (port == 0)
                {
                    MessageBox.Show("未能获取有效端口号！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 启动 WebSocket 服务
                wsServer = new WebSocketServer($"ws://0.0.0.0:{port}");
                wsServer.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        wsClients.Add(socket);
                        UpdateConnectionStatus(true);
                    };
                    socket.OnClose = () =>
                    {
                        wsClients.Remove(socket);
                        UpdateConnectionStatus(wsClients.Count > 0);
                    };
                    socket.OnError = (ex) =>
                    {
                        wsClients.Remove(socket);
                        UpdateConnectionStatus(wsClients.Count > 0);
                    };
                    socket.OnMessage = message =>
                    {
                        var msg = message?.Trim().ToLowerInvariant();
                        // 处理不同的消息
                        if (msg == "ping")
                        {
                            // 回复 pong
                            socket.Send("pong");
                        }
                        else if (msg == "show")
                        {
                            // 显示主窗体
                            this.Invoke(() =>
                            {
                                this.Show();
                                this.WindowState = FormWindowState.Normal;
                                this.ShowInTaskbar = true;
                                this.Activate();
                            });
                        }
                        else if (msg != null && msg.TrimStart().StartsWith("{"))
                        {
                            // 反序列化为 JsonNode 便于动态访问
                            var json = JsonNode.Parse(msg);
                            var cmd = json?["cmd"]?.ToString();
                            string requestId = json?["requestid"]?.ToString();
                            //处理打印任务
                            if (cmd == "print")
                            {
                                // 取出 printIniInfo 和 data
                                var printIniInfo = json["data"]?["printiniinfo"];
                                var data = json["data"]?["data"];
                                string filePath = printIniInfo?["filepath"]?.ToString();
                                string realName = printIniInfo?["realname"]?.ToString();

                                // 获取来源IP和端口
                                string clientIp = socket.ConnectionInfo.ClientIpAddress;
                                int clientPort = socket.ConnectionInfo.ClientPort;
                                string requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                string status = "作业正在后台处理";
                                // 获取当前系统默认打印机
                                string printerName = new System.Drawing.Printing.PrinterSettings().PrinterName;
                                // 任务ID自增
                                int taskId = 0;
                                // 任务名称为当前时间
                                string taskName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                // 查找对应TabPage和DataGridView
                                this.Invoke(() =>
                                {
                                    foreach (TabPage tab in tabControl1.TabPages)
                                    {
                                        // 支持“(默认)”后缀
                                        if (tab.Text.StartsWith(printerName))
                                        {
                                            var dgv = tab.Controls.OfType<DataGridView>().FirstOrDefault();
                                            if (dgv != null)
                                            {
                                                int rowIndex = dgv.Rows.Add(
                                                     $"{clientIp}:{clientPort}", // 来源
                                                     taskId,                     // 任务ID
                                                     taskName,                   // 任务名称
                                                     realName,                   // 模板
                                                     requestTime,                // 开始时间
                                                     status                      // 任务状态
                                                 );
                                                var row = dgv.Rows[rowIndex];
                                                row.Tag = new PrintTaskInfo
                                                {
                                                    FilePath = filePath,
                                                    Data = data
                                                };
                                                // 添加后排序
                                                dgv.Sort(dgv.Columns["requestTime"], ListSortDirection.Descending);
                                            }
                                            break;
                                        }
                                    }
                                });
                                // 调用实际打印方法
                                this.Invoke(() => PrintFile(filePath, data, taskName, socket, requestId));

                                //监听打印机状态
                                StartMonitorPrintJob(printerName, taskId, taskName);
                            }
                            else
                            {
                                // 处理其他cmd
                                Console.WriteLine($"收到未知cmd: {cmd}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"收到未知消息: {message}");
                        }
                    };
                });
                isServiceRunning = true;
                服务启动ToolStripMenuItem.Checked = true; // 启动成功，显示对号
                MessageBox.Show($"服务已启动，监听端口：{port}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                isServiceRunning = false;
                服务启动ToolStripMenuItem.Checked = false; // 启动失败，去掉对号
                MessageBox.Show($"服务启动失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 打印机监听方法实现
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="taskId"></param>
        private void StartMonitorPrintJob(string printerName, int taskId, string taskName)
        {
            Task.Run(() =>
            {
                try
                {
                    string query = $"SELECT * FROM Win32_PrintJob WHERE Name LIKE '%{printerName}%'";
                    using (var searcher = new ManagementObjectSearcher(query))
                    {
                        while (true)
                        {
                            var jobs = searcher.Get();
                            bool found = false;
                            foreach (ManagementObject job in jobs)
                            {
                                found = true;
                                //string document = job["Document"]?.ToString() ?? "";
                                int JobId = Convert.ToInt32(job["JobId"]);
                                if (JobId == taskId)
                                {
                                    // 匹配到本任务，更新状态
                                    string jobStatus = job["JobStatus"]?.ToString() ?? "";
                                    string status = job["Status"]?.ToString() ?? "";
                                    string displayStatus = string.IsNullOrEmpty(jobStatus) ? status : jobStatus;
                                    UpdateTaskStatusOnUI(printerName, taskName, displayStatus);
                                    if (displayStatus.Contains("Printed") || displayStatus.Contains("Completed") || displayStatus.Contains("Deleted"))
                                        return;
                                }
                            }
                            if (!found)
                            {
                                // 作业已消失，认为已完成
                                UpdateTaskStatusOnUI(printerName, taskName, "已完成");
                                return;
                            }
                            Thread.Sleep(1000); // 1秒轮询
                        }
                    }
                }
                catch (Exception ex)
                {
                    UpdateTaskStatusOnUI(printerName, taskName, "状态监听失败");
                }
            });
        }

        // 跨线程安全更新DataGridView
        private void UpdateTaskStatusOnUI(string printerName, string taskName, string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateTaskStatusOnUI(printerName, taskName, status)));
                return;
            }
            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Text.StartsWith(printerName))
                {
                    var dgv = tab.Controls.OfType<DataGridView>().FirstOrDefault();
                    if (dgv != null)
                    {
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.Cells["taskName"].Value?.ToString() == taskName.ToString())
                            {
                                // 如果已经是“打印失败”，不再覆盖
                                var currentStatus = row.Cells["status"].Value?.ToString();
                                if (currentStatus == "打印失败" && status == "已完成")
                                    return;
                                row.Cells["status"].Value = status;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 更新连接状态菜单项的选中状态
        /// </summary>
        /// <param name="connected"></param>
        private void UpdateConnectionStatus(bool connected)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateConnectionStatus(connected)));
                return;
            }
            连接状态ToolStripMenuItem.Checked = connected;
        }
        /// <summary>
        /// 打印文件方法
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="realName"></param>
        private void PrintFile(string filePath, object data, string taskName, IWebSocketConnection socket = null, string requestId = null)
        {
            // 判断是否为网络文件
            string localFile = filePath;
            if (filePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // 下载到临时目录
                string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(filePath));
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(filePath, tempPath);
                }
                localFile = tempPath;
            }
            if (!System.IO.File.Exists(localFile))
            {
                MessageBox.Show("未找到模板文件，请检查路径！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string ext = Path.GetExtension(localFile).ToLowerInvariant();

            if (ext == ".txt")
            {
                PrintTextFile(localFile);
            }
            else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif")
            {
                PrintImageFile(localFile);
            }
            else if (ext == ".pdf" || ext == ".frx")
            {
                using (Report report = new Report())
                {
                    report.Load(localFile);
                    // 判断data是否有数据
                    bool hasData = false;
                    if (data is JsonObject jsonObj)
                        hasData = jsonObj.Count > 0;
                    else if (data is System.Data.DataTable dt)
                        hasData = dt.Rows.Count > 0;
                    else if (data is System.Data.DataSet ds)
                        hasData = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
                    else if (data is System.Collections.IEnumerable list && data.GetType().IsGenericType)
                        hasData = list.Cast<object>().Any();

                    // 只有有数据时才注册数据
                    if (hasData)
                    {
                        if (data is JsonObject jsonObj2)
                        {
                            var dt2 = JsonObjectToDataTable(jsonObj2);
                            report.RegisterData(dt2, "Data");
                            
                        }
                        else if (data is System.Data.DataTable dt2)
                            report.RegisterData(dt2, "");
                        else if (data is System.Data.DataSet ds2)
                            report.RegisterData(ds2, "");
                        else if (data is System.Collections.IEnumerable list2 && data.GetType().IsGenericType)
                            report.RegisterData(list2, "");
                        else
                        {
                            MessageBox.Show("不支持的数据类型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    // 否则不注册数据，直接打印空白模板
                    // 获取第一个页面并设置纸张
                    // 获取当前系统默认打印机
                    string printerName = new System.Drawing.Printing.PrinterSettings().PrinterName;
                    try
                    {
                        report.Prepare();
                    }
                    catch (Exception ex)
                    {
                        UpdateTaskStatusOnUI(printerName, taskName, "打印失败");
                        // 发送错误信息到客户端
                        if (socket != null && !string.IsNullOrEmpty(requestId))
                        {
                            var errorMsg = new
                            {
                                cmd = "printResult",
                                requestId = requestId,
                                status = ex.Message
                            };
                            socket.Send(JsonSerializer.Serialize(errorMsg));
                        }
                        return;
                    }

                    var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                    string pdfPath = Path.Combine(Path.GetTempPath(), taskName + ".pdf");
                    report.Export(pdfExport, pdfPath);

                    PrintByShell(pdfPath);
                    // 等待队列刷新（可适当Sleep 0.5~1秒，确保作业已进入队列）
                    System.Threading.Thread.Sleep(1000);
                    // 获取真实JobId
                    int realJobId = GetPrintJobIdWithRetry(printerName, Path.GetFileName(pdfPath));
                    // 更新DataGridView
                    if (realJobId > 0)
                    {
                        this.Invoke(() =>
                        {
                            foreach (TabPage tab in tabControl1.TabPages)
                            {
                                if (tab.Text.StartsWith(printerName))
                                {
                                    var dgv = tab.Controls.OfType<DataGridView>().FirstOrDefault();
                                    if (dgv != null)
                                    {
                                        foreach (DataGridViewRow row in dgv.Rows)
                                        {
                                            // 用 taskName 或文件名定位
                                            if (row.Cells["taskName"].Value?.ToString() == taskName)
                                            {
                                                row.Cells["taskId"].Value = realJobId;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        });
                    }
                    // 打印成功后
                    if (socket != null && !string.IsNullOrEmpty(requestId))
                    {
                        var okMsg = new
                        {
                            cmd = "printResult",
                            requestId = requestId,
                            status = "打印完成"
                        };
                        socket.Send(JsonSerializer.Serialize(okMsg));
                    }

                }
            }
            else
            {
                MessageBox.Show("不支持的文件类型: " + ext, "打印错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 打印文本文件
        /// </summary>
        /// <param name="filePath"></param>
        private void PrintTextFile(string filePath)
        {
            var printDoc = new System.Drawing.Printing.PrintDocument();
            printDoc.DocumentName = filePath;
            var reader = new System.IO.StreamReader(filePath);
            printDoc.PrintPage += (s, e) =>
            {
                float y = 0;
                int count = 0;
                float leftMargin = e.MarginBounds.Left;
                float topMargin = e.MarginBounds.Top;
                string line = null;
                Font printFont = new Font("宋体", 10);
                float linesPerPage = e.MarginBounds.Height / printFont.GetHeight(e.Graphics);

                while (count < linesPerPage && ((line = reader.ReadLine()) != null))
                {
                    y = topMargin + count * printFont.GetHeight(e.Graphics);
                    e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, y, new StringFormat());
                    count++;
                }
                e.HasMorePages = (line != null);
            };
            printDoc.EndPrint += (s, e) => { reader.Close(); };
            printDoc.Print();
        }
        /// <summary>
        /// 打印图片文件
        /// </summary>
        /// <param name="filePath"></param>
        private void PrintImageFile(string filePath)
        {
            var printDoc = new System.Drawing.Printing.PrintDocument();
            printDoc.DocumentName = filePath;
            Image img = Image.FromFile(filePath);
            printDoc.PrintPage += (s, e) =>
            {
                Rectangle m = e.MarginBounds;
                // 按比例缩放图片
                if ((double)img.Width / (double)img.Height > (double)m.Width / (double)m.Height)
                {
                    m.Height = (int)((double)img.Height / (double)img.Width * m.Width);
                }
                else
                {
                    m.Width = (int)((double)img.Width / (double)img.Height * m.Height);
                }
                e.Graphics.DrawImage(img, m);
            };
            printDoc.EndPrint += (s, e) => { img.Dispose(); };
            printDoc.Print();
        }
        /// <summary>
        /// 通过Shell打印PDF或FRX文件
        /// </summary>
        /// <param name="filePath"></param>
        private void PrintByShell(string filePath)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = filePath,
                Verb = "print",
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = true // 必须加上
            };
            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                // 打印失败
            }

        }
        /// <summary>
        /// 获取打印作业ID
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="documentName"></param>
        /// <returns></returns>
        private int GetPrintJobId(string printerName, string documentName)
        {
            string query = $"SELECT * FROM Win32_PrintJob WHERE Name LIKE '%{printerName}%'";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject job in searcher.Get())
                {
                    string doc = job["Document"]?.ToString() ?? "";
                    if (doc.Contains(documentName))
                    {
                        return Convert.ToInt32(job["JobId"]);
                    }
                }
            }
            return -1; // 未找到
        }

        private int GetPrintJobIdWithRetry(string printerName, string documentName, int maxRetry = 10, int delayMs = 300)
        {
            for (int i = 0; i < maxRetry; i++)
            {
                int jobId = GetPrintJobId(printerName, documentName);
                if (jobId > 0)
                    return jobId;
                System.Threading.Thread.Sleep(delayMs);
            }
            return -1;
        }
        private void 检查更新ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // TODO: 实现检查更新逻辑
        }

        private void 重启ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("确定要重启程序吗？", "确认重启", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 关闭托盘图标
                notifyIcon1.Visible = false;

                // 重新启动程序
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Application.Exit();
            }
        }

        private void 退出ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // 退出程序
            var result = MessageBox.Show("确定要退出窗口吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                notifyIcon1.Visible = false;
                if (wsServer != null)
                {
                    wsServer.Dispose();
                    wsServer = null;
                }
                wsClients.Clear();

                Application.Exit();
                Environment.Exit(0); // 强制终止进程
            }

        }




        #endregion

        #region DataGridView右键菜单事件处理
        /// <summary>
        /// 取消打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelPrint_Click(object sender, EventArgs e)
        {
            var dgv = GetCurrentDgv();
            if (dgv == null) return;
            var row = dgv.SelectedRows.Count > 0 ? dgv.SelectedRows[0] : null;
            if (row == null) return;
            int taskId = Convert.ToInt32(row.Cells["taskId"].Value);
            // 这里实现取消打印逻辑（如通过WMI删除打印任务）
            string printerName = tabControl1.SelectedTab.Text;
            // 查询打印队列，找到文档名包含 taskId 的作业
            string query = $"SELECT * FROM Win32_PrintJob WHERE Name LIKE '%{printerName}%'";
            using (var searcher = new System.Management.ManagementObjectSearcher(query))
            {
                foreach (System.Management.ManagementObject job in searcher.Get())
                {
                    int JobId = Convert.ToInt32(job["JobId"]);
                    if (JobId == taskId)
                    {
                        try
                        {
                            job.Delete(); // 删除打印任务
                            row.Cells["status"].Value = "已取消";
                            MessageBox.Show($"已取消打印任务：{taskId}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("取消打印失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                }
            }
            MessageBox.Show("未找到对应的打印任务，可能已完成或被清除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 重新打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reprint_Click(object sender, EventArgs e)
        {
            var dgv = GetCurrentDgv();
            if (dgv == null) return;
            var row = dgv.SelectedRows.Count > 0 ? dgv.SelectedRows[0] : null;
            if (row == null) return;
            if (row.Tag is PrintTaskInfo info)
            {
                // 复用原 taskName，或可选生成新 taskName
                string taskName = row.Cells["taskName"].Value.ToString();
                string status = row.Cells["status"].Value.ToString();
                if (status == "已完成")
                    PrintFile(info.FilePath, info.Data, taskName);
            }
            else
            {
                MessageBox.Show("未找到原始打印信息，无法重新打印。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 删除打印记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRecord_Click(object sender, EventArgs e)
        {
            var dgv = GetCurrentDgv();
            if (dgv == null) return;
            var row = dgv.SelectedRows.Count > 0 ? dgv.SelectedRows[0] : null;
            if (row == null) return;

            int taskId = Convert.ToInt32(row.Cells["taskId"].Value);
            string status = row.Cells["status"].Value?.ToString();
            string printerName = tabControl1.SelectedTab.Text;
            // 如果未完成，先删除打印队列中的任务
            if (status != "已完成" && status != "已取消")
            {
                string query = $"SELECT * FROM Win32_PrintJob WHERE Name LIKE '%{printerName}%'";
                using (var searcher = new System.Management.ManagementObjectSearcher(query))
                {
                    foreach (System.Management.ManagementObject job in searcher.Get())
                    {
                        int JobId = Convert.ToInt32(job["JobId"]);
                        if (JobId == taskId)
                        {
                            try
                            {
                                job.Delete(); // 删除打印任务
                                              // 可选：更新状态为已取消
                                              //row.Cells["status"].Value = "已取消";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("删除打印任务失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        }
                    }
                }
            }
            dgv.Rows.Remove(row);
        }
        /// <summary>
        /// 获取当前选中TabPage中的DataGridView
        /// </summary>
        /// <returns></returns>
        private DataGridView GetCurrentDgv()
        {
            if (tabControl1.SelectedTab == null) return null;
            return tabControl1.SelectedTab.Controls.OfType<DataGridView>().FirstOrDefault();
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 获取配置文件中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetConfigValue(string key)
        {
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CONFIG.ini");
            if (!File.Exists(iniPath))
                return null;

            // 指定UTF-8编码读取，防止中文乱码
            var lines = File.ReadAllLines(iniPath, System.Text.Encoding.UTF8);
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                {
                    return line.Split('=', 2)[1].Trim();
                }
            }
            return null;
        }
        /// <summary>
        ///  JsonObject 转 DataTable 工具方法
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private DataTable JsonObjectToDataTable(JsonObject json)
        {
            var dt = new DataTable();
            if (json == null) return dt;

            // 添加列
            foreach (var kv in json)
            {
                if (!dt.Columns.Contains(kv.Key))
                    dt.Columns.Add(kv.Key, typeof(string));
            }

            // 添加一行数据
            var row = dt.NewRow();
            foreach (var kv in json)
            {
                row[kv.Key] = kv.Value?.ToString() ?? "";
            }
            dt.Rows.Add(row);

            return dt;
        }

        class PrintTaskInfo
        {
            public string FilePath { get; set; }
            public object Data { get; set; }
        }

        #endregion



    }
}
