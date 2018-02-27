using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using AppSettings.Client.Utility;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.Helper
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {
        public static string GetRealName<TSource>()
        {
            var tSource = typeof(TSource);
            return !tSource.IsGenericType ?
                    tSource.Name :
                    tSource.GetGenericArguments().FirstOrDefault().Name;
        }

        public static object GetDefaultValue(object value, Type type)
        {
            try
            {
                value = ConvertValue(value, type, null);
            }
            catch (Exception)
            {
                value = default(object);
            }
            return value;
        }

        public static object ConvertValue(object value, Type convertType, IFormatProvider provider)
        {
            if (convertType == null)
            {
                throw new ArgumentNullException("convertType is null");
            }
            if (convertType.IsValueType && !Utils.IsNullable(convertType) && value == null)
            {
                throw new InvalidCastException("vauleType is null");
            }
            if (value == null | value.ToString().Length == 0)
            {
                return null;
            }

            var convertible = value as IConvertible;
            if (convertible == null)
            {
                return value;
            }

            if (convertType == UtilConstants.TypeOfBoolean || convertType == UtilConstants.TypeOfBoolean_Nullable)
            {
                if (value.ToString().Equals("1"))
                    return true;
                if (value.ToString().Equals("0"))
                    return false;
            }

            var func = Utils.TryGetConvertFunc(convertType);
            if (func != null)
            {
                return func(convertible, provider, value);
            }

            return value;
        }

        public static IList BuildArray(List<XElement> elements, Type buildType)
        {
            Type listType = typeof(List<>).MakeGenericType(buildType);

            IList list = Activator.CreateInstance(listType) as IList;

            if (elements.Count == 0) return list;

            var propertes = buildType.GetProperties();
            foreach (XElement element in elements)
            {
                var obj = BuildObj(element, buildType, propertes);
                list.Add(obj);
            }
            return list;
        }

        public static object BuildObj(List<XElement> elements, Type buildType)
        {
            if (elements.Count == 0)
                return null;

            var element = elements[0];

            var propertes = buildType.GetProperties();
            var obj = BuildObj(element, buildType, propertes);

            return obj;
        }

        public static object BuildObj(XElement element, Type buildType, PropertyInfo[] propertes)
        {
            var elements = element.Elements();
            var attributes = element.Attributes();

            var obj = buildType.Assembly.CreateInstance(buildType.FullName);
            foreach (var current in propertes)
            {
                if (current.PropertyType.IsGenericType && !Utils.IsBasic(current.PropertyType.GetGenericArguments()[0]))
                {
                    var typeSub = current.PropertyType.GetGenericArguments()[0];
                    
                    var elementsSub = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(typeSub.Name)).ToList();
                    
                    var listSub = BuildArray(elementsSub, typeSub);
                    
                    current.SetValue(obj, listSub, null);
                }
                else if (!current.PropertyType.IsGenericType && !Utils.IsBasic(current.PropertyType))
                {
                    var elementsSub = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(current.PropertyType.Name)).ToList();
                    
                    var innerObj = BuildObj(elementsSub, current.PropertyType);
                    
                    current.SetValue(obj, innerObj, null);
                }
                else
                {
                    var elementCurrent = elements.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(current.Name));
                    var attribute = attributes.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(current.Name));

                    if (elementCurrent != null || attribute != null)
                    {
                        var value = elementCurrent != null ? elementCurrent.Value : attribute.Value;

                        current.SetValue(obj, GetDefaultValue(value, current.PropertyType), null);
                    }
                }
            }
            return obj;
        }
    }
}
