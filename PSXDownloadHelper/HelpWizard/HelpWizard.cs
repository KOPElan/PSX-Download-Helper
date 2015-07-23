using PSXDH.BLL;
using PSXDH.HttpsHelp;
using PSXDH.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HelpWizard
{
    public partial class HelpWizard : Form
    {
        public HelpWizard()
        {
            InitializeComponent();
        }

        public void GetAppConfig()
        {
            try
            {
                var colorTemp = new Color();
                if (!string.IsNullOrEmpty(AppConfig.Instance().Theme))
                    colorTemp = ColorTranslator.FromHtml(AppConfig.Instance().Theme);
                tb_Log.Text = "设置获取成功！";
            }
            catch (Exception ex)
            {
                tb_Log.Text = "获取设置时出错！\n错误信息：" + ex.Message;
            }
        }

        private void GetServerIp()
        {
            try
            {
                var iplist = ProxyServer.GetHostIp();
                IPAddress ipshow;
                if (iplist.Count() == 0)
                    tb_Log.Text = "IP地址获取失败！";

                var port = AppConfig.Instance().Port.ToString();

                tb_Log.Text += "\n获取IP地址开始：";
                foreach (var ip in iplist)
                    tb_Log.Text += "\nIP:" + ip;

                tb_Log.Text += "\n请点击 下一步 继续。";
            }
            catch (Exception ex)
            {
                tb_Log.Text += "\n获取本机IP地址时出错！\n错误信息：" + ex.Message;
            }
        }

        private void StartServer()
        {
            try
            {
                var address = IPAddress.Parse(tb_Ip.Text);
                var port = int.Parse(tb_Port.Text);
                var listener = new HttpListenerHelp(address, port, null);
                listener.Start();

                Thread.Sleep(2000);
                listener.Dispose();

                tb_Log.Text += "\n服务启动正常！";
            }
            catch (Exception ex)
            {
                tb_Log.Text += "\n服务启动异常！请修改IP地址或者端口号重新测试！\n错误信息：" + ex.Message;
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tb_Ip.Text) || String.IsNullOrEmpty(tb_Port.Text))
            {
                MessageBox.Show("IP地址和端口号不能为空，请填写后继续！");
                return;
            }

            btn_Start.Enabled = false;
            GetAppConfig();
            btn_Next.Visible = true;
        }

        int i = 2;
        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (i == 2)
            {
                btn_Next.Enabled = false;
                GetServerIp();
                i--;
                btn_Next.Enabled = true;
            }
            else if (i == 1)
            {
                btn_Next.Enabled = false;
                StartServer();
                tb_Log.Text += "\n诊断结束。";
            }
        }
    }
}
