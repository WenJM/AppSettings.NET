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

                SetValueCheckNull(info, obj, GetValueConvert(tbuild, element.Value));

                return obj;
            }
            //Class
            else if (!tbuild.IsGenericType)
            {
                return BuildObj(tbuild, elements, info, obj);
            }
            //Generic
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

                    SetValueCheckNull(info, obj, list);

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

            SetValueCheckNull(info, obj, objSub);

            return objSub;
        }

        private static void SetValueCheckNull(PropertyInfo info, object obj, object value)
        {
            if (info != null && obj != null && value != null)
            {
                info.SetValue(obj, value);
            }
        }

        private static object GetValueConvert(Type convertType, object value)
        {
            if (convertType == null)
            {
                throw new ArgumentNullException("convertType is null");
            }
            if (convertType.IsValueType && !convertType.IsNullable() && value == null)
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
                return func(convertible, value, null);
            }

            return value;
        }
    }
}
