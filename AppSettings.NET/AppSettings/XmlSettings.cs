using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Configuration;
using System.Web;
using System.Web.Caching;

namespace AppSettings.NET
{
    internal class XmlSettings : AppSettingsBase
    {
        /// <summary>
        /// 缓存Key
        /// </summary>
        protected string Key
        {
            get { return "APPSETTINGSLIST_Xml"; }
        }

        /// <summary>
        /// 通过节点名称和Attributes属性名称获取Attributes值
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="attributes">Attributes属性名称</param>
        /// <returns></returns>
        public string GetAttributesValue(string name, string attributes)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadData();
            }
            if (settings == null)
            {
                return string.Empty;
            }

            //获取节点信息
            var xmls = settings as List<XElement>;
            var xml = xmls.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(name));
            if (xml == null)
            {
                return string.Empty;
            }

            //获取属性信息
            var att = xml.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(attributes));
            if (att != null)
            {
                return att.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 加载自定义配置数据
        /// </summary>
        /// <returns></returns>
        public List<XElement> LoadData()
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
        /// 加载自定义配置
        /// </summary>
        /// <param name="xmlPath">自定义配置文件物理地址</param>
        /// <returns></returns>
        public List<XElement> GetAppSettings(string xmlPath)
        {
            var doc = XDocument.Load(xmlPath);
            var settings = doc.Elements().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings"));
            return settings.Elements().ToList();
        }
    }
}
