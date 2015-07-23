using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXDH.DAL;
using PSXDH.Model;

namespace PSXDH.BLL
{
    public class DataHistoryOperate
    {
        private static readonly DataHistory DataHistoryInstance = DataHistory.Instance();

        /// <summary>
        ///     增加一条记录
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <returns></returns>
        public static bool AddLog(UrlInfo urlInfo)
        {
            return DataHistoryInstance.AddLog(urlInfo);
        }

        /// <summary>
        ///     更新一条记录
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <returns></returns>
        public static bool UpdataLog(UrlInfo urlInfo)
        {
            return DataHistoryInstance.UpdataLog(urlInfo);
        }

        /// <summary>
        ///     返回已经有记录的信息
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public static UrlInfo GetInfo(string psnurl)
        {
            return DataHistoryInstance.GetInfo(psnurl);
        }

        /// <summary>
        ///     获取全部历史记录
        /// </summary>
        /// <returns></returns>
        public static List<UrlInfo> GetAllUrl()
        {
            return DataHistoryInstance.GetAllUrl();
        }

        /// <summary>
        ///     删除一条记录
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <returns></returns>
        public static bool DelLog(UrlInfo urlInfo)
        {
            return DataHistoryInstance.DelLog(urlInfo);
        }
    }
}
