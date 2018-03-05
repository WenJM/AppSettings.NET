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
            var elements = this.AppSettingElement(parentFull);
            if (!elements.Any())
            {
                return default(TValue);
            }
            return ReflectionHelper.Build(typeof(TValue), elements) as TValue;
        }
    }
}
