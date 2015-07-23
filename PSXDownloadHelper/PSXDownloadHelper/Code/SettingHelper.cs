using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXDH.BLL;
using PSXDH.Model;

namespace PSXDownloadHelper.Code
{
    public class SettingHelper
    {
        private static AppSettings appsetting = new AppSettings();
        private static AppConfig appconfig = AppConfig.Instance();
        public static void InitSettings()
        {
            appconfig.Language = appsetting.Language;
            appconfig.Ip = appsetting.Ip;
            appconfig.Port = appsetting.Port;
            appconfig.Rule = appsetting.Rule;
            appconfig.Host = appsetting["host" + appsetting.lixianConfig].ToString();
            appconfig.Cookie = appsetting["cookie" + appsetting.lixianConfig].ToString();
            appconfig.EnableLixian = appsetting.EnableLixian;
            appconfig.ConnType = appsetting.ConnType;
            appconfig.IsUsePcProxy = appsetting.IsUsePcProxy;
            appconfig.IsUserLocal = appsetting.IsUserLocal;
            appconfig.IsUseLixian = appsetting.IsUseLixian;
            appconfig.IsUseCdn = appsetting.IsUseCdn;
            appconfig.LastCheckCdn = appsetting.LastCheckCdn;
            appconfig.IsAutoCheckUpdate = appsetting.IsAutoCheckUpdate;
            appconfig.LastCheckUpdate = appsetting.LastCheckUpdate;
            appconfig.IsDimrule = appsetting.IsDimrule;
            appconfig.IsUseCustomeHosts = appsetting.IsUseCustomeHosts;
            appconfig.Theme = appsetting.Theme;
            appconfig.Ssid = appsetting.Ssid;
            appconfig.ApPassword = appsetting.ApPassword;
            appconfig.IsAutoFindFile = appsetting.IsAutoFindFile;
            appconfig.LocalFileDirectory = appsetting.LocalFileDirectory;
        }

        public static void ChangeSetting(string settingname, object value)
        {
            appsetting[settingname] = value;
            appsetting.Save();
        }

    }
}
