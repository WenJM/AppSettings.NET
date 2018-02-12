using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;

namespace AppSettings.Client.AppSettings
{
    internal abstract class AppSettingsBase
    {
        private string IsCachekey = "APPSETTINGSBASE_ISCACHE";

        protected abstract string Key { get;}

        protected string XmlPath
        {
            get
            {
                string filePath = ConfigurationManager.AppSettings["AppSettingsPath"];
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ConfigurationErrorsException("自定义配置文件配置AppSettingsPath未找到");
                }
                if (filePath.StartsWith("~"))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + filePath.TrimStart('~').TrimStart('\\');
                }
                if (File.Exists(filePath))
                {
                    return filePath;
                }
                if (IsRemoteExist(filePath))
                {
                    return filePath;
                }
                throw new ConfigurationErrorsException("自定义配置文件不存在");
            }
        }

        protected abstract TSource GetAppSettings<TSource>(string xmlPath, string xmlSubPath) where TSource : class;

        internal TSource LoadConfig<TSource>(string xmlSubPath) where TSource : class
        {
            try
            {
                var settings = GetAppSettings<TSource>(XmlPath, xmlSubPath);
                if (HttpRuntime.Cache[Key] != null)
                {
                    HttpRuntime.Cache.Remove(Key);
                }

                if (AppSettingConfig.IsLoadCache && File.Exists(XmlPath) && settings != null)
                {
                    var cdd = new CacheDependency(XmlPath);
                    HttpRuntime.Cache.Insert(Key, settings, cdd, DateTime.MaxValue, Cache.NoSlidingExpiration);
                }
                return settings;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(ex.Message);
            }
        }

        private bool IsRemoteExist(string uri)
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(uri);
                req.Method = "HEAD";
                req.Timeout = 100;
                res = (HttpWebResponse)req.GetResponse();
                return (res.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                    res = null;
                }
                if (req != null)
                {
                    req.Abort();
                    req = null;
                }
            }
        } 
    }
}
