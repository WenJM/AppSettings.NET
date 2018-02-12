using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Xml.Linq;
using AppSettings.Client.Util;
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
                    tSource.Name :
                    tSource.GetGenericArguments().FirstOrDefault().Name.ToLower() + "_ARRAY";

                return "APPSETTINGS_CLASS_" + className;
            }
        }

        public TSource Load()
        {
            return this.Load(null);
        }

        public TSource Load(string xmlSubPath)
        {
            var settings = HttpRuntime.Cache.Get(Key) as TSource;
            if (settings == null)
            {
                settings = LoadConfig<TSource>(xmlSubPath);
            }
            return settings;
        }

        protected override TValue LoadConfigFromFile<TValue>(string xmlPath, string xmlSubPath)
        {
            var tSoureType = typeof(TValue);
            var tSubSourceType = tSoureType.GetGenericArguments().FirstOrDefault();

            //Todo 处理方式需要再考虑
            var doc = XDocument.Load(xmlPath);
            var settings = doc.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings"));
            if (string.IsNullOrEmpty(xmlSubPath))
            {
                settings = settings.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(this.GenericName<TValue>()));
            }
            else
            {
                var arr = xmlSubPath.Split('.');
                foreach (var sub in arr)
                {
                    settings = settings.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(sub));
                }
            }
            if (!settings.Any())
            {
                return default(TValue);
            }
            
            if (tSoureType.IsGenericType)
            {
                return ReflectionHelper.BuildArray(settings.FirstOrDefault().Elements().ToList(), tSoureType.GetGenericArguments()[0]) as TValue;
            }

            return ReflectionHelper.BuildObj(settings.ToList(), tSoureType) as TValue;
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
