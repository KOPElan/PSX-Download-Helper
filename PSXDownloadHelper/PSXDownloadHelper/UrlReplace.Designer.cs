namespace PSXDownloadHelper
{
    partial class UrlReplace
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UrlReplace));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_local = new System.Windows.Forms.TextBox();
            this.tb_psn = new System.Windows.Forms.TextBox();
            this.btn_replace = new System.Windows.Forms.Button();
            this.lb_time = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_mark = new System.Windows.Forms.TextBox();
            this.timer_main = new System.Windows.Forms.Timer(this.components);
            this.btn_del = new System.Windows.Forms.Button();
            this.tb_lx = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_enablelx = new System.Windows.Forms.Button();
            this.btn_copy = new System.Windows.Forms.Button();
            this.lb_copy = new MetroStyle.AlertLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pic_isCdn = new System.Windows.Forms.PictureBox();
            this.btn_ping = new System.Windows.Forms.Button();
            this.tb_Ping = new System.Windows.Forms.TextBox();
            this.lb_filename = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pic_isCdn)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tb_local
            // 
            resources.ApplyResources(this.tb_local, "tb_local");
            this.tb_local.Name = "tb_local";
            this.tb_local.ReadOnly = true;
            this.toolTip1.SetToolTip(this.tb_local, resources.GetString("tb_local.ToolTip"));
            // 
            // tb_psn
            // 
            resources.ApplyResources(this.tb_psn, "tb_psn");
            this.tb_psn.Name = "tb_psn";
            this.tb_psn.ReadOnly = true;
            this.toolTip1.SetToolTip(this.tb_psn, resources.GetString("tb_psn.ToolTip"));
            this.tb_psn.DoubleClick += new System.EventHandler(this.tb_psn_DoubleClick);
            this.tb_psn.MouseEnter += new System.EventHandler(this.tb_psn_MouseEnter);
            this.tb_psn.MouseLeave += new System.EventHandler(this.tb_psn_MouseLeave);
            // 
            // btn_replace
            // 
            resources.ApplyResources(this.btn_replace, "btn_replace");
            this.btn_replace.Name = "btn_replace";
            this.btn_replace.UseVisualStyleBackColor = true;
            this.btn_replace.Click += new System.EventHandler(this.btn_replace_Click);
            // 
            // lb_time
            // 
            resources.ApplyResources(this.lb_time, "lb_time");
            this.lb_time.Name = "lb_time";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tb_mark
            // 
            resources.ApplyResources(this.tb_mark, "tb_mark");
            this.tb_mark.Name = "tb_mark";
            this.tb_mark.TextChanged += new System.EventHandler(this.tb_mark_TextChanged);
            // 
            // timer_main
            // 
            this.timer_main.Interval = 1000;
            this.timer_main.Tick += new System.EventHandler(this.timer_main_Tick);
            // 
            // btn_del
            // 
            resources.ApplyResources(this.btn_del, "btn_del");
            this.btn_del.Name = "btn_del";
            this.btn_del.UseVisualStyleBackColor = true;
            this.btn_del.Click += new System.EventHandler(this.btn_del_Click);
            // 
            // tb_lx
            // 
            resources.ApplyResources(this.tb_lx, "tb_lx");
            this.tb_lx.Name = "tb_lx";
            this.tb_lx.ReadOnly = true;
            this.tb_lx.TextChanged += new System.EventHandler(this.tb_lx_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btn_enablelx
            // 
            resources.ApplyResources(this.btn_enablelx, "btn_enablelx");
            this.btn_enablelx.Name = "btn_enablelx";
            this.btn_enablelx.UseVisualStyleBackColor = true;
            this.btn_enablelx.Click += new System.EventHandler(this.btn_enablelx_Click);
            // 
            // btn_copy
            // 
            resources.ApplyResources(this.btn_copy, "btn_copy");
            this.btn_copy.Name = "btn_copy";
            this.btn_copy.UseVisualStyleBackColor = true;
            this.btn_copy.Click += new System.EventHandler(this.tb_psn_DoubleClick);
            // 
            // lb_copy
            // 
            resources.ApplyResources(this.lb_copy, "lb_copy");
            this.lb_copy.BackColor = System.Drawing.Color.Orange;
            this.lb_copy.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lb_copy.left = 0;
            this.lb_copy.MainForm = null;
            this.lb_copy.Name = "lb_copy";
            this.lb_copy.ShowInMiddle = false;
            this.lb_copy.TimeShow = 1000;
            this.lb_copy.top = 0;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // pic_isCdn
            // 
            this.pic_isCdn.Image = global::PSXDownloadHelper.Properties.Resources.cdnstatus;
            resources.ApplyResources(this.pic_isCdn, "pic_isCdn");
            this.pic_isCdn.Name = "pic_isCdn";
            this.pic_isCdn.TabStop = false;
            this.toolTip1.SetToolTip(this.pic_isCdn, resources.GetString("pic_isCdn.ToolTip"));
            // 
            // btn_ping
            // 
            resources.ApplyResources(this.btn_ping, "btn_ping");
            this.btn_ping.Name = "btn_ping";
            this.btn_ping.UseVisualStyleBackColor = true;
            this.btn_ping.Click += new System.EventHandler(this.btn_ping_Click);
            // 
            // tb_Ping
            // 
            this.tb_Ping.BackColor = System.Drawing.Color.White;
            this.tb_Ping.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.tb_Ping, "tb_Ping");
            this.tb_Ping.Name = "tb_Ping";
            this.tb_Ping.ReadOnly = true;
            // 
            // lb_filename
            // 
            resources.ApplyResources(this.lb_filename, "lb_filename");
            this.lb_filename.BackColor = System.Drawing.Color.DarkOrange;
            this.lb_filename.ForeColor = System.Drawing.Color.White;
            this.lb_filename.Name = "lb_filename";
            // 
            // UrlReplace
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lb_filename);
            this.Controls.Add(this.tb_Ping);
            this.Controls.Add(this.btn_ping);
            this.Controls.Add(this.lb_copy);
            this.Controls.Add(this.btn_copy);
            this.Controls.Add(this.btn_enablelx);
            this.Controls.Add(this.tb_lx);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_del);
            this.Controls.Add(this.tb_mark);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lb_time);
            this.Controls.Add(this.btn_replace);
            this.Controls.Add(this.tb_psn);
            this.Controls.Add(this.tb_local);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pic_isCdn);
            this.DoubleBuffered = true;
            this.Name = "UrlReplace";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.UrlReplace_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.UrlReplace_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pic_isCdn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_local;
        private System.Windows.Forms.TextBox tb_psn;
        private System.Windows.Forms.Button btn_replace;
        private System.Windows.Forms.Label lb_time;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_mark;
        private System.Windows.Forms.Timer timer_main;
        private System.Windows.Forms.Button btn_del;
        private System.Windows.Forms.TextBox tb_lx;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_enablelx;
        private System.Windows.Forms.Button btn_copy;
        private MetroStyle.AlertLabel lb_copy;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pic_isCdn;
        private System.Windows.Forms.Button btn_ping;
        private System.Windows.Forms.TextBox tb_Ping;
        private System.Windows.Forms.Label lb_filename;
    }
}
