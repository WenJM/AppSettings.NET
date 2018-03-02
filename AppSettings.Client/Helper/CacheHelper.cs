using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace AppSettings.Client.Helper
{
    public static class CacheHelper
    {
        private static readonly MemoryCache _Cache = MemoryCache.Default;

        public static T Get<T>(string key)
        {
            var obj = _Cache.Get(key);
            return obj == null ? default(T) : (T)obj;
        }

        public static void Set<T>(string key, T value)
        {
            Set(key, value, 0, null, null);
        }

        public static void Set<T>(string key, T value, int expiration)
        {
            Set(key, value, expiration, null, null);
        }

        public static void Set<T>(string key, T value, ChangeMonitor monitor)
        {
            Set(key, value, 0, monitor, null);
        }

        public static void Set<T>(string key, T value, IList<string> keys)
        {
            Set(key, value, 0, null, keys);
        }

        public static void Set<T>(string key, T value, int expiration, ChangeMonitor monitor, IList<string> keys)
        {
            _Cache.Remove(key);

            _Cache.Set(key, value, CreatePolicy(expiration, monitor, keys));
        }

        public static ChangeMonitor CreateMonitor(string filePath)
        {
            return CreateMonitor(new List<string> { filePath });
        }

        public static ChangeMonitor CreateMonitor(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    throw new FileNotFoundException();
                }
            }
            return new HostFileChangeMonitor(filePaths);
        }

        private static CacheItemPolicy CreatePolicy(int expiration, ChangeMonitor change, IList<string> keys)
        {
            var policy = new CacheItemPolicy();
            if (expiration <= 0)
            {
                policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
                policy.Priority = CacheItemPriority.NotRemovable;
            }
            else
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expiration);
                policy.Priority = CacheItemPriority.Default;
            }

            //其它缓存Keys依赖
            if (keys != null && keys.Any())
            {
                policy.ChangeMonitors.Add(_Cache.CreateCacheEntryChangeMonitor(keys));
            }

            ///文件依赖
            if (change != null)
            {
                policy.ChangeMonitors.Add(change);
            }

            return policy;
        }
    }
}
