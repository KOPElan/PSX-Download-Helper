namespace PSXDownloadHelper
{
    partial class WifiAp
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WifiAp));
            this.mainPanel1 = new MetroStyle.MainPanel();
            this.alertLabel1 = new MetroStyle.AlertLabel();
            this.aphelp = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_result = new System.Windows.Forms.Label();
            this.btn_start = new MetroStyle.MetroButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_ap = new MetroStyle.MetroButton();
            this.tb_pwd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_ssid = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.formControl1 = new MetroStyle.FormControl();
            this.label3 = new System.Windows.Forms.Label();
            this.mainPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aphelp)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel1
            // 
            this.mainPanel1.BorderAlpha = 100;
            this.mainPanel1.BorderColor = System.Drawing.SystemColors.HotTrack;
            this.mainPanel1.BorderSize = 1;
            this.mainPanel1.Controls.Add(this.alertLabel1);
            this.mainPanel1.Controls.Add(this.aphelp);
            this.mainPanel1.Controls.Add(this.panel1);
            this.mainPanel1.Controls.Add(this.btn_start);
            this.mainPanel1.Controls.Add(this.label4);
            this.mainPanel1.Controls.Add(this.btn_ap);
            this.mainPanel1.Controls.Add(this.tb_pwd);
            this.mainPanel1.Controls.Add(this.label2);
            this.mainPanel1.Controls.Add(this.tb_ssid);
            this.mainPanel1.Controls.Add(this.label1);
            this.mainPanel1.Controls.Add(this.pictureBox1);
            this.mainPanel1.Controls.Add(this.formControl1);
            this.mainPanel1.Controls.Add(this.label3);
            resources.ApplyResources(this.mainPanel1, "mainPanel1");
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
            this.alertLabel1.TimeShow = 12000;
            this.alertLabel1.top = 0;
            // 
            // aphelp
            // 
            this.aphelp.Image = global::PSXDownloadHelper.Properties.Resources.help;
            resources.ApplyResources(this.aphelp, "aphelp");
            this.aphelp.Name = "aphelp";
            this.aphelp.TabStop = false;
            this.aphelp.MouseLeave += new System.EventHandler(this.aphelp_MouseLeave);
            this.aphelp.MouseHover += new System.EventHandler(this.aphelp_MouseHover);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.lb_result);
            this.panel1.Name = "panel1";
            // 
            // lb_result
            // 
            resources.ApplyResources(this.lb_result, "lb_result");
            this.lb_result.BackColor = System.Drawing.SystemColors.HotTrack;
            this.lb_result.ForeColor = System.Drawing.SystemColors.Control;
            this.lb_result.Name = "lb_result";
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
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btn_ap
            // 
            this.btn_ap.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_ap.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btn_ap.FlatAppearance.BorderSize = 2;
            this.btn_ap.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_ap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.btn_ap, "btn_ap");
            this.btn_ap.Name = "btn_ap";
            this.btn_ap.UseVisualStyleBackColor = false;
            this.btn_ap.Click += new System.EventHandler(this.btn_ap_Click);
            // 
            // tb_pwd
            // 
            resources.ApplyResources(this.tb_pwd, "tb_pwd");
            this.tb_pwd.Name = "tb_pwd";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tb_ssid
            // 
            resources.ApplyResources(this.tb_ssid, "tb_ssid");
            this.tb_ssid.Name = "tb_ssid";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PSXDownloadHelper.Properties.Resources.AP;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // formControl1
            // 
            resources.ApplyResources(this.formControl1, "formControl1");
            this.formControl1.BackColor = System.Drawing.Color.Transparent;
            this.formControl1.MainForm = this;
            this.formControl1.MinAmtion = 0;
            this.formControl1.Name = "formControl1";
            this.formControl1.ShowMaxandMinBox = false;
            this.formControl1.ShowMaxBox = false;
            this.formControl1.ShowMinAmtion = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // WifiAp
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(185)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.mainPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WifiAp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WIFIAP_FormClosing);
            this.Load += new System.EventHandler(this.WIFIAP_Load);
            this.mainPanel1.ResumeLayout(false);
            this.mainPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aphelp)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroStyle.MainPanel mainPanel1;
        private MetroStyle.FormControl formControl1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_pwd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_ssid;
        private MetroStyle.MetroButton btn_ap;
        private System.Windows.Forms.Label label4;
        private MetroStyle.MetroButton btn_start;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox aphelp;
        private MetroStyle.AlertLabel alertLabel1;
        private System.Windows.Forms.Label lb_result;
    }
}