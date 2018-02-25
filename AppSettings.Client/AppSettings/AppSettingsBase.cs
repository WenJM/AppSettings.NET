﻿using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.AppSettings
{
    internal abstract class AppSettingsBase
    {
        private const string XmlCacheKey = "XMLCACHEKEY";

        private bool IsRemoteFile = false; //远程文件

        protected abstract string Key { get;}

        protected string AppSettingsPath
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
                if (CheckUri(filePath))
                {
                    IsRemoteFile = true;

                    return filePath;
                }
                throw new ConfigurationErrorsException("自定义配置文件不存在");
            }
        }

        protected IEnumerable<XElement> AppSettingElement(string parentFull)
        {
            var elements = HttpRuntime.Cache[XmlCacheKey] as IEnumerable<XElement>;
            if (elements == null)
            {
                elements = XDocument.Load(AppSettingsPath).Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings")).Elements();
                if (elements != null)
                {
                    var cdd = new CacheDependency(AppSettingsPath);
                    HttpRuntime.Cache.Insert(XmlCacheKey, elements, cdd, DateTime.MaxValue, Cache.NoSlidingExpiration);
                }
            }

            if (!string.IsNullOrEmpty(parentFull))
            {
                parentFull.Split('.','\\','/').ToList().ForEach(x =>
                {
                    elements = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(x)).Elements();
                });
            }
            return elements;
        }

        protected abstract TValue LoadConfigFromFile<TValue>(string parentFull) where TValue : class;

        internal TValue LoadConfig<TValue>(string parentFull) where TValue : class
        {
            try
            {
                var settings = LoadConfigFromFile<TValue>(parentFull);
                if (HttpRuntime.Cache[Key] != null)
                {
                    HttpRuntime.Cache.Remove(Key);
                }
                if (AppSettingConfig.IsLoadCache && !IsRemoteFile && settings != null)
                {
                    var cdd = new CacheDependency(AppSettingsPath);
                    HttpRuntime.Cache.Insert(Key, settings, cdd, DateTime.MaxValue, Cache.NoSlidingExpiration);
                }
                return settings;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(ex.Message);
            }
        }

        private bool CheckUri(string uri)
        {
            HttpWebRequest re = null;
            HttpWebResponse res = null;
            try
            {
                re = (HttpWebRequest)WebRequest.Create(uri);
                re.Method = "HEAD";
                re.Timeout = 100;
                res = (HttpWebResponse)re.GetResponse();
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
                if (re != null)
                {
                    re.Abort();
                    re = null;
                }
            }
        } 
    }
}
