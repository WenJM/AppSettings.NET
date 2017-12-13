using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Caching;

namespace AppSettings.Client
{
    internal class XmlSettings : AppSettingsBase
    {
        protected string Key
        {
            get { return "APPSETTINGSLIST_XML"; }
        }

        public string GetAttributesValue(string name, string attributes)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = name.Contains(".") ? LoadData() : LoadData(name);
            }

            if (settings == null)
                return string.Empty;

            var xmls = settings as List<XElement>;

            name = name.Contains(".") ? name.Split('.').ToList().Last() : name;
            var xml = xmls.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(name));

            if (xml == null)
                return string.Empty;

            var att = xml.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(attributes));

            if (att != null)
                return att.Value;

            return string.Empty;
        }

        public List<XElement> LoadData(string xmlSubPath = null)
        {
            try
            {
                var settings = GetAppSettings(XmlPath, xmlSubPath);
                if (HttpRuntime.Cache[Key] != null)
                {
                    HttpRuntime.Cache.Remove(Key);
                }

                if (settings != null && settings.Count > 0 && IsCache() && File.Exists(XmlPath))
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

        public List<XElement> GetAppSettings(string xmlPath, string xmlSubPath = null)
        {
            var result = new List<XElement>();

            var doc = XDocument.Load(xmlPath);
            var settings = doc.Elements().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings")).Elements().ToList();

            if (!string.IsNullOrEmpty(xmlSubPath))
            {
                var subs = settings;
                xmlSubPath.Split('.').ToList().ForEach(x =>
                {
                    subs = subs.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(x)).ToList();
                });

                if (subs.Any())
                {
                    settings.AddRange(subs);
                }
            }
            return settings;
        }
    }
}
