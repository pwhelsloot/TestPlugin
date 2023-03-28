using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
    public static class ValueCoercion
    {
        public static object Coerce(object value, Type targetType)
        {
            switch (value)
            {
                case char charValue:
                    if (targetType == typeof(char))
                        return (char)charValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)charValue;
                    if (targetType == typeof(byte))
                        return (byte)charValue;
                    if (targetType == typeof(short))
                        return (short)charValue;
                    if (targetType == typeof(ushort))
                        return (ushort)charValue;
                    if (targetType == typeof(int))
                        return (int)charValue;
                    if (targetType == typeof(uint))
                        return (uint)charValue;
                    if (targetType == typeof(long))
                        return (long)charValue;
                    if (targetType == typeof(ulong))
                        return (ulong)charValue;
                    if (targetType == typeof(float))
                        return (float)charValue;
                    if (targetType == typeof(double))
                        return (double)charValue;
                    if (targetType == typeof(decimal))
                        return (decimal)charValue;
                    break;
                case sbyte sbyteValue:
                    if (targetType == typeof(char))
                        return (char)sbyteValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)sbyteValue;
                    if (targetType == typeof(byte))
                        return (byte)sbyteValue;
                    if (targetType == typeof(short))
                        return (short)sbyteValue;
                    if (targetType == typeof(ushort))
                        return (ushort)sbyteValue;
                    if (targetType == typeof(int))
                        return (int)sbyteValue;
                    if (targetType == typeof(uint))
                        return (uint)sbyteValue;
                    if (targetType == typeof(long))
                        return (long)sbyteValue;
                    if (targetType == typeof(ulong))
                        return (ulong)sbyteValue;
                    if (targetType == typeof(float))
                        return (float)sbyteValue;
                    if (targetType == typeof(double))
                        return (double)sbyteValue;
                    if (targetType == typeof(decimal))
                        return (decimal)sbyteValue;
                    break;
                case byte byteValue:
                    if (targetType == typeof(char))
                        return (char)byteValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)byteValue;
                    if (targetType == typeof(byte))
                        return (byte)byteValue;
                    if (targetType == typeof(short))
                        return (short)byteValue;
                    if (targetType == typeof(ushort))
                        return (ushort)byteValue;
                    if (targetType == typeof(int))
                        return (int)byteValue;
                    if (targetType == typeof(uint))
                        return (uint)byteValue;
                    if (targetType == typeof(long))
                        return (long)byteValue;
                    if (targetType == typeof(ulong))
                        return (ulong)byteValue;
                    if (targetType == typeof(float))
                        return (float)byteValue;
                    if (targetType == typeof(double))
                        return (double)byteValue;
                    if (targetType == typeof(decimal))
                        return (decimal)byteValue;
                    break;
                case short shortValue:
                    if (targetType == typeof(char))
                        return (char)shortValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)shortValue;
                    if (targetType == typeof(byte))
                        return (byte)shortValue;
                    if (targetType == typeof(short))
                        return (short)shortValue;
                    if (targetType == typeof(ushort))
                        return (ushort)shortValue;
                    if (targetType == typeof(int))
                        return (int)shortValue;
                    if (targetType == typeof(uint))
                        return (uint)shortValue;
                    if (targetType == typeof(long))
                        return (long)shortValue;
                    if (targetType == typeof(ulong))
                        return (ulong)shortValue;
                    if (targetType == typeof(float))
                        return (float)shortValue;
                    if (targetType == typeof(double))
                        return (double)shortValue;
                    if (targetType == typeof(decimal))
                        return (decimal)shortValue;
                    break;
                case ushort ushortValue:
                    if (targetType == typeof(char))
                        return (char)ushortValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)ushortValue;
                    if (targetType == typeof(byte))
                        return (byte)ushortValue;
                    if (targetType == typeof(short))
                        return (short)ushortValue;
                    if (targetType == typeof(ushort))
                        return (ushort)ushortValue;
                    if (targetType == typeof(int))
                        return (int)ushortValue;
                    if (targetType == typeof(uint))
                        return (uint)ushortValue;
                    if (targetType == typeof(long))
                        return (long)ushortValue;
                    if (targetType == typeof(ulong))
                        return (ulong)ushortValue;
                    if (targetType == typeof(float))
                        return (float)ushortValue;
                    if (targetType == typeof(double))
                        return (double)ushortValue;
                    if (targetType == typeof(decimal))
                        return (decimal)ushortValue;
                    break;
                case int intValue:
                    if (targetType == typeof(char))
                        return (char)intValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)intValue;
                    if (targetType == typeof(byte))
                        return (byte)intValue;
                    if (targetType == typeof(short))
                        return (short)intValue;
                    if (targetType == typeof(ushort))
                        return (ushort)intValue;
                    if (targetType == typeof(int))
                        return (int)intValue;
                    if (targetType == typeof(uint))
                        return (uint)intValue;
                    if (targetType == typeof(long))
                        return (long)intValue;
                    if (targetType == typeof(ulong))
                        return (ulong)intValue;
                    if (targetType == typeof(float))
                        return (float)intValue;
                    if (targetType == typeof(double))
                        return (double)intValue;
                    if (targetType == typeof(decimal))
                        return (decimal)intValue;
                    if (targetType.IsEnum)
                    {
                      return (int)intValue;
                    }
                    break;
                case uint uintValue:
                    if (targetType == typeof(char))
                        return (char)uintValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)uintValue;
                    if (targetType == typeof(byte))
                        return (byte)uintValue;
                    if (targetType == typeof(short))
                        return (short)uintValue;
                    if (targetType == typeof(ushort))
                        return (ushort)uintValue;
                    if (targetType == typeof(int))
                        return (int)uintValue;
                    if (targetType == typeof(uint))
                        return (uint)uintValue;
                    if (targetType == typeof(long))
                        return (long)uintValue;
                    if (targetType == typeof(ulong))
                        return (ulong)uintValue;
                    if (targetType == typeof(float))
                        return (float)uintValue;
                    if (targetType == typeof(double))
                        return (double)uintValue;
                    if (targetType == typeof(decimal))
                        return (decimal)uintValue;
                    break;
                case long longValue:
                    if (targetType == typeof(char))
                        return (char)longValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)longValue;
                    if (targetType == typeof(byte))
                        return (byte)longValue;
                    if (targetType == typeof(short))
                        return (short)longValue;
                    if (targetType == typeof(ushort))
                        return (ushort)longValue;
                    if (targetType == typeof(int))
                        return (int)longValue;
                    if (targetType == typeof(uint))
                        return (uint)longValue;
                    if (targetType == typeof(long))
                        return (long)longValue;
                    if (targetType == typeof(ulong))
                        return (ulong)longValue;
                    if (targetType == typeof(float))
                        return (float)longValue;
                    if (targetType == typeof(double))
                        return (double)longValue;
                    if (targetType == typeof(decimal))
                        return (decimal)longValue;
                    break;
                case ulong ulongValue:
                    if (targetType == typeof(char))
                        return (char)ulongValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)ulongValue;
                    if (targetType == typeof(byte))
                        return (byte)ulongValue;
                    if (targetType == typeof(short))
                        return (short)ulongValue;
                    if (targetType == typeof(ushort))
                        return (ushort)ulongValue;
                    if (targetType == typeof(int))
                        return (int)ulongValue;
                    if (targetType == typeof(uint))
                        return (uint)ulongValue;
                    if (targetType == typeof(long))
                        return (long)ulongValue;
                    if (targetType == typeof(ulong))
                        return (ulong)ulongValue;
                    if (targetType == typeof(float))
                        return (float)ulongValue;
                    if (targetType == typeof(double))
                        return (double)ulongValue;
                    if (targetType == typeof(decimal))
                        return (decimal)ulongValue;
                    break;
                case float floatValue:
                    if (targetType == typeof(char))
                        return (char)floatValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)floatValue;
                    if (targetType == typeof(byte))
                        return (byte)floatValue;
                    if (targetType == typeof(short))
                        return (short)floatValue;
                    if (targetType == typeof(ushort))
                        return (ushort)floatValue;
                    if (targetType == typeof(int))
                        return (int)floatValue;
                    if (targetType == typeof(uint))
                        return (uint)floatValue;
                    if (targetType == typeof(long))
                        return (long)floatValue;
                    if (targetType == typeof(ulong))
                        return (ulong)floatValue;
                    if (targetType == typeof(float))
                        return (float)floatValue;
                    if (targetType == typeof(double))
                        return (double)floatValue;
                    if (targetType == typeof(decimal))
                        return (decimal)floatValue;
                    break;
                case double doubleValue:
                    if (targetType == typeof(char))
                        return (char)doubleValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)doubleValue;
                    if (targetType == typeof(byte))
                        return (byte)doubleValue;
                    if (targetType == typeof(short))
                        return (short)doubleValue;
                    if (targetType == typeof(ushort))
                        return (ushort)doubleValue;
                    if (targetType == typeof(int))
                        return (int)doubleValue;
                    if (targetType == typeof(uint))
                        return (uint)doubleValue;
                    if (targetType == typeof(long))
                        return (long)doubleValue;
                    if (targetType == typeof(ulong))
                        return (ulong)doubleValue;
                    if (targetType == typeof(float))
                        return (float)doubleValue;
                    if (targetType == typeof(double))
                        return (double)doubleValue;
                    if (targetType == typeof(decimal))
                        return (decimal)doubleValue;
                    break;
                case decimal decimalValue:
                    if (targetType == typeof(char))
                        return (char)decimalValue;
                    if (targetType == typeof(sbyte))
                        return (sbyte)decimalValue;
                    if (targetType == typeof(byte))
                        return (byte)decimalValue;
                    if (targetType == typeof(short))
                        return (short)decimalValue;
                    if (targetType == typeof(ushort))
                        return (ushort)decimalValue;
                    if (targetType == typeof(int))
                        return (int)decimalValue;
                    if (targetType == typeof(uint))
                        return (uint)decimalValue;
                    if (targetType == typeof(long))
                        return (long)decimalValue;
                    if (targetType == typeof(ulong))
                        return (ulong)decimalValue;
                    if (targetType == typeof(float))
                        return (float)decimalValue;
                    if (targetType == typeof(double))
                        return (double)decimalValue;
                    if (targetType == typeof(decimal))
                        return (decimal)decimalValue;
                    break;
            }

            return value;
        }

        public static bool TryCoerce(object value, Type targetType, out object result)
        {
            switch (value)
            {
                case char charValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)charValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)charValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)charValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)charValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)charValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)charValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)charValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)charValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)charValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)charValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)charValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)charValue;
                        return true;
                    }

                    break;
                case sbyte sbyteValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)sbyteValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)sbyteValue;
                        return true;
                    }

                    break;
                case byte byteValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)byteValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)byteValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)byteValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)byteValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)byteValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)byteValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)byteValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)byteValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)byteValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)byteValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)byteValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)byteValue;
                        return true;
                    }

                    break;
                case short shortValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)shortValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)shortValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)shortValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)shortValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)shortValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)shortValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)shortValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)shortValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)shortValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)shortValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)shortValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)shortValue;
                        return true;
                    }

                    break;
                case ushort ushortValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)ushortValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)ushortValue;
                        return true;
                    }

                    break;
                case int intValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)intValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)intValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)intValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)intValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)intValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)intValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)intValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)intValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)intValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)intValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)intValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)intValue;
                        return true;
                    }
                    if (targetType.IsEnum)
                    {
                      result = (int)intValue;
                      return true;
                    }

                    break;
                case uint uintValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)uintValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)uintValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)uintValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)uintValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)uintValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)uintValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)uintValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)uintValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)uintValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)uintValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)uintValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)uintValue;
                        return true;
                    }

                    break;
                case long longValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)longValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)longValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)longValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)longValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)longValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)longValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)longValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)longValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)longValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)longValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)longValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)longValue;
                        return true;
                    }

                    break;
                case ulong ulongValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)ulongValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)ulongValue;
                        return true;
                    }

                    break;
                case float floatValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)floatValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)floatValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)floatValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)floatValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)floatValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)floatValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)floatValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)floatValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)floatValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)floatValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)floatValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)floatValue;
                        return true;
                    }

                    break;
                case double doubleValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)doubleValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)doubleValue;
                        return true;
                    }
                    break;
                case decimal decimalValue:
                    if (targetType == typeof(char))
                    {
                        result = (char)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(sbyte))
                    {
                        result = (sbyte)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(byte))
                    {
                        result = (byte)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(short))
                    {
                        result = (short)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(ushort))
                    {
                        result = (ushort)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(int))
                    {
                        result = (int)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(uint))
                    {
                        result = (uint)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(long))
                    {
                        result = (long)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(ulong))
                    {
                        result = (ulong)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(float))
                    {
                        result = (float)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(double))
                    {
                        result = (double)decimalValue;
                        return true;
                    }
                    if (targetType == typeof(decimal))
                    {
                        result = (decimal)decimalValue;
                        return true;
                    }

                    break;
            }

            result = null;
            return false;
        }
    }
}
