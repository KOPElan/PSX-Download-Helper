namespace PSXDownloadHelper
{
    partial class ServerConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                _listener.Dispose();
                _listener = null;
            }
            catch
            {
            }
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConfig));
            this.mainPanel1 = new MetroStyle.MainPanel();
            this.alertLabel1 = new MetroStyle.AlertLabel();
            this.formControl1 = new MetroStyle.FormControl();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_start = new MetroStyle.MetroButton();
            this.cb_ip = new System.Windows.Forms.ComboBox();
            this.pic_logo = new System.Windows.Forms.PictureBox();
            this.ckb_pcp = new System.Windows.Forms.CheckBox();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.ckb_loaclspeed = new System.Windows.Forms.CheckBox();
            this.pic_ap = new System.Windows.Forms.PictureBox();
            this.cb_conntype = new System.Windows.Forms.ComboBox();
            this.ckb_cdn = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ckb_lixianspeed = new System.Windows.Forms.CheckBox();
            this.server_notify = new System.Windows.Forms.NotifyIcon(this.components);
            this.Menu_Notify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ap)).BeginInit();
            this.Menu_Notify.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel1
            // 
            this.mainPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(185)))), ((int)(((byte)(240)))));
            resources.ApplyResources(this.mainPanel1, "mainPanel1");
            this.mainPanel1.BorderAlpha = 100;
            this.mainPanel1.BorderColor = System.Drawing.SystemColors.Highlight;
            this.mainPanel1.BorderSize = 2;
            this.mainPanel1.Controls.Add(this.alertLabel1);
            this.mainPanel1.Controls.Add(this.formControl1);
            this.mainPanel1.Controls.Add(this.label4);
            this.mainPanel1.Controls.Add(this.btn_start);
            this.mainPanel1.Controls.Add(this.cb_ip);
            this.mainPanel1.Controls.Add(this.pic_logo);
            this.mainPanel1.Controls.Add(this.ckb_pcp);
            this.mainPanel1.Controls.Add(this.tb_port);
            this.mainPanel1.Controls.Add(this.ckb_loaclspeed);
            this.mainPanel1.Controls.Add(this.pic_ap);
            this.mainPanel1.Controls.Add(this.cb_conntype);
            this.mainPanel1.Controls.Add(this.ckb_cdn);
            this.mainPanel1.Controls.Add(this.label1);
            this.mainPanel1.Controls.Add(this.label5);
            this.mainPanel1.Controls.Add(this.label3);
            this.mainPanel1.Controls.Add(this.ckb_lixianspeed);
            this.mainPanel1.MainForm = this;
            this.mainPanel1.Name = "mainPanel1";
            // 
            // alertLabel1
            // 
            resources.ApplyResources(this.alertLabel1, "alertLabel1");
            this.alertLabel1.BackColor = System.Drawing.Color.Orange;
            this.alertLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.alertLabel1.left = 0;
            this.alertLabel1.MainForm = null;
            this.alertLabel1.Name = "alertLabel1";
            this.alertLabel1.ShowInMiddle = false;
            this.alertLabel1.TimeShow = 10000;
            this.alertLabel1.top = 0;
            // 
            // formControl1
            // 
            resources.ApplyResources(this.formControl1, "formControl1");
            this.formControl1.BackColor = System.Drawing.Color.Transparent;
            this.formControl1.MainForm = this;
            this.formControl1.MinAmtion = 20;
            this.formControl1.Name = "formControl1";
            this.formControl1.ShowMaxandMinBox = true;
            this.formControl1.ShowMaxBox = false;
            this.formControl1.ShowMinAmtion = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Name = "label4";
            // 
            // btn_start
            // 
            this.btn_start.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_start.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btn_start.FlatAppearance.BorderSize = 2;
            this.btn_start.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_start.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.btn_start, "btn_start");
            this.btn_start.Image = global::PSXDownloadHelper.Properties.Resources.play;
            this.btn_start.Name = "btn_start";
            this.btn_start.UseVisualStyleBackColor = false;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // cb_ip
            // 
            resources.ApplyResources(this.cb_ip, "cb_ip");
            this.cb_ip.FormattingEnabled = true;
            this.cb_ip.Name = "cb_ip";
            // 
            // pic_logo
            // 
            this.pic_logo.BackColor = System.Drawing.Color.Transparent;
            this.pic_logo.Image = global::PSXDownloadHelper.Properties.Resources.AppIco;
            resources.ApplyResources(this.pic_logo, "pic_logo");
            this.pic_logo.Name = "pic_logo";
            this.pic_logo.TabStop = false;
            // 
            // ckb_pcp
            // 
            resources.ApplyResources(this.ckb_pcp, "ckb_pcp");
            this.ckb_pcp.Checked = true;
            this.ckb_pcp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckb_pcp.Name = "ckb_pcp";
            this.ckb_pcp.UseVisualStyleBackColor = true;
            // 
            // tb_port
            // 
            resources.ApplyResources(this.tb_port, "tb_port");
            this.tb_port.Name = "tb_port";
            this.tb_port.TextChanged += new System.EventHandler(this.tb_port_TextChanged);
            // 
            // ckb_loaclspeed
            // 
            resources.ApplyResources(this.ckb_loaclspeed, "ckb_loaclspeed");
            this.ckb_loaclspeed.Checked = true;
            this.ckb_loaclspeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckb_loaclspeed.Name = "ckb_loaclspeed";
            this.ckb_loaclspeed.UseVisualStyleBackColor = true;
            // 
            // pic_ap
            // 
            this.pic_ap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pic_ap.Image = global::PSXDownloadHelper.Properties.Resources.wifi;
            resources.ApplyResources(this.pic_ap, "pic_ap");
            this.pic_ap.Name = "pic_ap";
            this.pic_ap.TabStop = false;
            this.pic_ap.Click += new System.EventHandler(this.pic_ap_Click);
            this.pic_ap.MouseLeave += new System.EventHandler(this.pic_ap_MouseLeave);
            this.pic_ap.MouseHover += new System.EventHandler(this.pic_ap_MouseHover);
            // 
            // cb_conntype
            // 
            this.cb_conntype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cb_conntype, "cb_conntype");
            this.cb_conntype.FormattingEnabled = true;
            this.cb_conntype.Items.AddRange(new object[] {
            resources.GetString("cb_conntype.Items"),
            resources.GetString("cb_conntype.Items1")});
            this.cb_conntype.Name = "cb_conntype";
            // 
            // ckb_cdn
            // 
            resources.ApplyResources(this.ckb_cdn, "ckb_cdn");
            this.ckb_cdn.Name = "ckb_cdn";
            this.ckb_cdn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // ckb_lixianspeed
            // 
            resources.ApplyResources(this.ckb_lixianspeed, "ckb_lixianspeed");
            this.ckb_lixianspeed.Name = "ckb_lixianspeed";
            this.ckb_lixianspeed.UseVisualStyleBackColor = true;
            // 
            // server_notify
            // 
            this.server_notify.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.server_notify, "server_notify");
            this.server_notify.ContextMenuStrip = this.Menu_Notify;
            this.server_notify.MouseClick += new System.Windows.Forms.MouseEventHandler(this.server_notify_MouseClick);
            this.server_notify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.server_notify_MouseDoubleClick);
            // 
            // Menu_Notify
            // 
            this.Menu_Notify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenServerToolStripMenuItem,
            this.OpenLogToolStripMenuItem,
            this.toolStripSeparator1,
            this.ExitToolStripMenuItem});
            this.Menu_Notify.Name = "Menu_Notify";
            resources.ApplyResources(this.Menu_Notify, "Menu_Notify");
            // 
            // OpenServerToolStripMenuItem
            // 
            this.OpenServerToolStripMenuItem.Name = "OpenServerToolStripMenuItem";
            resources.ApplyResources(this.OpenServerToolStripMenuItem, "OpenServerToolStripMenuItem");
            this.OpenServerToolStripMenuItem.Click += new System.EventHandler(this.OpenServerToolStripMenuItem_Click);
            // 
            // OpenLogToolStripMenuItem
            // 
            this.OpenLogToolStripMenuItem.Name = "OpenLogToolStripMenuItem";
            resources.ApplyResources(this.OpenLogToolStripMenuItem, "OpenLogToolStripMenuItem");
            this.OpenLogToolStripMenuItem.Click += new System.EventHandler(this.OpenLogToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            resources.ApplyResources(this.ExitToolStripMenuItem, "ExitToolStripMenuItem");
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // ServerConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "ServerConfig";
            this.Opacity = 0.97D;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerConfig_FormClosing);
            this.Load += new System.EventHandler(this.IPConfig_Load);
            this.SizeChanged += new System.EventHandler(this.ServerConfig_SizeChanged);
            this.mainPanel1.ResumeLayout(false);
            this.mainPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ap)).EndInit();
            this.Menu_Notify.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tb_port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ckb_pcp;
        private System.Windows.Forms.CheckBox ckb_lixianspeed;
        private System.Windows.Forms.CheckBox ckb_loaclspeed;
        private System.Windows.Forms.ComboBox cb_ip;
        public MetroStyle.MainPanel mainPanel1;
        private MetroStyle.MetroButton btn_start;
        private System.Windows.Forms.PictureBox pic_logo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pic_ap;
        private MetroStyle.FormControl formControl1;
        private MetroStyle.AlertLabel alertLabel1;
        private System.Windows.Forms.NotifyIcon server_notify;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cb_conntype;
        private System.Windows.Forms.CheckBox ckb_cdn;
        private System.Windows.Forms.ContextMenuStrip Menu_Notify;
        private System.Windows.Forms.ToolStripMenuItem OpenServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

