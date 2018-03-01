using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Xml.Linq;
using AppSettings.Client.Helper;
using AppSettings.Client.Utility;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal abstract class AppSettingsBase
    {
        private const string XElementCacheKey = "XELEMENT_CACHE_KEY";

        private bool IsRemoteFile = false; //远程文件

        protected abstract string Key { get;}

        protected string AppSettingsPath
        {
            get
            {
                string filePath = ConfigurationManager.AppSettings["AppSettingsPath"];
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ConfigurationErrorsException("自定义配置文件配置AppSettingsPath未找到");
                }
                if (filePath.StartsWith("~"))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + filePath.TrimStart('~').TrimStart('\\');
                }
                if (File.Exists(filePath))
                {
                    return filePath;
                }
                if (Utils.CheckUri(filePath))
                {
                    IsRemoteFile = true;

                    return filePath;
                }
                throw new ConfigurationErrorsException("自定义配置文件不存在");
            }
        }

        protected IEnumerable<XElement> AppSettingElement(string parentFull)
        {
            var elements = CacheHelper.Get<IEnumerable<XElement>>(XElementCacheKey);
            if (elements == null)
            {
                elements = XDocument.Load(AppSettingsPath).Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings")).Elements();
                if (elements != null)
                {
                    if (IsRemoteFile)
                        CacheHelper.Set(XElementCacheKey, elements);
                    else
                        CacheHelper.Set(XElementCacheKey, elements, CacheHelper.CreateMonitor(AppSettingsPath));
                }
            }

            if (!string.IsNullOrEmpty(parentFull))
            {
                parentFull.Split('.','\\','/').ToList().ForEach(x =>
                {
                    elements = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(x)).Elements();
                });
            }
            return elements;
        }

        protected abstract TValue LoadConfigFromFile<TValue>(string parentFull) where TValue : class;

        internal TValue LoadConfig<TValue>(string parentFull) where TValue : class
        {
            try
            {
                var settings = LoadConfigFromFile<TValue>(parentFull);
                if (AppSettingConfig.IsLoadCache && settings != null)
                {
                    if (IsRemoteFile)
                        CacheHelper.Set(Key, settings);
                    else
                        CacheHelper.Set(Key, settings, CacheHelper.CreateMonitor(AppSettingsPath));
                }
                return settings;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(ex.Message);
            }
        }
    }
}
