using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using AppSettings.Client.Utility;
using AppSettings.Client.Extensions;
using AppSettings.Client.Exception;

namespace AppSettings.Client.Helper
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {   
        public static object Build(Type tbuild, IEnumerable<XElement> elements)
        {
            if (elements == null || !elements.Any()) return null;

            if (tbuild.IsBasic()) throw new AppSettingException("please try AppSetting[key]");

            var obj = Build(tbuild, elements, null, null);

            return obj;
        }

        public static object Build(Type tbuild, IEnumerable<XElement> elements, PropertyInfo info, object obj)
        {
            if (elements == null || !elements.Any()) return obj;

            //Basic
            if (tbuild.IsBasic())
            {
                var element = elements.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(info.Name));

                BuildValue(obj, info, tbuild.GetDefaultValue(element.Value));

                return obj;
            }
            //Class
            else if (!tbuild.IsGenericType)
            {
                return BuildObj(tbuild, elements, info, obj);
            }
            //Generice
            else
            {
                var interfaces = tbuild.GetInterfaces();

                //List
                if (interfaces.Contains(UtilConstants.TypeOfIList) || interfaces.Contains(UtilConstants.TypeOfIEnumerable))
                {
                    var typeGeneric = tbuild.GetGenericArguments().FirstOrDefault();
                    var typeValue = typeof(List<>).MakeGenericType(typeGeneric);
                    var list = Activator.CreateInstance(typeValue) as IList;

                    var elementSub = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(typeGeneric.Name));
                    var propertes = typeGeneric.GetProperties();
                    foreach (var e in elementSub)
                    {
                        var objSub = Build(typeGeneric, new List<XElement> { e }, null, null);
                        list.Add(objSub);
                    }

                    BuildValue(obj, info, list);

                    return list;
                }
                else
                {
                    return BuildObj(tbuild, elements, info, obj);
                }
            }
        }

        private static object BuildObj(Type tbuild, IEnumerable<XElement> elements, PropertyInfo info, object obj)
        {
            var element = elements.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(info != null ? info.Name : tbuild.Name));
            if (element == null) return null;

            var elementSubs = new List<XElement>();
            elementSubs.AddRange(element.Elements());
            elementSubs.AddRange(element.Attributes().Select(s => new XElement(s.Name, s.Value)));

            var objSub = tbuild.Assembly.CreateInstance(tbuild.FullName);
            var propertes = tbuild.GetProperties();
            foreach (var p in propertes)
            {
                Build(p.PropertyType, elementSubs, p, objSub);
            }

            BuildValue(obj, info, objSub);

            return objSub;
        }

        private static void BuildValue(object obj, PropertyInfo info, object value)
        {
            if (obj != null && info != null && value != null)
            {
                info.SetValue(obj, value);
            }
        }
    }
}
