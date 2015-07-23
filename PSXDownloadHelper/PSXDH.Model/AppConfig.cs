using System;
using System.Collections.Generic;

namespace PSXDH.Model
{
    public class AppConfig
    {
        private static AppConfig _instance;
        private static readonly object Lock = new object();
        public static AppConfig Instance()
        {
            if (_instance == null)
                lock (Lock)
                {
                    _instance = new AppConfig();
                }

            return _instance;
        }

        public string Language { get; set; }
        /// <summary>
        ///     匹配规则
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        ///     主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     登陆信息
        /// </summary>
        public string Cookie { get; set; }

        private static bool _enablelixian;
        /// <summary>
        ///     是否启用离线
        /// </summary>
        public bool EnableLixian
        {
            get { return _enablelixian; }
            set { _enablelixian = value; }
        }

        /// <summary>
        ///     连接模式
        /// </summary>
        public int ConnType { get; set; }

        public bool IsUsePcProxy { get; set; }

        public bool IsUseLixian { get; set; }

        public bool IsUserLocal { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// 是否启用CDN加速功能
        /// </summary>
        public bool IsUseCdn { get; set; }

        /// <summary>
        /// 最后检查CDN主机列表的时间
        /// </summary>
        public DateTime LastCheckCdn { get; set; }

        public bool IsAutoCheckUpdate { get; set; }

        public DateTime LastCheckUpdate { get; set; }

        public string Ssid { get; set; }

        public string Theme { get; set; }

        public string ApPassword { get; set; }

        /// <summary>
        ///     是否模糊匹配
        /// </summary>
        public bool IsDimrule { get; set; }

        /// <summary>
        /// 是否使用自定义主机地址
        /// </summary>
        public bool IsUseCustomeHosts { get; set; }

        /// <summary>
        /// 是否自动寻找本地文件替换
        /// </summary>
        public bool IsAutoFindFile { get; set; }

        /// <summary>
        /// 本地替换目录
        /// </summary>
        public string LocalFileDirectory { get; set; }
    }
}