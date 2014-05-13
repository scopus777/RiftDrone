using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AR.Drone.Client
{
    class EnumHelper
    {
        private static readonly char[] FlagDelimiter = new[] { ',' };

        public static bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", "value cannot null or empty");

            var enumType = typeof(TEnum);

            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", enumType.FullName));


            result = default(TEnum);

            // Try to parse the value directly 
            if (Enum.IsDefined(enumType, value))
            {
                result = (TEnum)Enum.Parse(enumType, value);
                return true;
            }

            // Get some info on enum
            var enumValues = Enum.GetValues(enumType);
            if (enumValues.Length == 0)
                return false;  // probably can't happen as you cant define empty enum?
            var enumTypeCode = Type.GetTypeCode(enumValues.GetValue(0).GetType());

            // Try to parse it as a flag 
            if (value.IndexOf(',') != -1)
            {
                if (!Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
                    return false;  // value has flags but enum is not flags

                // todo: cache this for efficiency
                var enumInfo = new Dictionary<string, object>();
                var enumNames = Enum.GetNames(enumType);
                for (var i = 0; i < enumNames.Length; i++)
                    enumInfo.Add(enumNames[i], enumValues.GetValue(i));

                ulong retVal = 0;
                foreach (var name in value.Split(FlagDelimiter))
                {
                    var trimmedName = name.Trim();
                    if (!enumInfo.ContainsKey(trimmedName))
                        return false;   // Enum has no such flag

                    var enumValueObject = enumInfo[trimmedName];
                    ulong enumValueLong;
                    switch (enumTypeCode)
                    {
                        case TypeCode.Byte:
                            enumValueLong = (byte)enumValueObject;
                            break;
                        case TypeCode.SByte:
                            enumValueLong = (byte)((sbyte)enumValueObject);
                            break;
                        case TypeCode.Int16:
                            enumValueLong = (ushort)((short)enumValueObject);
                            break;
                        case TypeCode.Int32:
                            enumValueLong = (uint)((int)enumValueObject);
                            break;
                        case TypeCode.Int64:
                            enumValueLong = (ulong)((long)enumValueObject);
                            break;
                        case TypeCode.UInt16:
                            enumValueLong = (ushort)enumValueObject;
                            break;
                        case TypeCode.UInt32:
                            enumValueLong = (uint)enumValueObject;
                            break;
                        case TypeCode.UInt64:
                            enumValueLong = (ulong)enumValueObject;
                            break;
                        default:
                            return false;   // should never happen
                    }
                    retVal |= enumValueLong;
                }
                result = (TEnum)Enum.ToObject(enumType, retVal);
                return true;
            }

            // the value may be a number, so parse it directly
            switch (enumTypeCode)
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (!SByte.TryParse(value, out sb))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, sb);
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (!Byte.TryParse(value, out b))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, b);
                    break;
                case TypeCode.Int16:
                    short i16;
                    if (!Int16.TryParse(value, out i16))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, i16);
                    break;
                case TypeCode.UInt16:
                    ushort u16;
                    if (!UInt16.TryParse(value, out u16))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, u16);
                    break;
                case TypeCode.Int32:
                    int i32;
                    if (!Int32.TryParse(value, out i32))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, i32);
                    break;
                case TypeCode.UInt32:
                    uint u32;
                    if (!UInt32.TryParse(value, out u32))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, u32);
                    break;
                case TypeCode.Int64:
                    long i64;
                    if (!Int64.TryParse(value, out i64))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, i64);
                    break;
                case TypeCode.UInt64:
                    ulong u64;
                    if (!UInt64.TryParse(value, out u64))
                        return false;
                    result = (TEnum)Enum.ToObject(enumType, u64);
                    break;
                default:
                    return false; // should never happen
            }

            return true;
        }
    }
}
