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
                settings = LoadConfig<List<XElement>>(null);
                if (settings == null)
                    return string.Empty;
            }

            var xml = settings.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(name));
            if (xml == null)
                return string.Empty;

            var att = xml.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(attributes));
            if (att == null)
                return string.Empty;

            return att.Value;
        }

        protected override TValue LoadConfigFromFile<TValue>(string parentFull)
        {
            return this.AppSettingElement(parentFull).ToList() as TValue;
        }
    }
}
