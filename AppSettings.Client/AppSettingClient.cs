using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using AppSettings.Client.AppSettings;

namespace AppSettings.Client
{
    /// <summary>
    /// 提供对自定义应用程序配置文件的访问（支持深层节点）
    /// </summary>
    public static class AppSettingClient
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
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        public static T GetEntity<T>(string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntity(xmlSubPath);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="xmlSubPath">子节点</param>
        /// <returns></returns>
        public static T GetEntity<T>(Func<T, bool> predicate, string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntity(predicate, xmlSubPath);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="xmlSubPath">子节点</param>
        /// <returns></returns>
        public static List<T> GetEntityList<T>(string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntitys(xmlSubPath);
        }

        /// <summary>
        /// 根据实体获取对应的自定义配置，实体名和属性要和节点一致
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="xmlSubPath">子节点</param>
        /// <returns></returns>
        public static List<T> GetEntityList<T>(Func<T, bool> predicate, string xmlSubPath = null)
        {
            return (new ClassSettings<T>()).GetEntitys(predicate, xmlSubPath);
        }

        /// <summary>
        /// 设置自定义配置到缓存中
        /// </summary>
        /// <returns></returns>
        public static void InitSettingsCache()
        {
            _valueSettings.LoadConfig<NameValueCollection>(null);
            _xmlSettings.LoadConfig<List<XElement>>(null);
        }

        /// <summary>
        /// 设置自定义配置到缓存中
        /// </summary>
        /// <returns></returns>
        public static void InitSettingsCache<T>()
        {
            (new ClassSettings<T>()).LoadConfig<List<T>>(null);
        }
    }
}
