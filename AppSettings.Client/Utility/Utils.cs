using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSettings.Client.Utility
{
    static class Util
    {
        static List<Type> MapBasicType; //基础类型
        static List<Type> MapNullableType;//可空类型
        static Dictionary<Type, Func<IConvertible, IFormatProvider, object, object>> dicConvert; //类型转换

        static Util()
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
            dicConvert = new Dictionary<Type, Func<IConvertible, IFormatProvider, object, object>>();
            dicConvert.Add(UtilConstants.TypeOfObject, (c, p, o) => o);
            dicConvert.Add(UtilConstants.TypeOfString, (c, p, o) => c.ToString(p));
            dicConvert.Add(UtilConstants.TypeOfBoolean, (c, p, o) => c.ToBoolean(p));
            dicConvert.Add(UtilConstants.TypeOfBoolean_Nullable, (c, p, o) => c.ToBoolean(p));
            dicConvert.Add(UtilConstants.TypeOfChar, (c, p, o) => c.ToChar(p));
            dicConvert.Add(UtilConstants.TypeOfChar_Nullable, (c, p, o) => c.ToChar(p));
            dicConvert.Add(UtilConstants.TypeOfByte, (c, p, o) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfByte_Nullable, (c, p, o) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfSbyte, (c, p, o) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfSbyte_Nullable, (c, p, o) => c.ToSByte(p));
            dicConvert.Add(UtilConstants.TypeOfSingle, (c, p, o) => c.ToSingle(p));
            dicConvert.Add(UtilConstants.TypeOfSingle_Nullable, (c, p, o) => c.ToSingle(p));
            dicConvert.Add(UtilConstants.TypeOfDouble, (c, p, o) => c.ToDouble(p));
            dicConvert.Add(UtilConstants.TypeOfDouble_Nullable, (c, p, o) => c.ToDouble(p));
            dicConvert.Add(UtilConstants.TypeOfDecimal, (c, p, o) => c.ToDecimal(p));
            dicConvert.Add(UtilConstants.TypeOfDecimal_Nullable, (c, p, o) => c.ToDecimal(p));
            dicConvert.Add(UtilConstants.TypeOfDateTime, (c, p, o) => c.ToDateTime(p));
            dicConvert.Add(UtilConstants.TypeOfDateTime_Nullable, (c, p, o) => c.ToDateTime(p));
            dicConvert.Add(UtilConstants.TypeOfShort, (c, p, o) => c.ToInt16(p));
            dicConvert.Add(UtilConstants.TypeOfShort_Nullable, (c, p, o) => c.ToInt16(p));
            dicConvert.Add(UtilConstants.TypeOfInt32, (c, p, o) => c.ToInt32(p));
            dicConvert.Add(UtilConstants.TypeOfInt32_Nullable, (c, p, o) => c.ToInt32(p));
            dicConvert.Add(UtilConstants.TypeOfInt64, (c, p, o) => c.ToInt64(p));
            dicConvert.Add(UtilConstants.TypeOfInt64_Nullable, (c, p, o) => c.ToInt64(p));
            dicConvert.Add(UtilConstants.TypeOfUShort, (c, p, o) => c.ToUInt16(p));
            dicConvert.Add(UtilConstants.TypeOfUShort_Nullable, (c, p, o) => c.ToUInt16(p));
            dicConvert.Add(UtilConstants.TypeOfUInt32, (c, p, o) => c.ToUInt32(p));
            dicConvert.Add(UtilConstants.TypeOfUInt32_Nullable, (c, p, o) => c.ToUInt32(p));
            dicConvert.Add(UtilConstants.TypeOfUInt64, (c, p, o) => c.ToUInt64(p));
            dicConvert.Add(UtilConstants.TypeOfUInt64_Nullable, (c, p, o) => c.ToUInt64(p));
        }

        public static bool IsBasic(Type type)
        {
            return MapBasicType.Contains(type);
        }

        public static bool IsNullable(Type type)
        {
            return MapNullableType.Contains(type);
        }

        public static Func<IConvertible, IFormatProvider, object, object> TryGetConvertFunc(Type type)
        {
            Func<IConvertible, IFormatProvider, object, object> func;

            return dicConvert.TryGetValue(type, out func) ? func : null;
        }
    }
}
