using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using System.Collections.Specialized;
using System.Xml.Linq;
using System.Web;
using System.Web.Caching;

namespace AppSettings.NET
{
    internal class ValueSettings : AppSettingsBase
    {
        /// <summary>
        /// 缓存Key
        /// </summary>
        protected string Key 
        {
            get { return "APPSETTINGSLIST_Default"; }
        }

        /// <summary>
        /// 获取自定义配置的数据。
        /// </summary>
        public NameValueCollection AppSettings
        {
            get
            {
                var settings = HttpRuntime.Cache.Get(Key);
                if (settings == null)
                {
                    settings = LoadData();
                }
                return (NameValueCollection)settings;
            }
        }

        /// <summary>
        /// 加载键值类型的自定义配置
        /// </summary>
        /// <returns></returns>
        public NameValueCollection LoadData()
        {
            try
            {
                var settings = GetAppSettings(XmlPath);
                if (HttpRuntime.Cache[Key] != null)
                {
                    HttpRuntime.Cache.Remove(Key);
                }

                if (settings != null && settings.Count > 0 && IsCache())
                {
                    var cdd = new CacheDependency(XmlPath); 
                    HttpRuntime.Cache.Insert(Key, settings, cdd, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration);
                }
                return settings;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(ex.Message);
            }
        }

        /// <summary>
        /// 加载键值类型的自定义配置
        /// </summary>
        /// <param name="xmlPath">自定义配置文件物理地址</param>
        /// <returns></returns>
        public NameValueCollection GetAppSettings(string xmlPath)
        {
            var nv = new NameValueCollection();

            var doc = XDocument.Load(xmlPath);

            var settings = doc.Elements().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings"));
            var adds = settings.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("add")).ToList();
            foreach (XElement x in adds)
            {
                var key = x.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("key"));
                var value = x.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("value"));
                if (key != null && value != null)
                {
                    if (!nv.AllKeys.Contains(key.Value))
                    {
                        nv.Add(key.Value, value.Value);
                    }
                }
            }
            return nv;
        }
    }
}
