using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PSXDH.Model;
using System.Text.RegularExpressions;

namespace PSXDH.BLL
{
    public class MonitorLog
    {
        /// <summary>
        /// 语言
        /// </summary>
        public static readonly Dictionary<int, string> Languages = new Dictionary<int, string>
            {
                {0,"zh-CHS"},
                {1,"zh-CHT"},
                {2,"pt-BR"},
                {3,"en"}
            };

        /// <summary>
        /// 主题颜色
        /// </summary>
        public static readonly List<string> Themes = new List<string>
            {
                "#46B9F0",
                "#E98EA8",
                "#F2886E",
                "#F4AA6E",
                "#89C97D",
                "#66C0B8",
                "#BB9EC3",
                "#6AC9E0",
                "#F65314",
                "#7CBB00",
                "#00A1F1",
                "#FFBB00"
            };

        /// <summary>
        /// 匹配链接
        /// </summary>
        /// <returns></returns>
        public static bool RegexUrl(string urls)
        {
            var rules = AppConfig.Instance().Rule.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (rules.Length <= 0 || String.IsNullOrEmpty(urls))
                return false;

            return
                rules.Select(rule => new Regex(rule.ToLower().Replace(".", @"\.").Replace("*", ".*?")))
                     .Any(regex => regex.Match(urls.ToLower()).Success);
        }

        private static string updatelog = "";
        private static int? version;

        /// <summary>
        /// 获取最新版本号
        /// </summary>
        /// <returns></returns>
        public static int GetNewVersion()
        {
            /*
             * 版本更新检查，如果提供此服务请自行补充。
             * 下面代码为原版检查更新代码
             */
            return 0;

            if (version.HasValue)
                return version.Value;

            const string url = "http://blog.acgpedia.com/extensions/service/update.txt";
            var strValue = GetWebContent(url);
            var regexversion = new Regex(@"#Version:(.*?)#PSX Download Helper", RegexOptions.Singleline);
            var newversion = regexversion.Match(strValue).Groups[1].Value.Replace(".", "");
            version = int.Parse(newversion);
            return version.Value;
        }

        /// <summary>
        /// 获取检查更新版本信息
        /// </summary>
        /// <returns></returns>
        public static string GetUpdateLog()
        {
            if (!String.IsNullOrEmpty(updatelog))
                return updatelog;

            const string strUrl = "http://blog.acgpedia.com/extensions/service/update.txt";
            var strValue = GetWebContent(strUrl);
            var regupdate = new Regex(@"<div id=""newversion"">.*?</div>", RegexOptions.Singleline);
            updatelog = regupdate.Match(strValue).Value;
            return updatelog;
        }

        /// <summary>
        /// 更新cdn列表
        /// </summary>
        /// <returns></returns>
        public static bool UpdateCdn()
        {
            try
            {
                var cdnlist = GetWebContent("http://blog.acgpedia.com/extensions/service/cdnhosts.txt");
                var sw = new StreamWriter(@"Hosts\CdnHosts.ini",false);
                sw.Write(cdnlist);
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取网页内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetWebContent(string url)
        {
            var wc = new WebClient { Credentials = CredentialCache.DefaultCredentials };
            var enc = Encoding.GetEncoding("UTF-8");
            var pageData = wc.DownloadData(url); // 从资源下载数据并返回字节数组。
            var strValue = enc.GetString(pageData);
            return strValue;
        }
    }
}
