using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSXDownloadHelper
{
    public partial class Featuring : Form
    {
        public Featuring()
        {
            InitializeComponent();
        }

        private void Featuring_Load(object sender, EventArgs e)
        {
            using (var sr = new StreamReader(@"Extensions\Featuring.html"))
            {
                var content = sr.ReadToEnd();
                webBrowser1.DocumentText = content;
            }
        }
    }
}
