using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using PSXDH.BLL;
using PSXDH.Model;

namespace PSXDownloadHelper
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Code.SettingHelper.InitSettings();
            var uiCulture = new CultureInfo(AppConfig.Instance().Language);
            Application.ThreadException += OnThreadException;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            Application.Run(ServerConfig.ServerInstance());
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            try
            {
                var errorMsg = "程序运行过程中发生错误，错误信息如下:\n";
                errorMsg += args.Exception.Message;
                errorMsg += "\n发生错误的程序集为:";
                errorMsg += args.Exception.Source;
                errorMsg += "\n发生错误的具体位置为:\n";
                errorMsg += args.Exception.StackTrace;
                errorMsg += "\n\n 请抓取此错误屏幕，至项目主页进行反馈，感谢！";
                MessageBox.Show(errorMsg, @"运行时错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(@"系统运行时发生严重错误!", @"严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}