namespace PSXDH.Model
{
    public class UrlInfo
    {
        public UrlInfo()
        {
            SetLixian = false;
            IsLixian = false;
        }

        public UrlInfo(string psnurl, string replacepath, string marktxt, bool isLixian = false, string lixianurl = null)
        {
            SetLixian = false;
            PsnUrl = psnurl;
            ReplacePath = replacepath;
            MarkTxt = marktxt;
            LixianUrl = lixianurl;
            IsLixian = isLixian;
        }

        /// <summary>
        ///     PSN连接
        /// </summary>
        public string PsnUrl { get; set; }

        /// <summary>
        ///     替换地址
        /// </summary>
        public string ReplacePath { get; set; }

        /// <summary>
        ///     当前连接备注信息
        /// </summary>
        public string MarkTxt { get; set; }

        /// <summary>
        ///     离线地址
        /// </summary>
        public string LixianUrl { get; set; }

        /// <summary>
        ///     是否是线
        /// </summary>
        public bool IsLixian { get; set; }

        /// <summary>
        ///     增加为离线
        /// </summary>
        public bool SetLixian { get; set; }
        
        /// <summary>
        /// 是否为CDN地址
        /// </summary>
        public bool IsCdn { get; set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; }
    }
}