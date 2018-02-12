using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal class ValueSettings : AppSettingsBase
    {
        protected override string Key 
        {
            get { return "APPSETTINGSLIST_DEFAULT"; }
        }

        public NameValueCollection AppSettings
        {
            get
            {
                var settings = HttpRuntime.Cache.Get(Key) as NameValueCollection;
                if (settings == null)
                {
                    settings = LoadConfig<NameValueCollection>(null);
                }
                return settings;
            }
        }

        protected override TSource GetAppSettings<TSource>(string xmlPath, string xmlSubPath)
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
            return nv as TSource;
        }
    }
}
