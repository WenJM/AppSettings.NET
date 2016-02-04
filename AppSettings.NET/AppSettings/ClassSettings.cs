using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Xml.Linq;
using System.Web;
using System.Web.Caching;
using System.Linq.Expressions;

namespace AppSettings.NET
{
    internal class ClassSettings<T> : AppSettingsBase
    {
        /// <summary>
        /// 缓存Key
        /// </summary>
        protected string Key
        {
            get { return "APPSETTINGSLIST_Class_" + ReflectionHelper.GetClassName<T>(); }
        }

        /// <summary>
        /// 获取自定义配置实体信息
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取自定义配置实体信息
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取自定义配置实体信息集合
        /// </summary>
        /// <returns></returns>
        public List<T> GetEntityList(string xmlSubPath = null)
        {
            var settings = HttpRuntime.Cache.Get(Key);
            if (settings == null)
            {
                settings = LoadData(xmlSubPath);
            }
            return settings as List<T>;
        }

        /// <summary>
        /// 获取自定义配置实体信息集合
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 加载自定义配置数据
        /// </summary>
        /// <returns></returns>
        public List<T> LoadData(string xmlSubPath = null)
        {
            try
            {
                var settings = GetAppSettings(XmlPath, xmlSubPath);
                if (HttpRuntime.Cache[Key] != null)
                {
                    HttpRuntime.Cache.Remove(Key);
                }

                if (settings != null && settings.Count > 0 && IsCache())
                {
                    //缓存依赖文件
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

        /// <summary>
        /// 加载自定义配置列表
        /// </summary>
        /// <param name="xmlPath">自定义配置文件物理地址</param>
        /// <param name="xmlSubPath">配置文件中的相对位置</param>
        /// <returns></returns>
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

            List<T> result = new List<T>();
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
