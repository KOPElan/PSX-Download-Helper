using System.Collections;
using System.Collections.Specialized;
using PSXDH.Model;

namespace PSXDH.DAL
{
    public class LixianUrl
    {
        public static Hashtable HashLixian;
        private static LixianUrl _instance;
        private static readonly object Lock = new object();

        public static LixianUrl Instance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance = new LixianUrl();
                    HashLixian = new Hashtable();
                }
            }
            return _instance;
        }

        /// <summary>
        ///     存入离线地址
        /// </summary>
        /// <param name="psnurl"></param>
        /// <param name="lixianurl"></param>
        /// <returns></returns>
        public bool PushLixianUrl(string psnurl, string lixianurl)
        {
            if (!HashLixian.ContainsKey(psnurl))
                HashLixian.Add(psnurl, lixianurl);
            else
                HashLixian[psnurl] = lixianurl;
            return true;
        }

        /// <summary>
        ///     获取存在的离线路径
        /// </summary>
        /// <param name="psnurl"></param>
        /// <param name="lixianurl"></param>
        /// <returns></returns>
        public bool GetLixianUrl(string psnurl, out string lixianurl)
        {
            lixianurl = null;
            try
            {
                UrlInfo temp = DataHistory.Instance().GetInfo(psnurl);
                if (temp != null)
                {
                    lixianurl = temp.LixianUrl;
                    return temp.IsLixian;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ui"></param>
        /// <returns></returns>
        public string GetQuery(string query, ref UrlInfo ui)
        {
            //Query = Query.Replace("http://ares.dl.playstation.net/cdn/HP9000/PCSD00006_00/GCFOcGEdYLwirsTIGTvskAPlIJFcrRvmphUaEasTrBHmAMzGFCdPzobaUQrlehFiHMlJGpuLyCmOeBIFfqqIsJilCpNGOWSTZMKmw.pkg?country=hk", "http://xflxsrc.store.qq.com:443/ftn_handler/1450b6a548476607552337ccf0d566504dc34c07cb7ef869224b6602b42806e8e85643564d9fb8d2092253f7bbea4254bf942727c59cf9d386b2097d95ff8b21/GCFOcGEdYLwirsTIGTvskAPlIJFcrRvmphUaEasTrBHmAMzGFCdPzobaUQrlehFiHMlJGpuLyCmOeBIFfqqIsJilCpNGOWSTZMKmw.pkg").Replace("ares.dl.playstation.net", "xflxsrc.store.qq.com:443");
            //Query += "\r\nCookie: pgv_pvid=4433548656; pgv_info=ssid=s4095335791&pgvReferrer=; pt2gguin=o0332126295; uin=o0332126295; skey=@8xaR4bH8j; ptisp=cnc; RK=pizv1LbmGC; FTN5K=08a93f35";

            var queryfield = new StringDictionary();
            string url = "";
            string[] strArray = query.Replace("\r\n", "\n").Split(new[] { '\n' });
            int index = strArray[0].IndexOf(' ');
            if (index > 0)
            {
                strArray[0] = strArray[0].Substring(index).Trim();
            }
            index = strArray[0].LastIndexOf(' ');
            if (index > 0)
            {
                url = strArray[0].Substring(0, index);
            }
            for (int i = 1; i < strArray.Length; i++)
            {
                index = strArray[i].IndexOf(":");
                if ((index > 0) && (index < (strArray[i].Length - 1)))
                {
                    try
                    {
                        queryfield.Add(strArray[i].Substring(0, index), strArray[i].Substring(index + 1).Trim());
                    }
                    catch
                    {
                    }
                }
            }
            string templx;
            if (GetLixianUrl(url, out templx) && !string.IsNullOrEmpty(templx))
            {
                query = query.Replace(url, templx);
                query = query.Replace(queryfield["Host"], AppConfig.Instance().Host);
                if (!string.IsNullOrEmpty(AppConfig.Instance().Cookie))
                    query += "\nCookie:" + AppConfig.Instance().Cookie;
                ui.IsLixian = true;
                ui.LixianUrl = templx;
                ui.PsnUrl = url;
            }
            return query;
        }
    }
}