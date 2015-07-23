using System.Collections;
using PSXDH.Model;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace PSXDH.DAL
{
    public class HashUrl
    {
        public static Hashtable UrlList;
        private static HashUrl _instance;
        private static readonly object Lock = new object();

        public static HashUrl Instance()
        {
            if (_instance == null)
                lock (Lock)
                {
                    _instance = new HashUrl();
                    if (UrlList == null)
                        UrlList = new Hashtable();
                }
            return _instance;
        }

        /// <summary>
        ///     增加psnurl与本地文件对应
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public bool PushUrl(UrlInfo ui)
        {
            try
            {
                if (UrlList.ContainsKey(ui.PsnUrl))
                    UrlList[ui.PsnUrl] = ui.ReplacePath;
                else
                    UrlList.Add(ui.PsnUrl, ui.ReplacePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取存在替换连接的本地路径，不存在返回空
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public string PsnLocalPath(string psnurl)
        {
            try
            {
                UrlInfo temp = DataHistory.Instance().GetInfo(psnurl);
                if (temp != null && !String.IsNullOrEmpty(temp.ReplacePath))
                    return temp.ReplacePath;

                if (!AppConfig.Instance().IsAutoFindFile)
                    return String.Empty;

                //如果开启自动匹配本地文件，则自动查找
                string filename = GetUrlFileName(psnurl);
                if (!String.IsNullOrEmpty(filename))
                    return AppConfig.Instance().LocalFileDirectory + "\\" + filename;

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取url中的文件名
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public string GetUrlFileName(string psnurl)
        {
            var rules = AppConfig.Instance().Rule.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            if (rules.Count <= 0)
                return string.Empty;

            rules = rules.Select(r => r.Replace("*", "")).ToList();
            string filename = string.Empty;
            rules.ForEach(r =>
            {
                if (psnurl.IndexOf(r) > 0)
                {
                    filename = psnurl.Substring(0, psnurl.IndexOf(r) + r.Length);
                    filename = filename.Substring(filename.LastIndexOf("/") + 1);
                }
            });

            return filename;
        }
    }
}