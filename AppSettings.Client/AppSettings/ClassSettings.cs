using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using AppSettings.Client.Util;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal class ClassSettings<T> : AppSettingsBase
    {
        protected override string Key
        {
            get { return "APPSETTINGSLIST_CLASS_" + ReflectionHelper.GetClassName<T>().ToUpper(); }
        }

        public T GetEntity()
        {
            return this.GetEntity(null);
        }

        public T GetEntity(string xmlSubPath)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadConfig<List<T>>(xmlSubPath);
            }

            var entitys = settings as List<T>;
            if (entitys != null)
            {
                return entitys.FirstOrDefault();
            }
            return default(T);
        }

        public T GetEntity(Func<T, bool> predicate, string xmlSubPath = null)
        {
            var settings = HttpRuntime.Cache.Get(Key) as List<T>;
            if (settings == null)
            {
                settings = LoadConfig<List<T>>(xmlSubPath);
            }
         
            if (settings != null)
            {
                return settings.FirstOrDefault(predicate);
            }
            return default(T);
        }

        public List<T> GetEntitys()
        {
            return this.GetEntitys(string.Empty);
        }

        public List<T> GetEntitys(string xmlSubPath)
        {
            var settings = HttpRuntime.Cache.Get(Key) as List<T>;
            if (settings == null)
            {
                settings = LoadConfig<List<T>>(xmlSubPath);
            }
            return settings;
        }

        public List<T> GetEntitys(Func<T, bool> predicate)
        {
            return this.GetEntitys(predicate, null);
        }

        public List<T> GetEntitys(Func<T, bool> predicate, string xmlSubPath)
        {
            var settings = HttpRuntime.Cache.Get(Key) as List<T>;
            if (settings == null)
            {
                settings = LoadConfig<List<T>>(xmlSubPath);
            }
            
            if (settings != null)
            {
                return settings.Where(predicate).ToList();
            }
            return null;
        }

        protected override TSource GetAppSettings<TSource>(string xmlPath, string xmlSubPath)
        {
            var doc = XDocument.Load(xmlPath);

            var settings = doc.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings")).ToList();
            if (string.IsNullOrEmpty(xmlSubPath))
            {
                settings = settings.Elements().Where(s =>
                    s.Name.LocalName.EqualsIgnoreCase(ReflectionHelper.GetClassName<T>())).ToList();
            }
            else
            {
                var arr = xmlSubPath.Split('.');
                foreach (var sub in arr)
                {
                    settings = settings.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(sub)).ToList();
                }
            }

            var result = new List<T>();
            if (settings.Count == 0) 
                return result as TSource;

            var Propertes = ReflectionHelper.GetPropertys<T>();
            foreach (XElement element in settings)
            {
                var obj = (T)ReflectionHelper.BuildObj(typeof(T), Propertes, element);
                result.Add(obj);
            }
            return result as TSource;
        }
    }
}
