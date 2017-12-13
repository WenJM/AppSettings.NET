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
    internal class ClassSettings<T> : AppSettingsBase
    {
        protected string Key
        {
            get { return "APPSETTINGSLIST_CLASS_" + ReflectionHelper.GetClassName<T>(); }
        }

        public T GetEntity(string xmlSubPath = null)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadData(xmlSubPath);
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
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadData(xmlSubPath);
            }

            var entitys = settings as List<T>;
            if (entitys != null)
            {
                return entitys.FirstOrDefault(predicate);
            }
            return default(T);
        }

        public List<T> GetEntityList(string xmlSubPath = null)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadData(xmlSubPath);
            }
            return settings as List<T>;
        }

        public List<T> GetEntityList(Func<T, bool> predicate, string xmlSubPath = null)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadData(xmlSubPath);
            }

            var entitys = settings as List<T>;
            if (entitys != null)
            {
                return entitys.Where(predicate).ToList();
            }
            return null;
        }

        public List<T> LoadData(string xmlSubPath = null)
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

        private List<T> GetAppSettings(string xmlPath, string xmlSubPath = null)
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
                return result;

            var Propertes = ReflectionHelper.GetPropertys<T>();
            foreach (XElement element in settings)
            {
                var obj = (T)ReflectionHelper.BuildObj(typeof(T), Propertes, element);
                result.Add(obj);
            }
            return result;
        }
    }
}
