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
        private static readonly ValueSettings _valueSettings = new ValueSettings();

        private static readonly XmlSettings _xmlSettings = new XmlSettings();

        public static NameValueCollection AppSettings
        {
            get
            {
                return _valueSettings.AppSettings;
            }
        }

        public static string AttributesValue(string name, string attributes)
        {
            return _xmlSettings.LoadValue(name, attributes);
        }

        public static void InitSettingCache()
        {
            _valueSettings.LoadConfig<NameValueCollection>(null);
            _xmlSettings.LoadConfig<List<XElement>>(null);
        }
    }
}
