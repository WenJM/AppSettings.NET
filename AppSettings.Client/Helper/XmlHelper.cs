using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.Helper
{
    public static class XmlHelper
    {
        public static IEnumerable<XElement> TryGetXElementOfCache(string key, string uri, bool IsRemote)
        {
            var elements = CacheHelper.Get<IEnumerable<XElement>>(key);
            if (elements == null)
            {
                elements = XDocument.Load(uri).Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings")).Elements();
                if (elements != null)
                {
                    if (IsRemote)
                        CacheHelper.Set(key, elements);
                    else
                        CacheHelper.Set(key, elements, CacheHelper.CreateMonitor(uri));
                }
            }
            return elements;
        }

        public static IEnumerable<XElement> ResetXElementOfCache(string key, string uri, bool IsRemote)
        {
            var elements = XDocument.Load(uri).Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings")).Elements();
            if (elements != null)
            {
                if (IsRemote)
                    CacheHelper.Set(key, elements);
                else
                    CacheHelper.Set(key, elements, CacheHelper.CreateMonitor(uri));
            }
            return elements;
        }
    }
}
