namespace PSXDownloadHelper
{
    partial class CheckUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckUpdate));
            this.wb_checkupdate = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wb_checkupdate
            // 
            this.wb_checkupdate.AllowNavigation = false;
            this.wb_checkupdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wb_checkupdate.Location = new System.Drawing.Point(0, 0);
            this.wb_checkupdate.MinimumSize = new System.Drawing.Size(20, 20);
            this.wb_checkupdate.Name = "wb_checkupdate";
            this.wb_checkupdate.Size = new System.Drawing.Size(406, 394);
            this.wb_checkupdate.TabIndex = 0;
            // 
            // CheckUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 394);
            this.Controls.Add(this.wb_checkupdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CheckUpdate";
            this.Text = "检查更新";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wb_checkupdate;

    }
}