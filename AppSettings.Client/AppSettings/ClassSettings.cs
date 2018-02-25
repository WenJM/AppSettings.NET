using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Xml.Linq;
using AppSettings.Client.Helper;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal class ClassSettings<TSource> : AppSettingsBase where TSource:class
    {
        protected override string Key
        {
            get
            {
                var tSource = typeof(TSource);

                var className = !tSource.IsGenericType ?
                    tSource.Name.ToUpper() :
                    tSource.GetGenericArguments().FirstOrDefault().Name.ToUpper() + "_ARRAY";

                return "APPSETTINGS_CLASS_" + className;
            }
        }

        public TSource Load()
        {
            return this.Load(null);
        }

        public TSource Load(string parentFull)
        {
            var settings = HttpRuntime.Cache.Get(Key) as TSource;
            if (settings == null)
            {
                settings = LoadConfig<TSource>(parentFull);
            }
            return settings;
        }

        protected override TValue LoadConfigFromFile<TValue>(string parentFull)
        {
            var tSoureType = typeof(TValue);

            var className = this.GenericName<TValue>();
            var elements = this.AppSettingElement(parentFull).Where(s => s.Name.LocalName.EqualsIgnoreCase(this.GenericName<TValue>()));
            if (!elements.Any())
            {
                return default(TValue);
            }
            
            if (tSoureType.IsGenericType)
            {
                return ReflectionHelper.BuildArray(elements.ToList(), tSoureType.GetGenericArguments()[0]) as TValue;
            }

            return ReflectionHelper.BuildObj(elements.ToList(), tSoureType) as TValue;
        }

        private string GenericName<TValue>()
        {
            var tSource = typeof(TValue);
            return !tSource.IsGenericType ?
                tSource.Name :
                tSource.GetGenericArguments().FirstOrDefault().Name;
        }
    }
}
