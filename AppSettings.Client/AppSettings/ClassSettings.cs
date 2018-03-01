using System;
using System.Collections.Generic;
using System.Linq;
using AppSettings.Client.Helper;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal class ClassSettings<TSource> : AppSettingsBase where TSource : class
    {
        protected override string Key
        {
            get
            {
                var genericType = typeof(TSource);
                return $"APPSETTINGS_CLASS_{genericType.GetRealName().ToUpper()}" + (genericType.IsGenericType ? "_ARRAY" : string.Empty);
            }
        }

        public TSource Load(string parentFull)
        {
            var settings = CacheHelper.Get<TSource>(Key);
            if (settings == null)
            {
                settings = LoadConfig<TSource>(parentFull);
            }
            return settings;
        }

        protected override TValue LoadConfigFromFile<TValue>(string parentFull)
        {
            var className = ReflectionHelper.GetRealName<TValue>();
            var elements = this.AppSettingElement(parentFull).Where(s => s.Name.LocalName.EqualsIgnoreCase(className));
            if (!elements.Any())
            {
                return default(TValue);
            }

            var tSoureType = typeof(TValue);
            if (tSoureType.IsGenericType)
            {
                return ReflectionHelper.BuildArray(elements.ToList(), tSoureType.GetGenericArguments().FirstOrDefault()) as TValue;
            }

            return ReflectionHelper.BuildObj(elements.ToList(), tSoureType) as TValue;
        }
    }
}
