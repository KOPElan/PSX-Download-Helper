using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Resources;
using System.Windows.Forms;
using PSXDH.BLL;
using PSXDH.HttpsHelp;
using PSXDH.Model;
using PSXDownloadHelper.Code;
using PSXDownloadHelper.Properties;

namespace PSXDownloadHelper
{
    public partial class ServerConfig : Form
    {
        private static HttpListenerHelp _listener;
        private static ServerConfig _instance;
        private readonly LogWin _lw = LogWin.Instance();
        private readonly ResourceManager _rm = new ResourceManager(typeof(ServerConfig));
        private readonly AppSettings _setting = new AppSettings();

        public ServerConfig()
        {
            InitializeComponent();
        }

        public static ServerConfig ServerInstance()
        {
            return _instance ?? (_instance = new ServerConfig());
        }

        private void IPConfig_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppConfig.Instance().Theme))
                    mainPanel1.BackColor = ColorTranslator.FromHtml(AppConfig.Instance().Theme);

                GetServerIp();

                //应用appsetting
                ckb_pcp.Checked = AppConfig.Instance().IsUsePcProxy;
                ckb_loaclspeed.Checked = AppConfig.Instance().IsUserLocal;
                ckb_lixianspeed.Checked = AppConfig.Instance().EnableLixian;
                cb_conntype.SelectedIndex = AppConfig.Instance().ConnType;
                ckb_cdn.Checked = AppConfig.Instance().IsUseCdn;

                //图标透明
                pic_logo.Parent = mainPanel1;

                //刚开始不能打开日志窗口
                OpenLogToolStripMenuItem.Enabled = false;

                this.Shown += ServerConfig_Shown;
            }
            catch (Exception ex)
            {
                MessageBox.Show(_rm.GetString("apperrorinfo") + ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 显示更新日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerConfig_Shown(object sender, EventArgs e)
        {
            return;
            if (!_setting.IsShowLog) return;
            var updatelog = new Featuring();
            updatelog.ShowDialog();
            updatelog.Focus();
            SettingHelper.ChangeSetting("IsShowLog", false);
        }

        private void GetServerIp()
        {
            try
            {
                var iplist = ProxyServer.GetHostIp();
                IPAddress ipshow;
                cb_ip.Items.Clear();
                cb_ip.Items.AddRange(iplist);
                cb_ip.Text = (!string.IsNullOrEmpty(AppConfig.Instance().Ip) && IPAddress.TryParse(AppConfig.Instance().Ip, out ipshow) && iplist.Contains(ipshow)
                                  ? AppConfig.Instance().Ip
                                  : cb_ip.Items[0].ToString());
                tb_port.Text = AppConfig.Instance().Port.ToString();
            }
            catch
            {
                cb_ip.SelectedText = String.Empty;
                tb_port.Text = AppConfig.Instance().Port.ToString();
            }
        }

        #region 启动服务
        /// <summary>
        ///     启动端口监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Dispose();
                    _listener = null;
                    btn_start.Image = Resources.play;
                    btn_start.Text = _rm.GetString("btn_start.Text");
                    _lw.Hide();
                    OpenLogToolStripMenuItem.Enabled = false;
                }
                else
                {
                    var address = IPAddress.Parse(cb_ip.Text);
                    var port = int.Parse(tb_port.Text);
                    _listener = new HttpListenerHelp(address, port, _lw.AddUrl);
                    _listener.Start();

                    StartServer();
                    btn_start.Image = Resources.minus;
                    btn_start.Text = _rm.GetString("stop");
                    Hide();

                    _lw.Show();
                    _lw.Activate();
                    OpenLogToolStripMenuItem.Enabled = true;
                }
                cb_ip.Enabled = !cb_ip.Enabled;
                tb_port.Enabled = !tb_port.Enabled;
                ckb_lixianspeed.Enabled = !ckb_lixianspeed.Enabled;
                ckb_loaclspeed.Enabled = !ckb_loaclspeed.Enabled;
                cb_conntype.Enabled = !cb_conntype.Enabled;
                ckb_cdn.Enabled = !ckb_cdn.Enabled;
            }
            catch (Exception ex)
            {
                if (_listener != null)
                {
                    _listener.Dispose();
                    _listener = null;
                }

                MessageBox.Show(_rm.GetString("starterrorinfo") + ex.Message);
            }
        }

        /// <summary>
        /// 启动服务设置Appconfig
        /// </summary>
        private void StartServer()
        {
            if (ckb_cdn.Checked)
            {
                _lw.CheckCdn();//每次打开都检查CDN状态
            }
            else
            {
                _lw.pic_cdnloading.Image = Resources.minus;
                _lw.toolTip1.SetToolTip(_lw.pic_cdnloading, _rm.GetString("cdndiable"));
                _lw.btn_CheckCdn.Enabled = false;
            }

            //日志窗口各服务状态显示
            _lw.btn_server.BackgroundImage = Resources.play;
            _lw.btn_pcp.BackgroundImage = ckb_pcp.Checked ? Resources.play : Resources.minus;
            _lw.btn_localspeed.BackgroundImage = ckb_loaclspeed.Checked ? Resources.play : Resources.minus;
            _lw.btn_lixianspeed.BackgroundImage = ckb_lixianspeed.Checked ? Resources.play : Resources.minus;
            _lw.lb_conntype.Text = cb_conntype.Text;
            ConfigChange();
        }
        #endregion

        #region 启动设置变化
        public void ConfigChange()
        {
            AppConfig.Instance().EnableLixian = _setting.EnableLixian = ckb_lixianspeed.Checked;
            AppConfig.Instance().IsUseCdn = _setting.IsUseCdn = ckb_cdn.Checked;
            AppConfig.Instance().ConnType = _setting.ConnType = cb_conntype.SelectedIndex;
            AppConfig.Instance().IsUsePcProxy = _setting.IsUsePcProxy = ckb_pcp.Checked;
            AppConfig.Instance().EnableLixian = _setting.EnableLixian = ckb_lixianspeed.Checked;
            AppConfig.Instance().IsUserLocal = _setting.IsUserLocal = ckb_loaclspeed.Checked;
            AppConfig.Instance().Ip = _setting.Ip = cb_ip.Text;
            _setting.Save();
        }

        private void tb_port_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AppConfig.Instance().Port = _setting.Port = int.Parse(tb_port.Text);
                _setting.Save();
            }
            catch
            {
                tb_port.Text = "8080";
                MessageBox.Show("请输入正确的端口号。1~65536");
            }
        }
        #endregion

        #region 打开AP设置
        private void pic_ap_Click(object sender, EventArgs e)
        {
            var ap = new WifiAp();
            if (ap.ShowDialog() == DialogResult.OK)
            {
                GetServerIp();
            }
        }

        private void pic_ap_MouseHover(object sender, EventArgs e)
        {
            alertLabel1.Visible = true;
        }

        private void pic_ap_MouseLeave(object sender, EventArgs e)
        {
            alertLabel1.Visible = false;
        }
        #endregion

        #region #托盘显示

        private void ServerConfig_SizeChanged(object sender, EventArgs e)
        {
            ShowInTaskbar = WindowState != FormWindowState.Minimized;
        }

        /// <summary>
        /// 托盘图标功能增强，当服务运行时打开日志窗口，服务没运行时打开服务窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_notify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_listener != null)
            {
                _lw.Show();
                _lw.WindowState = FormWindowState.Normal;
                _lw.Activate();
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }

        }
        private void server_notify_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                Menu_Notify.Show();
        }

        private void OpenServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void OpenLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _lw.Show();
            _lw.WindowState = FormWindowState.Normal;
            _lw.Activate();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }
        #endregion

        private void ServerConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }
    }
}