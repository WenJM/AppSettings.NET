using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Xml.Linq;
using AppSettings.Client.Scan;
using AppSettings.Client.Helper;
using AppSettings.Client.Extensions;
using AppSettings.Client.Utility;

namespace AppSettings.Client.AppSettings
{
    internal abstract class AppSettingsBase
    {
        protected abstract string Key { get;}

        protected abstract TValue LoadConfigFromFile<TValue>(string parentFull) where TValue : class;

        protected IEnumerable<XElement> AppSettingElement(string parentFull)
        {
            //读取XML文件
            var elements = XmlHelper.TryGetXElementOfCache(AppSettingConfig.XElementCacheKey, AppSettingConfig.AppSettingsPath, AppSettingConfig.IsRemoteFile);
            if (!string.IsNullOrEmpty(parentFull))
            {
                parentFull.Split('.','\\','/').ToList().ForEach(x =>
                {
                    elements = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(x)).Elements();
                });
            }

            //启动远程文件扫描
            if (AppSettingConfig.IsRemoteFile && AppSettingConfig.IsScanFile)
            {
                ScanWoker.Current.Start();
            }

            return elements;
        }

        public TValue LoadConfig<TValue>(string parentFull) where TValue : class
        {
            try
            {
                var settings = LoadConfigFromFile<TValue>(parentFull);
                if (AppSettingConfig.IsCacheConfig && settings != null)
                {
                    if (AppSettingConfig.IsRemoteFile)
                        CacheHelper.Set(Key, settings, new List<string> { AppSettingConfig.XElementCacheKey });
                    else
                        CacheHelper.Set(Key, settings, CacheHelper.CreateMonitor(AppSettingConfig.AppSettingsPath));
                }
                return settings;
            }
            catch (System.Exception ex)
            {
                throw new ConfigurationErrorsException(ex.Message);
            }
        }
    }
}
