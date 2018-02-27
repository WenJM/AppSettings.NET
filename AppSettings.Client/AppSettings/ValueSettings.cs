using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AppSettings.Client.Helper;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal class ValueSettings : AppSettingsBase
    {
        protected override string Key 
        {
            get { return "APPSETTINGS_DEFAULT"; }
        }

        public NameValueCollection AppSettings
        {
            get
            {
                var settings = CacheHelper.Get<NameValueCollection>(Key);
                if (settings == null)
                {
                    settings = LoadConfig<NameValueCollection>(null);
                }
                return settings;
            }
        }

        protected override TValue LoadConfigFromFile<TValue>(string parentFull)
        {
            var nv = new NameValueCollection();
            var elements = this.AppSettingElement(parentFull);
            var adds = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase("add"));
            foreach (var x in adds)
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
            return nv as TValue;
        }
    }
}
