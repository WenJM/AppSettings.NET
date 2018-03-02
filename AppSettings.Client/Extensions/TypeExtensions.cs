using AppSettings.Client.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSettings.Client.Extensions
{
    public static class TypeExtensions
    {
        public static string GetRealName(this Type genericType)
        {
            return !genericType.IsGenericType ?
                    genericType.Name :
                    genericType.GetGenericArguments().FirstOrDefault().GetRealName();
        }

        public static bool IsBasic(this Type type)
        {
            return Utils.IsBasic(type);
        }

        public static bool IsNullable(this Type type)
        {
            return Utils.IsNullable(type);
        }

        public static object GetDefaultValue(this Type convertType, object value)
        {
            try
            {
                value = ConvertValue(convertType, value);
            }
            catch (Exception)
            {
                value = default(object);
            }
            return value;
        }

        private static object ConvertValue(Type convertType, object value)
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
