
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppSettings.Client.Utility;

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

        public static Type GetRealType(this Type genericType)
        {
            return !genericType.IsGenericType ?
                    genericType :
                    genericType.GetGenericArguments().FirstOrDefault().GetRealType();
        }

        public static bool IsBasic(this Type type)
        {
            return Utils.IsBasic(type);
        }

        public static bool IsNullable(this Type type)
        {
            return Utils.IsNullable(type);
        }
    }
}
