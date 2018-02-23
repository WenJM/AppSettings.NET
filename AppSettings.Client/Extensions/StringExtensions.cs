using System;

namespace AppSettings.Client.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// 确定此字符串是否与指定的 System.String 对象具有相同的值。忽略大小写
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string thisValue, string value)
        {
            return string.Equals(thisValue, value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
