using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXDH.DAL;
using PSXDH.Model;

namespace PSXDH.BLL
{
    public class UrlOperate
    {
        private static readonly HashUrl HashurlOperate = HashUrl.Instance();
        /// <summary>
        /// 查找本地替换URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MatchFile(string url)
        {
            return HashurlOperate.PsnLocalPath(url);
        }

        /// <summary>
        ///     增加psnurl与本地文件对应
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static bool PushUrl(UrlInfo ui)
        {
            return HashurlOperate.PushUrl(ui);
        }

        /// <summary>
        ///     获取存在替换连接的本地路径，不存在返回空
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public static string PsnLocalPath(string psnurl)
        {
            return HashurlOperate.PsnLocalPath(psnurl);
        }

        /// <summary>
        /// 获取url中的文件名
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public static string GetUrlFileName(string psnurl)
        {
            return HashurlOperate.GetUrlFileName(psnurl);
        }


        public static LixianUrl LixianInstance = LixianUrl.Instance();

        /// <summary>
        ///     存入离线地址
        /// </summary>
        /// <param name="psnurl"></param>
        /// <param name="lixianurl"></param>
        /// <returns></returns>
        public static bool PushLixianUrl(string psnurl, string lixianurl)
        {
            return LixianInstance.PushLixianUrl(psnurl, lixianurl);
        }

        /// <summary>
        ///     获取存在的离线路径
        /// </summary>
        /// <param name="psnurl"></param>
        /// <param name="lixianurl"></param>
        /// <returns></returns>
        public static bool GetLixianUrl(string psnurl, out string lixianurl)
        {
            return LixianInstance.GetLixianUrl(psnurl, out lixianurl);
        }

        /// <summary>
        /// 获取离线请求文件头
        /// </summary>
        /// <param name="query"></param>
        /// <param name="uinfo"></param>
        /// <returns></returns>
        public static string GetQuery(string query, ref UrlInfo uinfo)
        {
            return LixianInstance.GetQuery(query, ref uinfo);
        }
    }
}
