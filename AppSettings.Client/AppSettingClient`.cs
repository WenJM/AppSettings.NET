using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppSettings.Client.AppSettings;

namespace AppSettings.Client
{
    /// <summary>
    /// 提供对自定义应用程序配置文件的访问（支持深层节点）
    /// </summary>
    public static class AppSettingClient<TSource> where TSource : class
    {
        private static readonly ClassSettings<TSource> _classSettings = new ClassSettings<TSource>();

        public static TSource Load()
        {
            return Load(string.Empty);
        }

        public static TSource Load(string xmlSubPath)
        {
            return _classSettings.Load(xmlSubPath);
        }

        public static void InitSettingCache()
        {
            _classSettings.LoadConfig<TSource>(null);
        }
    }
}
