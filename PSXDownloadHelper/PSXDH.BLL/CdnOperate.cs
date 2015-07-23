using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using PSXDH.DAL;

namespace PSXDH.BLL
{
    public class CdnOperate
    {
        private static readonly CdnHost CdnInstance = CdnHost.Instance();

        /// <summary>
        /// 读取CDN链接
        /// </summary>
        public static void ReadCdnConfig()
        {
            CdnInstance.ReadCdnConfig();
        }

        /// <summary>
        /// 获取DNS解析，如果有CDN主机则返回主机地址，否则返回默认解析
        /// </summary>
        /// <param name="host"></param>
        /// <param name="isCdn"></param>
        /// <returns></returns>
        public static IPAddress GetCdnAddress(string host,out bool isCdn)
        {
            return CdnInstance.GetCdnAddress(host,out isCdn);
        }

        /// <summary>
        /// 导出最终的CDN列表
        /// </summary>
        /// <returns></returns>
        public static bool ExportHost()
        {
            return CdnInstance.ExportHost();
        }
    }
}
