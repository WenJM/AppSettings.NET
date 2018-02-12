using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal class XmlSettings : AppSettingsBase
    {
        protected override string Key
        {
            get { return "APPSETTINGS_XML"; }
        }

        public string LoadValue(string name, string attributes)
        {
            var settings = HttpRuntime.Cache.Get(Key) as List<XElement>;
            if (settings == null)
            {
                settings = name.Contains(".") ? LoadConfig<List<XElement>>(null) : LoadConfig<List<XElement>>(name);
            }

            if (settings == null)
            {
                return string.Empty;
            }

            name = name.Contains(".") ? name.Split('.').ToList().Last() : name;

            var xml = settings.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(name));

            if (xml == null)
                return string.Empty;

            var att = xml.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(attributes));

            if (att != null)
                return att.Value;

            return string.Empty;
        }

        protected override TValue LoadConfigFromFile<TValue>(string xmlPath, string xmlSubPath)
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
            return settings as TValue;
        }
    }
}
