using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.Util
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    internal static class ReflectionHelper
    {
        internal static object GetDefaultValue(object value, Type type)
        {
            try
            {
                value = ConvertValue(value, type);
            }
            catch (Exception)
            {
                value = default(object);
            }
            return value;
        }
        
        internal static object ConvertValue(object value, Type type)
        {
            return ConvertValue(value, type, null);
        }
        
        internal static object ConvertValue(object value, Type type, IFormatProvider provider)
        {
            if (type == null)
            {
                throw new ArgumentNullException("conversionType");
            }

            bool valueCanbeNull = IsValueNullable(type);
            if (valueCanbeNull && (value == null || value.ToString().Length == 0))
            {
                return null;
            }

            if (value == null)
            {
                if (type.IsValueType)
                {
                    throw new InvalidCastException("值为空！");
                }
                return null;
            }
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
            {
                if (value.GetType() != type)
                {
                    throw new InvalidCastException("值不能被转换！");
                }
                return value;
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                if (value.ToString() == "1")
                    return true;
                if (value.ToString() == "0")
                    return false;
                return convertible.ToBoolean(provider);
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return convertible.ToChar(provider);
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return convertible.ToSByte(provider);
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                return convertible.ToByte(provider);
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                return convertible.ToInt16(provider);
            }
            if (type == typeof(ushort) || type == typeof(ushort?))
            {
                return convertible.ToUInt16(provider);
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return convertible.ToInt32(provider);
            }
            if (type == typeof(uint) || type == typeof(uint?))
            {
                return convertible.ToUInt32(provider);
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return convertible.ToInt64(provider);
            }
            if (type == typeof(ulong) || type == typeof(ulong?))
            {
                return convertible.ToUInt64(provider);
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return convertible.ToSingle(provider);
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                return convertible.ToDouble(provider);
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return convertible.ToDecimal(provider);
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return convertible.ToDateTime(provider);
            }
            if (type == typeof(string))
            {
                return convertible.ToString(provider);
            }
            if (type == typeof(object))
            {
                return value;
            }
            return value;
        }

        internal static bool IsBasicType(Type type)
        {
            switch (type.Name)
            {
                case "Boolean":
                case "Byte":
                case "Char":
                case "DateTime":
                case "Decimal":
                case "Double":
                case "Int16":
                case "Int32":
                case "Int64":
                case "SByte":
                case "Single":
                case "String":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }

        internal static bool IsValueNullable(Type type)
        {
            if (type == typeof(bool?))
                return true;
            if (type == typeof(char?))
                return true;
            if (type == typeof(sbyte?))
                return true;
            if (type == typeof(byte?))
                return true;
            if (type == typeof(short?))
                return true;
            if (type == typeof(ushort?))
                return true;
            if (type == typeof(int?))
                return true;
            if (type == typeof(uint?))
                return true;
            if (type == typeof(long?))
                return true;
            if (type == typeof(ulong?))
                return true;
            if (type == typeof(float?))
                return true;
            if (type == typeof(double?))
                return true;
            if (type == typeof(decimal?))
                return true;
            if (type == typeof(DateTime?))
                return true;
            return false;
        }
        
        internal static IList BuildArray(List<XElement> elements, Type type)
        {
            Type listType = typeof(List<>).MakeGenericType(type);

            IList list = Activator.CreateInstance(listType) as IList;

            if (elements.Count == 0) return list;

            var propertes = type.GetProperties();
            foreach (XElement element in elements)
            {
                var obj = BuildObj(element, type, propertes);
                list.Add(obj);
            }
            return list;
        }

        internal static object BuildObj(List<XElement> elements, Type type)
        {
            if (elements.Count == 0)
                return null;

            var element = elements[0];

            var propertes = type.GetProperties();
            var obj = BuildObj(element, type, propertes);

            return obj;
        }
        
        internal static object BuildObj(XElement element, Type type, PropertyInfo[] propertes)
        {
            var elements = element.Elements().ToList();
            var attributes = element.Attributes().ToList();

            var obj = type.Assembly.CreateInstance(type.FullName);
            foreach (var current in propertes)
            {
                if (current.PropertyType.IsGenericType && !IsBasicType(current.PropertyType.GetGenericArguments()[0]))
                {
                    var typeSub = current.PropertyType.GetGenericArguments()[0];
                    
                    var elements2 = element.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(typeSub.Name)).ToList();
                    
                    var listSub = BuildArray(elements2, typeSub);
                    
                    current.SetValue(obj, listSub, null);
                }
                else if (!current.PropertyType.IsGenericType && !IsBasicType(current.PropertyType))
                {
                    var elements2 = elements.Where(s => s.Name.LocalName.EqualsIgnoreCase(current.PropertyType.Name)).ToList();
                    
                    var innerObj = BuildObj(elements2, current.PropertyType);
                    
                    current.SetValue(obj, innerObj, null);
                }
                else
                {
                    var eChild = elements.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(current.Name));
                    var attribute = attributes.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(current.Name));

                    if (eChild != null || attribute != null)
                    {
                        var value = eChild != null ? eChild.Value : attribute.Value;

                        current.SetValue(obj, GetDefaultValue(value, current.PropertyType), null);
                    }
                }
            }
            return obj;
        }
    }
}
