using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AppSettings.Client
{
    /// <summary>
    /// 提供对自定义应用程序配置文件的访问（支持深层节点）
    /// </summary>
    public static class SettingsManager
    {
        private static ValueSettings _valueSettings = new ValueSettings();

        private static XmlSettings _xmlSettings = new XmlSettings();

        /// <summary>
        /// 获取自定义配置的数据。
        /// </summary>
        public static NameValueCollection AppSettings
        {
            get
            {
                return _valueSettings.AppSettings;
            }
        }

        /// <summary>
        /// 获取自定义配置属性值
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="attributes">属性名称</param>
        /// <returns></returns>
        public static string GetAttributesValue(string name, string attributes)
        {
            return _xmlSettings.GetAttributesValue(name, attributes);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetEntity<T>(string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntity(xmlSubPath);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <typeparam name="T">条件</typeparam>
        /// <returns></returns>
        public static T GetEntity<T>(Func<T, bool> predicate, string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntity(predicate, xmlSubPath);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <returns></returns>
        public static List<T> GetEntityList<T>(string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntityList(xmlSubPath);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static List<T> GetEntityList<T>(Func<T, bool> predicate, string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntityList(predicate, xmlSubPath);
        }

        /// <summary>
        /// 设置自定义配置到缓存中
        /// </summary>
        /// <returns></returns>
        public static void InitSettingsCache()
        {
            _valueSettings.LoadData();
            _xmlSettings.LoadData();
        }

        /// <summary>
        /// 设置自定义配置到缓存中
        /// </summary>
        /// <returns></returns>
        public static void InitSettingsCache<T>()
        {
            (new ClassSettings<T>()).LoadData();
        }
    }
}
