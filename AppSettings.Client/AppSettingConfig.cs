using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using AppSettings.Client.Utility;

namespace AppSettings.Client
{
    public static class AppSettingConfig
    {
        /// <summary>
        /// XElement缓存Key
        /// </summary>
        public const string XElementCacheKey = "XELEMENT_CACHE_KEY";
        /// <summary>
        /// 配置文件路径
        /// </summary>
        internal static string AppSettingsPath = AppDomain.CurrentDomain.BaseDirectory + "\\appsettings.xml";
        /// <summary>
        /// 是否缓存配置信息
        /// </summary>
        public static bool IsCacheConfig = false;
        /// <summary>
        /// 是否远程文件
        /// </summary>
        public static bool IsRemoteFile = false;
        /// <summary>
        /// 是否扫描远程文件
        /// </summary>
        public static bool IsScanFile = true;
        /// <summary>
        /// 扫描间隔
        /// </summary>
        public static int ScanInterval = 60 * 5;

        static AppSettingConfig()
        {
            Initialize();
        }

        private static void Initialize()
        {
            AppSettingsPath = ConfigurationManager.AppSettings["AppSettingsPath"];
            if (string.IsNullOrEmpty(AppSettingsPath))
            {
                throw new ConfigurationErrorsException("自定义配置文件配置AppSettingsPath未找到");
            }
            if (AppSettingsPath.StartsWith("~"))
            {
                AppSettingsPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + AppSettingsPath.TrimStart('~').TrimStart('\\');
            }

            if (File.Exists(AppSettingsPath))
            {
                return;
            }
            if (Utils.CheckUri(AppSettingsPath))
            {
                IsRemoteFile = true;
                return;
            }

            throw new ConfigurationErrorsException("自定义配置文件不存在");
        }
    }
}
