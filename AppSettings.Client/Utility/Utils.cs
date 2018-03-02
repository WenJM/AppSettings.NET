using System;
using System.Net;
using System.Collections.Generic;

namespace AppSettings.Client.Utility
{
    static class Utils
    {
        static List<Type> MapBasicType; //基础类型
        static List<Type> MapNullableType;//可空类型
        static Dictionary<Type, Func<IConvertible,  object, IFormatProvider, object>> dicConvert; //类型转换

        static Utils()
        {
            //基础类型
            MapBasicType = new List<Type>();
            MapBasicType.Add(UtilConstants.TypeOfString);
            MapBasicType.Add(UtilConstants.TypeOfBoolean);
            MapBasicType.Add(UtilConstants.TypeOfChar);
            MapBasicType.Add(UtilConstants.TypeOfByte);
            MapBasicType.Add(UtilConstants.TypeOfSbyte);
            MapBasicType.Add(UtilConstants.TypeOfSingle);
            MapBasicType.Add(UtilConstants.TypeOfDouble);
            MapBasicType.Add(UtilConstants.TypeOfDecimal);
            MapBasicType.Add(UtilConstants.TypeOfDateTime);
            MapBasicType.Add(UtilConstants.TypeOfShort);
            MapBasicType.Add(UtilConstants.TypeOfUShort);
            MapBasicType.Add(UtilConstants.TypeOfInt32);
            MapBasicType.Add(UtilConstants.TypeOfUInt32);
            MapBasicType.Add(UtilConstants.TypeOfInt64);
            MapBasicType.Add(UtilConstants.TypeOfUInt64);

            //可空类型
            MapNullableType = new List<Type>();
            MapNullableType.Add(UtilConstants.TypeOfBoolean_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfChar_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfByte_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfSbyte_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfSingle_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfDouble_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfDecimal_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfDateTime_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfShort_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfUShort_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfInt32_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfUInt32_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfInt64_Nullable);
            MapNullableType.Add(UtilConstants.TypeOfUInt64_Nullable);

            //类型转换
            dicConvert = new Dictionary<Type, Func<IConvertible, object, IFormatProvider, object>>();
            dicConvert.Add(UtilConstants.TypeOfObject, (c, o, p) => o);
            dicConvert.Add(UtilConstants.TypeOfString, (c, o, p) => c.ToString(p));
            dicConvert.Add(UtilConstants.TypeOfBoolean, (c, o, p) => c.ToBoolean(p));
            dicConvert.Add(UtilConstants.TypeOfBoolean_Nullable, (c, o, p) => c.ToBoolean(p));
            dicConvert.Add(UtilConstants.TypeOfChar, (c, o, p) => c.ToChar(p));
            dicConvert.Add(UtilConstants.TypeOfChar_Nullable, (c, o, p) => c.ToChar(p));
            dicConvert.Add(UtilConstants.TypeOfByte, (c, o, p) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfByte_Nullable, (c, o, p) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfSbyte, (c, o, p) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfSbyte_Nullable, (c, o, p) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfSingle, (c, o, p) => c.ToSingle(p));
            dicConvert.Add(UtilConstants.TypeOfSingle_Nullable, (c, o, p) => c.ToSingle(p));
            dicConvert.Add(UtilConstants.TypeOfDouble, (c, o, p) => c.ToDouble(p));
            dicConvert.Add(UtilConstants.TypeOfDouble_Nullable, (c, o, p) => c.ToDouble(p));
            dicConvert.Add(UtilConstants.TypeOfDecimal, (c, o, p) => c.ToDecimal(p));
            dicConvert.Add(UtilConstants.TypeOfDecimal_Nullable, (c, o, p) => c.ToDecimal(p));
            dicConvert.Add(UtilConstants.TypeOfDateTime, (c, o, p) => c.ToDateTime(p));
            dicConvert.Add(UtilConstants.TypeOfDateTime_Nullable, (c, o, p) => c.ToDateTime(p));
            dicConvert.Add(UtilConstants.TypeOfShort, (c, o, p) => c.ToInt16(p));
            dicConvert.Add(UtilConstants.TypeOfShort_Nullable, (c, o, p) => c.ToInt16(p));
            dicConvert.Add(UtilConstants.TypeOfInt32, (c, o, p) => c.ToInt32(p));
            dicConvert.Add(UtilConstants.TypeOfInt32_Nullable, (c, o, p) => c.ToInt32(p));
            dicConvert.Add(UtilConstants.TypeOfInt64, (c, o, p) => c.ToInt64(p));
            dicConvert.Add(UtilConstants.TypeOfInt64_Nullable, (c, o, p) => c.ToInt64(p));
            dicConvert.Add(UtilConstants.TypeOfUShort, (c, o, p) => c.ToUInt16(p));
            dicConvert.Add(UtilConstants.TypeOfUShort_Nullable, (c, o, p) => c.ToUInt16(p));
            dicConvert.Add(UtilConstants.TypeOfUInt32, (c, o, p) => c.ToUInt32(p));
            dicConvert.Add(UtilConstants.TypeOfUInt32_Nullable, (c, o, p) => c.ToUInt32(p));
            dicConvert.Add(UtilConstants.TypeOfUInt64, (c, o, p) => c.ToUInt64(p));
            dicConvert.Add(UtilConstants.TypeOfUInt64_Nullable, (c, o, p) => c.ToUInt64(p));
        }

        public static bool IsBasic(Type type)
        {
            return MapBasicType.Contains(type);
        }

        public static bool IsNullable(Type type)
        {
            return MapNullableType.Contains(type);
        }

        public static Func<IConvertible, object, IFormatProvider, object> TryGetConvertFunc(Type type)
        {
            Func<IConvertible, object, IFormatProvider, object> func;

            return dicConvert.TryGetValue(type, out func) ? func : null;
        }

        public static bool CheckUri(string uri)
        {
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(uri);
                http.Method = "HEAD";
                http.Timeout = 1000 * 60;
                using (var response = (HttpWebResponse)http.GetResponse())
                {
                    http.Abort();
                    http = null;
                    return (response.StatusCode == HttpStatusCode.OK);
                }
            }
            catch
            {
                return false;
            }
        }

        public static DateTime ReadLastModified(string uri)
        {
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(uri);
                http.Method = "HEAD";
                http.Timeout = 1000 * 60;
                using (var res = (HttpWebResponse)http.GetResponse())
                {
                    return res.LastModified;
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
