using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintApp
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            // 绑定本地打印机列表
            BindPrintersTocomboBox();


            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CONFIG.ini");
            if (File.Exists(iniPath))
            {
                var lines = File.ReadAllLines(iniPath);
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("socket_port="))
                    {
                        string port = line.Split('=')[1].Trim();
                        textBox1.Text = port;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// 绑定本地打印机列表到下拉框
        /// </summary>
        private void BindPrintersTocomboBox()
        {
            comboBox1.Items.Clear();
            // 获取当前系统默认打印机
            string defaultPrinter = new System.Drawing.Printing.PrinterSettings().PrinterName;
            int defaultIndex = -1;
            int i = 0;

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(printer);
                if (printer == defaultPrinter)
                    defaultIndex = i;
                i++;
            }

            // 设置默认打印机为选中项
            if (defaultIndex >= 0)
                comboBox1.SelectedIndex = defaultIndex;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CONFIG.ini");
            string key = "socket_port=";
            string newValue = key + textBox1.Text.Trim();

            List<string> lines = new List<string>();
            bool found = false;

            if (File.Exists(iniPath))
            {
                lines = File.ReadAllLines(iniPath).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Trim().StartsWith(key))
                    {
                        lines[i] = newValue;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                lines.Add(newValue);
            }

            File.WriteAllLines(iniPath, lines);
            // 自动设置默认打印机
            if (comboBox1.SelectedItem is string printerName && !string.IsNullOrWhiteSpace(printerName))
            {
                // 获取主窗体并调用设置方法
                if (this.Owner is PrintingApp mainForm)
                {
                    mainForm.SetSystemDefaultPrinter(printerName);
                    mainForm.BindPrintersToMenu(); // 刷新主窗体菜单对号

                }
                else
                {
                    // 直接调用 Win32 API 作为兜底
                    PrintingApp.SetDefaultPrinter(printerName);
                }
            }

            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

    }
}
