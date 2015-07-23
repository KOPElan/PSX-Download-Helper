using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using PSXDH.Model;

namespace PSXDH.DAL
{
    public class DataHistory
    {
        private static DataHistory _instance;
        private static readonly object Lock = new object();
        private static XElement _datas;

        private static readonly string Xmlpath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                                                 @"\DataFiles\DataHistory.xml";

        public static DataHistory Instance()
        {
            if (_instance == null)
                lock (Lock)
                {
                    _instance = new DataHistory();
                    if (!File.Exists(Xmlpath))
                    {
                        CreatXml();
                        return _instance;
                    }
                    _datas = XElement.Load(Xmlpath);
                }

            return _instance;
        }

        /// <summary>
        ///     创建xml文件
        /// </summary>
        /// <returns></returns>
        private static void CreatXml()
        {
            try
            {
                _datas = new XElement("PsnRecords", "");
                _datas.Save(Xmlpath);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     增加一条记录
        /// </summary>
        /// <param name="urlinfo"></param>
        /// <returns></returns>
        public bool AddLog(UrlInfo urlinfo)
        {
            if (!UpdataLog(urlinfo))
            {
                var psnrecord = new XElement("PsnRecord",
                                             new XElement("Names", urlinfo.MarkTxt),
                                             new XElement("PsnUrl", urlinfo.PsnUrl),
                                             new XElement("LocalUrl", urlinfo.ReplacePath),
                                             new XElement("isLixian", urlinfo.IsLixian),
                                             new XElement("LixianUrl", urlinfo.LixianUrl)
                    );
                _datas.Add(psnrecord);
                _datas.Save(Xmlpath);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     更新一条记录
        /// </summary>
        /// <param name="urlinfo"></param>
        /// <returns></returns>
        public bool UpdataLog(UrlInfo urlinfo)
        {
            IEnumerable<XElement> log = (from el in _datas.Elements("PsnRecord")
                                         let xElement = el.Element("PsnUrl")
                                         where xElement != null && xElement.Value == urlinfo.PsnUrl
                                         select el);
            if (log.FirstOrDefault() != null)
            {
                XElement xe = log.FirstOrDefault();
                xe.SetElementValue("Names", urlinfo.MarkTxt);
                xe.SetElementValue("LocalUrl", urlinfo.ReplacePath);
                xe.SetElementValue("isLixian", urlinfo.IsLixian);
                xe.SetElementValue("LixianUrl", urlinfo.LixianUrl);
                _datas.Save(Xmlpath);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     返回已经有记录的信息
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public UrlInfo GetInfo(string psnurl)
        {
            if (String.IsNullOrEmpty(psnurl))
                return null;

            psnurl = psnurl.ToLower();
            XElement log = null;
            if (!AppConfig.Instance().IsDimrule)
            {
                log = (from el in _datas.Elements("PsnRecord")
                       let xElement = el.Element("PsnUrl")
                       where xElement != null && xElement.Value.ToLower() == psnurl
                       select el).FirstOrDefault();
            }
            else  //模糊匹配
            {
                if (string.IsNullOrEmpty(AppConfig.Instance().Rule))
                    return null;

                List<string> rules = AppConfig.Instance().Rule.ToLower().Split('|').ToList();

                string temp = (rules.Where(r => psnurl.IndexOf(r.Replace("*", "")) > 0)).FirstOrDefault();
                if (String.IsNullOrEmpty(temp))
                    return null;

                string tempurl = psnurl.Substring(0,
                                                  psnurl.IndexOf(temp.Replace("*", "")) +
                                                  temp.Replace("*", "").Length);
                log = (_datas.Elements("PsnRecord")
                             .Select(el => new { el, xElement = el.Element("PsnUrl") })
                             .Where(@t => @t.xElement != null && @t.xElement.Value.ToLower().IndexOf(tempurl) >= 0)
                             .Select(@t => @t.el)).FirstOrDefault();

            }
            if (log == null)
                return null;

            var ui = new UrlInfo
                {
                    PsnUrl = log.Element("PsnUrl").Value,
                    ReplacePath = log.Element("LocalUrl").Value,
                    MarkTxt = log.Element("Names").Value,
                    IsLixian = bool.Parse(log.Element("isLixian").Value),
                    LixianUrl = log.Element("LixianUrl").Value
                };
            return ui;
        }

        /// <summary>
        ///     获取全部历史记录
        /// </summary>
        /// <returns></returns>
        public List<UrlInfo> GetAllUrl()
        {
            List<UrlInfo> log = (from el in _datas.Elements("PsnRecord")
                                 select
                                     new UrlInfo(el.Element("PsnUrl").Value, el.Element("LocalUrl").Value,
                                                 el.Element("Names").Value, bool.Parse(el.Element("isLixian").Value),
                                                 el.Element("LixianUrl").Value)).ToList();
            return log;
        }

        /// <summary>
        ///     删除一条记录
        /// </summary>
        /// <param name="urlinfo"></param>
        /// <returns></returns>
        public bool DelLog(UrlInfo urlinfo)
        {
            try
            {
                XElement log = (from el in _datas.Elements("PsnRecord")
                                let xElement = el.Element("PsnUrl")
                                where xElement != null && xElement.Value == urlinfo.PsnUrl
                                select el).FirstOrDefault();
                if (log != null) log.Remove();
                _datas.Save(Xmlpath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}