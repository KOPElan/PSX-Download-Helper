using System.Text;
using System.Windows.Forms;

namespace PSXDownloadHelper
{
    public partial class CheckUpdate : Form
    {
        public CheckUpdate(string versioninfo)
        {
            InitializeComponent();
            var sb = new StringBuilder("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<title>PSX Download Helper更新日志</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{margin:0;padding:20px;line-height:25px;border-top:4px #00b5ad solid;background-color:#fff;color:#333;font-size:.85em;font-family:\"Segoe UI\",Verdana,Helvetica,Sans-Serif}");
            sb.AppendLine("p{margin-bottom:10px;}");
            sb.AppendLine("a{border-bottom: 1px #096ec4 solid;text-decoration: none;}");
            sb.AppendLine("a:link,a:visited,a:active{color: #096ec4;padding-bottom:2px;text-decoration: none;border-bottom: 1px #096ec4 solid;}");            
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>{0}</body>");
            sb.AppendLine("</html>");
            wb_checkupdate.DocumentText = sb.ToString().Replace("{0}", versioninfo);
        }
    }
}