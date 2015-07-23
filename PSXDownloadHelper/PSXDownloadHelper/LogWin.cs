using PSXDH.BLL;
using PSXDH.Model;
using PSXDownloadHelper.Properties;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSXDownloadHelper
{
    public partial class LogWin : Form
    {
        #region 全局变量
        private static readonly object SyncObj = new object();
        private static LogWin _instance;

        private static int _lognum;
        private readonly Hashtable _ctrls = new Hashtable();

        private readonly ResourceManager _rm = new ResourceManager(typeof(LogWin));
        private readonly AppSettings _setting = new AppSettings();

        private int localversion = 0;
        #endregion

        public LogWin()
        {
            InitializeComponent();
        }

        public static LogWin Instance()
        {
            if (_instance == null)
                lock (SyncObj)
                {
                    if (_instance == null)
                        _instance = new LogWin();
                }
            return _instance;
        }

        private void LogWin_Load(object sender, EventArgs e)
        {
            try
            {
                BackColorSet();
                //关于信息读取
                webBrowser_weibo.Navigate(Application.StartupPath + @"\Extensions\weibo.htm");
                lb_product.Text = AssemblyTitle;
                lb_vision.Text += AssemblyVersion;
                lb_copyright.Text = AssemblyCopyright;
                localversion = int.Parse(AssemblyVersion.Replace(".", ""));

                //设置信息初始化
                cb_lg.SelectedIndex = MonitorLog.Languages.FirstOrDefault(r => r.Value == AppConfig.Instance().Language).Key;
                tb_rule.Text = AppConfig.Instance().Rule;
                ckb_mohu.Checked = AppConfig.Instance().IsDimrule;
                ckb_usecustome.Checked = AppConfig.Instance().IsUseCustomeHosts;
                Ckb_HideFrm.Checked = _setting.IsHideFrm;
                ckb_autoupdate.Checked = AppConfig.Instance().IsAutoCheckUpdate;
                ckb_autofindfile.Checked = btn_findloaclfolder.Enabled = AppConfig.Instance().IsAutoFindFile;
                lb_loaclfilefolder.Text = AppConfig.Instance().LocalFileDirectory;

                //离线设置
                LixianCofig();

                //自动更新
                AutoCheckUpdateAlert();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"程序运行时发生异常！异常信息：" + ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     最小化至托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogWin_SizeChanged(object sender, EventArgs e)
        {
            if (_setting.IsHideFrm)
                ShowInTaskbar = WindowState != FormWindowState.Minimized;
        }

        private void LogWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            ServerConfig.ServerInstance().Dispose();
            Environment.Exit(0);
        }

        #region 显示提示

        /// <summary>
        ///     显示提示
        /// </summary>
        /// <param name="str"></param>
        private void ShowAlert(string str)
        {
            lb_alert.Text = str;
            var p = new Point(Width / 2 - lb_alert.Width / 2, Height / 5);
            lb_alert.Location = p;
            lb_alert.Visible = true;
            timer_alert.Enabled = true;
        }

        private void timer_alert_Tick(object sender, EventArgs e)
        {
            if (lb_alert.Visible)
                lb_alert.Visible = false;

            timer_alert.Enabled = false;
        }

        #endregion

        #region 无线AP设置

        private void pic_ap_MouseHover(object sender, EventArgs e)
        {
            alertLabel1.Visible = true;
        }

        private void pic_ap_MouseLeave(object sender, EventArgs e)
        {
            alertLabel1.Visible = false;
        }

        private void pic_ap_Click(object sender, EventArgs e)
        {
            var ap = new WifiAp();
            ap.Show();
        }

        #endregion

        #region 程序集访问

        /// <summary>
        ///     程序集访问
        /// </summary>
        public string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string AssemblyTitle
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #endregion

        #region 捕捉记录

        public void AddUrl(UrlInfo urlinfo)
        {
            if (flp_log.InvokeRequired)
            {
                AddUrldelegate d = AddUrl;
                flp_log.Invoke(d, urlinfo);
            }
            else
            {
                if (AppConfig.Instance().Rule.Length <= 0)
                {
                    MessageBox.Show(_rm.GetString("rulealert"));
                    return;
                }
                if (!MonitorLog.RegexUrl(urlinfo.PsnUrl))
                    return;

                bool iscdn = urlinfo.IsCdn;
                if (_ctrls.Contains(urlinfo.PsnUrl))
                {
                    if (flp_log.Controls.ContainsKey((string)_ctrls[urlinfo.PsnUrl]))
                    {
                        urlinfo.MarkTxt =
                            ((UrlReplace)
                             flp_log.Controls.Find((string)_ctrls[urlinfo.PsnUrl], true).FirstOrDefault()).MarkTxt;
                        flp_log.Controls.RemoveByKey((string)_ctrls[urlinfo.PsnUrl]);
                        _ctrls.Remove(urlinfo.PsnUrl);
                    }
                    if (flp_lixian.Controls.ContainsKey((string)_ctrls[urlinfo.PsnUrl]))
                    {
                        urlinfo.MarkTxt =
                            ((UrlReplace)
                             flp_lixian.Controls.Find((string)_ctrls[urlinfo.PsnUrl], true).FirstOrDefault())
                                .MarkTxt;
                        flp_lixian.Controls.RemoveByKey((string)_ctrls[urlinfo.PsnUrl]);
                        _ctrls.Remove(urlinfo.PsnUrl);
                    }
                }


                UrlInfo temp = DataHistoryOperate.GetInfo(urlinfo.PsnUrl);
                if (temp != null)
                {
                    if (!string.IsNullOrEmpty(temp.ReplacePath) && !File.Exists(temp.ReplacePath))
                        temp.ReplacePath = "";
                    urlinfo = temp;
                }

                var u = new UrlReplace
                    {
                        Name = "url_replace" + (_lognum++).ToString(),
                        PsnUrl = urlinfo.PsnUrl,
                        LocalPath =
                            String.IsNullOrEmpty(urlinfo.ReplacePath)
                                ? _rm.GetString("replacetip")
                                : urlinfo.ReplacePath,
                        LogTime = DateTime.Now,
                        MarkTxt = urlinfo.MarkTxt,
                        IsLixian = urlinfo.IsLixian,
                        LixianUrl = urlinfo.LixianUrl,
                        IsCdn = iscdn,
                        Host = urlinfo.Host
                    };
                u.ClickReplaceEvent += u_ClickReplaceEvent;
                u.NameTextChanged += u_NameTextChanged;
                u.EnableLixian += u_EnableLixian;
                u.DragDropFileto += u_DragDropFileto;
                u.PingClick += u_PingClick;
                u.SetLixian(AppConfig.Instance().EnableLixian);
                if (u.IsLixian)
                {
                    flp_lixian.Controls.Add(u);
                    var scrollp = new Point(0, flp_lixian.Height - flp_lixian.AutoScrollPosition.Y);
                    flp_lixian.AutoScrollPosition = scrollp;
                }
                else
                {
                    flp_log.Controls.Add(u);
                    var scrollp = new Point(0, flp_log.Height - flp_log.AutoScrollPosition.Y);
                    flp_log.AutoScrollPosition = scrollp;
                }
                tab_lixian.Text = _rm.GetString("tab_lixian.Text") + "(" + flp_lixian.Controls.Count.ToString() +
                                  ")";
                tabPage_log.Text = _rm.GetString("tabPage_log.Text") + "(" + flp_log.Controls.Count.ToString() + ")";
                if (!_ctrls.ContainsKey(urlinfo.PsnUrl))
                    _ctrls.Add(urlinfo.PsnUrl, u.Name);
            }
        }

        private void u_PingClick(object sender, EventArgs e, UrlReplace obj)
        {
            obj.ShowPing(obj.Host);
        }

        /// <summary>
        ///     拖动选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="obj"></param>
        private void u_DragDropFileto(object sender, EventArgs e, UrlReplace obj)
        {
            var p = ((Array)((DragEventArgs)e).Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            obj.LocalPath = p;
            UrlOperate.PushUrl(obj.ToUrlInfo());
            DataHistoryOperate.AddLog(obj.ToUrlInfo());
        }

        private void u_EnableLixian(object sender, EventArgs e, UrlReplace obj)
        {
            DataHistoryOperate.AddLog(obj.ToUrlInfo());
            UrlOperate.PushLixianUrl(obj.PsnUrl, obj.LixianUrl);
        }

        private void u_NameTextChanged(object sender, EventArgs e, UrlReplace obj)
        {
            DataHistoryOperate.AddLog(obj.ToUrlInfo());
        }

        private void u_ClickReplaceEvent(object sender, EventArgs e, UrlReplace obj)
        {
            if (ofd_file.ShowDialog() == DialogResult.OK)
            {
                obj.LocalPath = ofd_file.FileName;
                UrlOperate.PushUrl(obj.ToUrlInfo());
                DataHistoryOperate.AddLog(obj.ToUrlInfo());
            }
        }



        private delegate void AddUrldelegate(UrlInfo ui);

        #endregion

        #region 设置
        /// <summary>
        /// 设置主题
        /// </summary>
        private void BackColorSet()
        {
            MonitorLog.Themes.ForEach(delegate(string s)
                {
                    var btn = new Button { BackColor = ColorTranslator.FromHtml(s), Width = 20, Height = 20 };
                    btn.Click += btn_Click;
                    flp_sp.Controls.Add(btn);
                });
            if (!string.IsNullOrEmpty(_setting.Theme))
                mainPanel1.BackColor = ColorTranslator.FromHtml(_setting.Theme);
            btn_color.BackColor = mainPanel1.BackColor;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            btn_color.BackColor = ((Button)sender).BackColor;
        }

        /// <summary>
        ///     更改语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_lg_SelectedIndexChanged(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(MonitorLog.Languages[cb_lg.SelectedIndex]);
            string str = _setting.Language;
            _setting.Language = MonitorLog.Languages[cb_lg.SelectedIndex];
            _setting.Save();
            if (_setting.Language != str)
                MessageBox.Show(_rm.GetString("setlanguage"));
        }

        /// <summary>
        /// 自动查找本地目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckb_autofindfile_CheckedChanged(object sender, EventArgs e)
        {
            btn_findloaclfolder.Enabled = ckb_autofindfile.Checked;
            AppConfig.Instance().IsAutoFindFile = _setting.IsAutoFindFile = ckb_autofindfile.Checked;
        }

        private void btn_findloaclfolder_Click(object sender, EventArgs e)
        {
            if (fbd_localfilefolder.ShowDialog() != DialogResult.OK)
                return;

            lb_loaclfilefolder.Text = fbd_localfilefolder.SelectedPath;
            AppConfig.Instance().LocalFileDirectory = _setting.LocalFileDirectory = lb_loaclfilefolder.Text;
        }

        /// <summary>
        ///     保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_savesetting_Click(object sender, EventArgs e)
        {
            AppConfig.Instance().Rule = _setting.Rule = tb_rule.Text;
            AppConfig.Instance().Theme = _setting.Theme = btn_color.BackColor.ToArgb().ToString(CultureInfo.InvariantCulture);
            AppConfig.Instance().IsAutoCheckUpdate = _setting.IsAutoCheckUpdate = ckb_autoupdate.Checked;
            AppConfig.Instance().IsDimrule = _setting.IsDimrule = ckb_mohu.Checked;
            AppConfig.Instance().IsUseCustomeHosts = _setting.IsUseCustomeHosts = ckb_usecustome.Checked;
            AppConfig.Instance().IsAutoFindFile = _setting.IsAutoFindFile = ckb_autofindfile.Checked;
            AppConfig.Instance().LocalFileDirectory = _setting.LocalFileDirectory = lb_loaclfilefolder.Text;
            _setting.IsHideFrm = Ckb_HideFrm.Checked;
            _setting.Save();

            Color bg = ColorTranslator.FromHtml(_setting.Theme);
            mainPanel1.BackColor = bg;
            ServerConfig.ServerInstance().mainPanel1.BackColor = bg;

            alert_set.Text = _rm.GetString("settingapply");
            alert_set.Visible = true;
        }

        /// <summary>
        ///     选择界面颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_color_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btn_color.BackColor = colorDialog1.Color;
            }
        }

        /// <summary>
        /// 提供翻译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void link_translation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.google.com/spreadsheet/ccc?key=0AidC-uh8A7lkdExSUkVZTjlCV2VfclhUWlJJWTQwV3c#gid=0");
        }

        #endregion

        #region 检查更新

        private void btn_checkupdate_Click(object sender, EventArgs e)
        {
            var t = new Task(() =>
            {
                try
                {
                    var msg = MonitorLog.GetNewVersion() > localversion ? "<b>新版本已经发布，请下载更新</b><br/>" : "<b>你使用的已经是最新版本</b><br/>";
                    ShowUpdateLog(msg + MonitorLog.GetUpdateLog());
                }
                catch
                {
                    ShowUpdateLog("");
                }
            });
            t.Start();

            btn_checkupdate.Text = _rm.GetString("checkupdatenow");
            btn_checkupdate.Enabled = false;
        }
        /// <summary>
        /// 检查更新
        /// </summary>
        private delegate void Getupdatedelegate(string str);
        private void ShowUpdateLog(string strValue)
        {
            if (btn_checkupdate.InvokeRequired)
            {
                Getupdatedelegate g = ShowUpdateLog;
                btn_checkupdate.Invoke(g, strValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(strValue))
                {
                    var cu = new CheckUpdate(strValue);
                    cu.Show();
                }
                else
                {
                    var dialogresult = MessageBox.Show(_rm.GetString("updateerror"), "Warning", MessageBoxButtons.YesNoCancel);
                    _setting.IsAutoCheckUpdate = dialogresult != DialogResult.Yes;
                    ckb_autoupdate.Checked = _setting.IsAutoCheckUpdate;
                    _setting.Save();
                }
                btn_checkupdate.Text = _rm.GetString("btn_checkupdate.Text");
                btn_checkupdate.Enabled = true;
            }
        }

        /// <summary>
        /// 自动更新提示
        /// </summary>
        private void AutoCheckUpdateAlert()
        {
            if (!AppConfig.Instance().IsAutoCheckUpdate)
                return;

            var t = new Task(() =>
                {
                    try
                    {
                        var newversion = MonitorLog.GetNewVersion();
                        if (newversion > localversion)
                            if (MessageBox.Show(_rm.GetString("UpdateAlert")) == DialogResult.OK)
                                ShowUpdateLog(MonitorLog.GetUpdateLog());

                        AppConfig.Instance().LastCheckUpdate = _setting.LastCheckUpdate = DateTime.Now;
                        _setting.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            t.Start();
        }
        #endregion

        #region 历史记录

        /// <summary>
        ///     显示历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_showalllog_Click(object sender, EventArgs e)
        {
            flp_history.Controls.Clear();
            var logs = DataHistoryOperate.GetAllUrl();
            logs.ForEach(delegate(UrlInfo u)
                {
                    var ur = new UrlReplace
                        {
                            PsnUrl = u.PsnUrl,
                            LocalPath = u.ReplacePath,
                            MarkTxt = u.MarkTxt,
                            DelBtnVisible = true,
                            IsLixian = u.IsLixian,
                            LixianUrl = u.LixianUrl
                        };
                    ur.DelRecord += ur_DelRecord;
                    ur.NameTextChanged += u_NameTextChanged;
                    ur.ClickReplaceEvent += u_ClickReplaceEvent;
                    flp_history.Controls.Add(ur);
                });
        }

        /// <summary>
        ///     删除一条记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="obj"></param>
        private void ur_DelRecord(object sender, EventArgs e, UrlReplace obj)
        {
            DataHistoryOperate.DelLog(obj.ToUrlInfo());
        }

        /// <summary>
        ///     删除历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_delall_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(_rm.GetString("historydelalert"), @"警告", MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Information) == DialogResult.OK)
            {
                try
                {
                    string paths = Application.StartupPath + @"\DataFiles\DataHistory.xml";
                    if (File.Exists(paths))
                    {
                        File.Delete(paths);
                    }
                    flp_history.Controls.Clear();
                    if (MessageBox.Show(_rm.GetString("historydelsuccess")) == DialogResult.OK)
                    {
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_rm.GetString("historydelerror") + ex.Message);
                }
            }
        }

        #endregion

        #region 离线

        /// <summary>
        ///     启动离线帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lxhelp_Click(object sender, EventArgs e)
        {
            var p = new Process
                {
                    StartInfo = { Arguments = "http://www.elanblog.com/?page_id=14", FileName = "iexplore.exe" }
                };
            p.Start();
            p.Close();
        }

        private void btn_server_Click(object sender, EventArgs e)
        {
            ServerConfig sw = ServerConfig.ServerInstance();
            sw.Show();
            if (sw.WindowState == FormWindowState.Minimized)
                sw.WindowState = FormWindowState.Normal;
            sw.Activate();
        }

        /// <summary>
        ///     设置离线加速参数
        /// </summary>
        private void LixianCofig()
        {
            tb_cookie1.Text = _setting.cookie1;
            tb_cookie2.Text = _setting.cookie2;
            tb_host1.Text = _setting.host1;
            tb_host2.Text = _setting.host2;
            tb_lx1.Text = _setting.lixian1;
            tb_lx2.Text = _setting.lixian2;
            if (_setting.lixianConfig == 1)
                ckb_lx1.Checked = true;
            else
                ckb_lx2.Checked = true;
            UpdateLXconfig();
        }

        private void UpdateLXconfig()
        {
            AppConfig.Instance().Host = _setting["host" + _setting.lixianConfig].ToString();
            AppConfig.Instance().Cookie = _setting["cookie" + _setting.lixianConfig].ToString();
        }

        private void btn_lx1_Click(object sender, EventArgs e)
        {
            _setting.lixian1 = tb_lx1.Text;
            _setting.host1 = tb_host1.Text;
            _setting.cookie1 = tb_cookie1.Text;
            _setting.Save();
            UpdateLXconfig();
            ShowAlert(_rm.GetString("savesuccess"));
        }

        private void btn_lx2_Click(object sender, EventArgs e)
        {
            _setting.lixian2 = tb_lx2.Text;
            _setting.host2 = tb_host2.Text;
            _setting.cookie2 = tb_cookie2.Text;
            _setting.Save();
            UpdateLXconfig();
            ShowAlert(_rm.GetString("savesuccess"));
        }

        private void ckb_lx1_CheckedChanged(object sender, EventArgs e)
        {
            ckb_lx2.Checked = !ckb_lx1.Checked;
            _setting.lixianConfig = ckb_lx1.Checked ? 1 : 2;
            _setting.Save();
        }

        private void ckb_lx2_CheckedChanged(object sender, EventArgs e)
        {
            ckb_lx1.Checked = !ckb_lx2.Checked;
            _setting.lixianConfig = ckb_lx1.Checked ? 1 : 2;
            _setting.Save();
        }

        #endregion

        #region CDN加速

        public void CheckCdn()
        {
            var t = new Task(() =>
                {
                    try
                    {
                        CdnOperate.ReadCdnConfig();
                        Thread.Sleep(2500);
                        OnCdnCheckComplete();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(_rm.GetString("cdnerror") + ex.Message, "Error");
                        OnCdnCheckComplete(false);
                    }
                });
            t.Start();
        }

        private void OnCdnCheckComplete(bool isSuccess = true)
        {
            if (pic_cdnloading.InvokeRequired)
            {
                CdnCheckCompleteDelegate o = OnCdnCheckComplete;
                pic_cdnloading.Invoke(o, isSuccess);
            }
            else
            {
                if (!isSuccess)
                {
                    pic_cdnloading.Image = Resources.minus;
                    toolTip1.SetToolTip(pic_cdnloading, _rm.GetString("cdnfileerror"));
                    btn_CheckCdn.Enabled = false;
                    return;
                }
                pic_cdnloading.Visible = false;
                pic_cdnsuccess.Visible = true;
                btn_CheckCdn.Enabled = true;
                btn_editcdn.Enabled = true;
                Btn_ExportCdn.Enabled = true;
                _setting.LastCheckCdn = DateTime.Now.Date;
                _setting.Save();
            }
        }

        private void btn_CheckCdn_Click(object sender, EventArgs e)
        {
            pic_cdnsuccess.Visible = false;
            pic_cdnloading.Image = Resources.cdnloading;
            pic_cdnloading.Visible = true;
            btn_CheckCdn.Enabled = false;
            _setting.LastCheckCdn = DateTime.Now.Date;
            CheckCdn();
        }

        private void btn_editcdn_Click(object sender, EventArgs e)
        {
            CMS_EditMenu.Show((Control)sender, 0, 23);
        }

        private void menu_editcdnhost_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
                {
                    FileName = @"Hosts\CdnHosts.ini"
                });
        }

        private void menu_editcustom_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
                {
                    FileName = @"Hosts\CustomHosts.ini"
                });
        }

        private void Btn_ExportCdn_Click(object sender, EventArgs e)
        {
            CdnOperate.ExportHost();
            MessageBox.Show(_rm.GetString("exportsuccess"));
        }

        private delegate void CdnCheckCompleteDelegate(bool isSuccess);

        /// <summary>
        /// 打开最新cdn地址页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_openservercdn_Click(object sender, EventArgs e)
        {
            var p = new Process
            {
                StartInfo = { Arguments = "http://www.elanblog.com/extensions/service/cdnhosts.txt", FileName = "iexplore.exe" }
            };
            p.Start();
            p.Close();
        }

        /// <summary>
        /// 下载最新cdn地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_updatecdn_Click(object sender, EventArgs e)
        {
            var isUpdate = MessageBox.Show(_rm.GetString("cdn_update"), "Warning", MessageBoxButtons.OKCancel);
            if (isUpdate != DialogResult.OK)
                return;

            if (MonitorLog.UpdateCdn())
                MessageBox.Show(_rm.GetString("cdn_updatesuccess"));
            else
                MessageBox.Show(_rm.GetString("cdn_updatefailed"));
        }


        #endregion

        private void btn_featuring_Click(object sender, EventArgs e)
        {
            var f1 = new Featuring();
            f1.Show();
        }

    }
}