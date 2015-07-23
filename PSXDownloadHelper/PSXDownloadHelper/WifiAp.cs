using System;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace PSXDownloadHelper
{
    public partial class WifiAp : Form
    {
        private readonly AppSettings _mySetting = new AppSettings();
        private readonly ResourceManager _rm = new ResourceManager(typeof(WifiAp));

        public WifiAp()
        {
            InitializeComponent();
        }

        private void WIFIAP_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_mySetting.Theme))
                mainPanel1.BackColor = ColorTranslator.FromHtml(_mySetting.Theme);
            tb_ssid.Text = _mySetting.Ssid;
            tb_pwd.Text = _mySetting.ApPassword;
        }

        private void btn_ap_Click(object sender, EventArgs e)
        {
            if (tb_ssid.Text == "" || tb_pwd.Text == "")
            {
                MessageBox.Show(_rm.GetString("ssidpwdnull"));
                return;
            }
            if (tb_pwd.Text.Trim().Length != 10)
            {
                MessageBox.Show(_rm.GetString("pwdtip"));
                return;
            }

            _mySetting.Ssid = tb_ssid.Text;
            _mySetting.ApPassword = tb_pwd.Text;
            _mySetting.Save();
            string cmdtxt = string.Format("netsh wlan set hostednetwork mode=allow ssid={0} key={1}", _mySetting.Ssid,
                                          _mySetting.ApPassword);

            var cmd = new Process
                {
                    StartInfo =
                        {
                            FileName = "cmd.exe",
                            Arguments = cmdtxt,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            RedirectStandardError = true
                        }
                };
            cmd.Start();
            cmd.StandardInput.WriteLine(cmdtxt);
            cmd.StandardInput.WriteLine("exit");
            string output = cmd.StandardOutput.ReadToEnd();
            cmd.StandardOutput.Close();
            cmd.WaitForExit();
            cmd.Close();
            lb_result.Text = output.Substring(output.IndexOf(cmdtxt) + cmdtxt.Length,
                                              output.Replace("exit\r\n", "").LastIndexOf("\r\n") -
                                              output.IndexOf(cmdtxt) - cmdtxt.Length);
            lb_result.Text = string.IsNullOrEmpty(lb_result.Text) ? _rm.GetString("aperror") : lb_result.Text;
            lb_result.Visible = true;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            const string cmdtxt = "netsh wlan start hostednetwork";

            var cmd = new Process
                {
                    StartInfo =
                        {
                            FileName = "cmd.exe",
                            Arguments = cmdtxt,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            RedirectStandardError = true
                        }
                };
            //cmd.StartInfo.Arguments = "/c C:\\Windows\\System32\\cmd.exe";
            //cmd.StartInfo.Verb = "RunAs";
            cmd.Start();
            cmd.StandardInput.WriteLine(cmdtxt);
            cmd.StandardInput.WriteLine("exit");
            string output = cmd.StandardOutput.ReadToEnd();
            cmd.StandardOutput.Close();
            cmd.WaitForExit();
            cmd.Close();
            lb_result.Text = output.Substring(output.IndexOf(cmdtxt) + cmdtxt.Length,
                                              output.Replace("exit\r\n", "").LastIndexOf("\r\n") -
                                              output.IndexOf(cmdtxt) - cmdtxt.Length);
            lb_result.Text = string.IsNullOrEmpty(lb_result.Text.Trim())
                                 ? _rm.GetString("aptip") + "\n" + cmdtxt  //aptip=无法开启无线AP，请使用管理员权限运行程序重试。你也可以手动方式开启，以管理员方式打开CMD，输入：
                                 : lb_result.Text;
            lb_result.Visible = true;
        }

        private void aphelp_MouseHover(object sender, EventArgs e)
        {
            alertLabel1.Visible = true;
        }

        private void aphelp_MouseLeave(object sender, EventArgs e)
        {
            alertLabel1.Visible = false;
        }

        private void WIFIAP_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}