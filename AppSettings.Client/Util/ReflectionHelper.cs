using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Collections;
using AppSettings.Client.Extensions;

namespace AppSettings.Client.Util
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    internal static class ReflectionHelper
    {
        /// <summary>
        /// 获取类名
        /// </summary>
        /// <returns></returns>
        internal static string GetClassName<T>()
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// 获取类属性
        /// </summary>
        /// <returns></returns>
        internal static PropertyInfo[] GetPropertys<T>()
        {
            return typeof(T).GetProperties();
        }

        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object GetDefaultValue(object value, Type type)
        {
            try
            {
                value = ChangeValueType(value, type);
            }
            catch (Exception)
            {
                value = default(object);
            }
            return value;
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        internal static object ChangeValueType(object value, Type conversionType)
        {
            return ChangeValueType(value, conversionType, null);
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        internal static object ChangeValueType(object value, Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }

            bool valueCanbeNull = IsValueNullable(conversionType);
            if (valueCanbeNull && (value == null || value.ToString().Length == 0))//如果Nullable<>类型，且值是空，则直接返回空
            {
                return null;
            }
            if (value == null)
            {
                if (conversionType.IsValueType)
                {
                    throw new InvalidCastException("值为空！");
                }
                return null;
            }
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
            {
                if (value.GetType() != conversionType)
                {
                    throw new InvalidCastException("值不能被转换！");
                }
                return value;
            }
            if (conversionType == typeof(System.Boolean) || conversionType == typeof(Nullable<System.Boolean>))
            {
                if (value.ToString() == "1")
                    return true;
                if (value.ToString() == "0")
                    return false;
                return convertible.ToBoolean(provider);
            }
            if (conversionType == typeof(System.Char) || conversionType == typeof(Nullable<System.Char>))
            {
                return convertible.ToChar(provider);
            }
            if (conversionType == typeof(System.SByte) || conversionType == typeof(Nullable<System.SByte>))
            {
                return convertible.ToSByte(provider);
            }
            if (conversionType == typeof(System.Byte) || conversionType == typeof(Nullable<System.Byte>))
            {
                return convertible.ToByte(provider);
            }
            if (conversionType == typeof(System.Int16) || conversionType == typeof(Nullable<System.Int16>))
            {
                return convertible.ToInt16(provider);
            }
            if (conversionType == typeof(System.UInt16) || conversionType == typeof(Nullable<System.UInt16>))
            {
                return convertible.ToUInt16(provider);
            }
            if (conversionType == typeof(System.Int32) || conversionType == typeof(Nullable<System.Int32>))
            {
                return convertible.ToInt32(provider);
            }
            if (conversionType == typeof(System.UInt32) || conversionType == typeof(Nullable<System.UInt32>))
            {
                return convertible.ToUInt32(provider);
            }
            if (conversionType == typeof(System.Int64) || conversionType == typeof(Nullable<System.Int64>))
            {
                return convertible.ToInt64(provider);
            }
            if (conversionType == typeof(System.UInt64) || conversionType == typeof(Nullable<System.UInt64>))
            {
                return convertible.ToUInt64(provider);
            }
            if (conversionType == typeof(System.Single) || conversionType == typeof(Nullable<System.Single>))
            {
                return convertible.ToSingle(provider);
            }
            if (conversionType == typeof(System.Double) || conversionType == typeof(Nullable<System.Double>))
            {
                return convertible.ToDouble(provider);
            }
            if (conversionType == typeof(System.Decimal) || conversionType == typeof(Nullable<System.Decimal>))
            {
                return convertible.ToDecimal(provider);
            }
            if (conversionType == typeof(System.DateTime) || conversionType == typeof(Nullable<System.DateTime>))
            {
                return convertible.ToDateTime(provider);
            }
            if (conversionType == typeof(System.String))
            {
                return convertible.ToString(provider);
            }
            if (conversionType == typeof(System.Object))
            {
                return value;
            }
            return value;
        }

        /// <summary>
        /// 判断改类型是否是基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 判断该类型是否是可为空值的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsValueNullable(Type type)
        {
            if (type == typeof(Nullable<System.Boolean>))
                return true;
            if (type == typeof(Nullable<System.Char>))
                return true;
            if (type == typeof(Nullable<System.SByte>))
                return true;
            if (type == typeof(Nullable<System.Byte>))
                return true;
            if (type == typeof(Nullable<System.Int16>))
                return true;
            if (type == typeof(Nullable<System.UInt16>))
                return true;
            if (type == typeof(Nullable<System.Int32>))
                return true;
            if (type == typeof(Nullable<System.UInt32>))
                return true;
            if (type == typeof(Nullable<System.Int64>))
                return true;
            if (type == typeof(Nullable<System.UInt64>))
                return true;
            if (type == typeof(Nullable<System.Single>))
                return true;
            if (type == typeof(Nullable<System.Double>))
                return true;
            if (type == typeof(Nullable<System.Decimal>))
                return true;
            if (type == typeof(Nullable<System.DateTime>))
                return true;
            return false;
        }

        /// <summary>
        /// 生成类型集合
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static IList BuildList(List<XElement> elements, Type type)
        {
            Type listType = typeof(List<>).MakeGenericType(type);

            IList list = Activator.CreateInstance(listType) as IList;

            if (elements.Count == 0) return list;

            //该类型的属性
            var Propertes = type.GetProperties();
            //逐行解析
            foreach (XElement element in elements)
            {
                var obj = BuildObj(type, Propertes, element);
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// 生成单个类型对象
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object BuildObj(List<XElement> elements, Type type)
        {
            if (elements.Count == 0) 
                return null;

            var element = elements[0];

            //该类型的属性
            var Propertes = type.GetProperties();
            var obj = BuildObj(type, Propertes, element);

            return obj;
        }

        /// <summary>
        /// 生成单个类型对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Propertes"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        internal static object BuildObj(Type type, PropertyInfo[] Propertes, XElement element)
        {
            //得到XML中的所有属性
            var attributes = element.Attributes().ToList();

            //创建实例
            var obj = type.Assembly.CreateInstance(type.FullName);

            foreach (var current in Propertes)
            {
                var propertyInfo = current;
                var attribute = attributes.FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase(propertyInfo.Name));

                //如果是泛型（暂时仅仅实现复杂类型）
                if (propertyInfo.PropertyType.IsGenericType && !IsBasicType(propertyInfo.PropertyType.GetGenericArguments()[0]))
                {
                    //得到泛型的T的类型
                    var type2 = propertyInfo.PropertyType.GetGenericArguments()[0];
                    //得到T的对于的XML元素集合
                    var elements2 = element.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(type2.Name)).ToList();
                    //构建集合
                    var subList = BuildList(elements2, type2);
                    //给属性设置值
                    propertyInfo.SetValue(obj, subList, null);
                }
                //如果是自定义类型
                else if (!propertyInfo.PropertyType.IsGenericType && !IsBasicType(propertyInfo.PropertyType))
                {
                    //得到T的对应的XML元素集合
                    var elements2 = element.Elements().Where(s => s.Name.LocalName.EqualsIgnoreCase(propertyInfo.PropertyType.Name)).ToList();
                    //构建对象
                    var innerObj = BuildObj(elements2, propertyInfo.PropertyType);
                    //给属性设置值
                    propertyInfo.SetValue(obj, innerObj, null);
                }
                else
                {
                    //简单类型的属性
                    if (attribute != null)
                    {
                        propertyInfo.SetValue(obj, GetDefaultValue(attribute.Value, propertyInfo.PropertyType), null);
                    }
                }
            }
            return obj;
        }
    }
}
