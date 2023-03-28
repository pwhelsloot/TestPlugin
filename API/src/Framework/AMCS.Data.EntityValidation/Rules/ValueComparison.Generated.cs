using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal static partial class ValueComparison
  {
    private static void AddGeneratedComparers(Dictionary<(Type, Type), IComparer> comparers)
    {
      comparers.Add((typeof(SByte), typeof(SByte)), new SByteSByteComparer());
      comparers.Add((typeof(SByte), typeof(Byte)), new SByteByteComparer());
      comparers.Add((typeof(SByte), typeof(Int16)), new SByteInt16Comparer());
      comparers.Add((typeof(SByte), typeof(UInt16)), new SByteUInt16Comparer());
      comparers.Add((typeof(SByte), typeof(Int32)), new SByteInt32Comparer());
      comparers.Add((typeof(SByte), typeof(UInt32)), new SByteUInt32Comparer());
      comparers.Add((typeof(SByte), typeof(Int64)), new SByteInt64Comparer());
      comparers.Add((typeof(SByte), typeof(Single)), new SByteSingleComparer());
      comparers.Add((typeof(SByte), typeof(Double)), new SByteDoubleComparer());
      comparers.Add((typeof(SByte), typeof(Decimal)), new SByteDecimalComparer());
      comparers.Add((typeof(Byte), typeof(SByte)), new ByteSByteComparer());
      comparers.Add((typeof(Byte), typeof(Byte)), new ByteByteComparer());
      comparers.Add((typeof(Byte), typeof(Int16)), new ByteInt16Comparer());
      comparers.Add((typeof(Byte), typeof(UInt16)), new ByteUInt16Comparer());
      comparers.Add((typeof(Byte), typeof(Int32)), new ByteInt32Comparer());
      comparers.Add((typeof(Byte), typeof(UInt32)), new ByteUInt32Comparer());
      comparers.Add((typeof(Byte), typeof(Int64)), new ByteInt64Comparer());
      comparers.Add((typeof(Byte), typeof(UInt64)), new ByteUInt64Comparer());
      comparers.Add((typeof(Byte), typeof(Single)), new ByteSingleComparer());
      comparers.Add((typeof(Byte), typeof(Double)), new ByteDoubleComparer());
      comparers.Add((typeof(Byte), typeof(Decimal)), new ByteDecimalComparer());
      comparers.Add((typeof(Int16), typeof(SByte)), new Int16SByteComparer());
      comparers.Add((typeof(Int16), typeof(Byte)), new Int16ByteComparer());
      comparers.Add((typeof(Int16), typeof(Int16)), new Int16Int16Comparer());
      comparers.Add((typeof(Int16), typeof(UInt16)), new Int16UInt16Comparer());
      comparers.Add((typeof(Int16), typeof(Int32)), new Int16Int32Comparer());
      comparers.Add((typeof(Int16), typeof(UInt32)), new Int16UInt32Comparer());
      comparers.Add((typeof(Int16), typeof(Int64)), new Int16Int64Comparer());
      comparers.Add((typeof(Int16), typeof(Single)), new Int16SingleComparer());
      comparers.Add((typeof(Int16), typeof(Double)), new Int16DoubleComparer());
      comparers.Add((typeof(Int16), typeof(Decimal)), new Int16DecimalComparer());
      comparers.Add((typeof(UInt16), typeof(SByte)), new UInt16SByteComparer());
      comparers.Add((typeof(UInt16), typeof(Byte)), new UInt16ByteComparer());
      comparers.Add((typeof(UInt16), typeof(Int16)), new UInt16Int16Comparer());
      comparers.Add((typeof(UInt16), typeof(UInt16)), new UInt16UInt16Comparer());
      comparers.Add((typeof(UInt16), typeof(Int32)), new UInt16Int32Comparer());
      comparers.Add((typeof(UInt16), typeof(UInt32)), new UInt16UInt32Comparer());
      comparers.Add((typeof(UInt16), typeof(Int64)), new UInt16Int64Comparer());
      comparers.Add((typeof(UInt16), typeof(UInt64)), new UInt16UInt64Comparer());
      comparers.Add((typeof(UInt16), typeof(Single)), new UInt16SingleComparer());
      comparers.Add((typeof(UInt16), typeof(Double)), new UInt16DoubleComparer());
      comparers.Add((typeof(UInt16), typeof(Decimal)), new UInt16DecimalComparer());
      comparers.Add((typeof(Int32), typeof(SByte)), new Int32SByteComparer());
      comparers.Add((typeof(Int32), typeof(Byte)), new Int32ByteComparer());
      comparers.Add((typeof(Int32), typeof(Int16)), new Int32Int16Comparer());
      comparers.Add((typeof(Int32), typeof(UInt16)), new Int32UInt16Comparer());
      comparers.Add((typeof(Int32), typeof(Int32)), new Int32Int32Comparer());
      comparers.Add((typeof(Int32), typeof(UInt32)), new Int32UInt32Comparer());
      comparers.Add((typeof(Int32), typeof(Int64)), new Int32Int64Comparer());
      comparers.Add((typeof(Int32), typeof(Single)), new Int32SingleComparer());
      comparers.Add((typeof(Int32), typeof(Double)), new Int32DoubleComparer());
      comparers.Add((typeof(Int32), typeof(Decimal)), new Int32DecimalComparer());
      comparers.Add((typeof(UInt32), typeof(SByte)), new UInt32SByteComparer());
      comparers.Add((typeof(UInt32), typeof(Byte)), new UInt32ByteComparer());
      comparers.Add((typeof(UInt32), typeof(Int16)), new UInt32Int16Comparer());
      comparers.Add((typeof(UInt32), typeof(UInt16)), new UInt32UInt16Comparer());
      comparers.Add((typeof(UInt32), typeof(Int32)), new UInt32Int32Comparer());
      comparers.Add((typeof(UInt32), typeof(UInt32)), new UInt32UInt32Comparer());
      comparers.Add((typeof(UInt32), typeof(Int64)), new UInt32Int64Comparer());
      comparers.Add((typeof(UInt32), typeof(UInt64)), new UInt32UInt64Comparer());
      comparers.Add((typeof(UInt32), typeof(Single)), new UInt32SingleComparer());
      comparers.Add((typeof(UInt32), typeof(Double)), new UInt32DoubleComparer());
      comparers.Add((typeof(UInt32), typeof(Decimal)), new UInt32DecimalComparer());
      comparers.Add((typeof(Int64), typeof(SByte)), new Int64SByteComparer());
      comparers.Add((typeof(Int64), typeof(Byte)), new Int64ByteComparer());
      comparers.Add((typeof(Int64), typeof(Int16)), new Int64Int16Comparer());
      comparers.Add((typeof(Int64), typeof(UInt16)), new Int64UInt16Comparer());
      comparers.Add((typeof(Int64), typeof(Int32)), new Int64Int32Comparer());
      comparers.Add((typeof(Int64), typeof(UInt32)), new Int64UInt32Comparer());
      comparers.Add((typeof(Int64), typeof(Int64)), new Int64Int64Comparer());
      comparers.Add((typeof(Int64), typeof(Single)), new Int64SingleComparer());
      comparers.Add((typeof(Int64), typeof(Double)), new Int64DoubleComparer());
      comparers.Add((typeof(Int64), typeof(Decimal)), new Int64DecimalComparer());
      comparers.Add((typeof(UInt64), typeof(Byte)), new UInt64ByteComparer());
      comparers.Add((typeof(UInt64), typeof(UInt16)), new UInt64UInt16Comparer());
      comparers.Add((typeof(UInt64), typeof(UInt32)), new UInt64UInt32Comparer());
      comparers.Add((typeof(UInt64), typeof(UInt64)), new UInt64UInt64Comparer());
      comparers.Add((typeof(UInt64), typeof(Single)), new UInt64SingleComparer());
      comparers.Add((typeof(UInt64), typeof(Double)), new UInt64DoubleComparer());
      comparers.Add((typeof(UInt64), typeof(Decimal)), new UInt64DecimalComparer());
      comparers.Add((typeof(Single), typeof(SByte)), new SingleSByteComparer());
      comparers.Add((typeof(Single), typeof(Byte)), new SingleByteComparer());
      comparers.Add((typeof(Single), typeof(Int16)), new SingleInt16Comparer());
      comparers.Add((typeof(Single), typeof(UInt16)), new SingleUInt16Comparer());
      comparers.Add((typeof(Single), typeof(Int32)), new SingleInt32Comparer());
      comparers.Add((typeof(Single), typeof(UInt32)), new SingleUInt32Comparer());
      comparers.Add((typeof(Single), typeof(Int64)), new SingleInt64Comparer());
      comparers.Add((typeof(Single), typeof(UInt64)), new SingleUInt64Comparer());
      comparers.Add((typeof(Single), typeof(Single)), new SingleSingleComparer());
      comparers.Add((typeof(Single), typeof(Double)), new SingleDoubleComparer());
      comparers.Add((typeof(Double), typeof(SByte)), new DoubleSByteComparer());
      comparers.Add((typeof(Double), typeof(Byte)), new DoubleByteComparer());
      comparers.Add((typeof(Double), typeof(Int16)), new DoubleInt16Comparer());
      comparers.Add((typeof(Double), typeof(UInt16)), new DoubleUInt16Comparer());
      comparers.Add((typeof(Double), typeof(Int32)), new DoubleInt32Comparer());
      comparers.Add((typeof(Double), typeof(UInt32)), new DoubleUInt32Comparer());
      comparers.Add((typeof(Double), typeof(Int64)), new DoubleInt64Comparer());
      comparers.Add((typeof(Double), typeof(UInt64)), new DoubleUInt64Comparer());
      comparers.Add((typeof(Double), typeof(Single)), new DoubleSingleComparer());
      comparers.Add((typeof(Double), typeof(Double)), new DoubleDoubleComparer());
      comparers.Add((typeof(Decimal), typeof(SByte)), new DecimalSByteComparer());
      comparers.Add((typeof(Decimal), typeof(Byte)), new DecimalByteComparer());
      comparers.Add((typeof(Decimal), typeof(Int16)), new DecimalInt16Comparer());
      comparers.Add((typeof(Decimal), typeof(UInt16)), new DecimalUInt16Comparer());
      comparers.Add((typeof(Decimal), typeof(Int32)), new DecimalInt32Comparer());
      comparers.Add((typeof(Decimal), typeof(UInt32)), new DecimalUInt32Comparer());
      comparers.Add((typeof(Decimal), typeof(Int64)), new DecimalInt64Comparer());
      comparers.Add((typeof(Decimal), typeof(UInt64)), new DecimalUInt64Comparer());
      comparers.Add((typeof(Decimal), typeof(Decimal)), new DecimalDecimalComparer());
    }

    private class SByteSByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteUInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteUInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteSingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteDoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SByteDecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (SByte)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteSByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteUInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteUInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteUInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteSingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteDoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class ByteDecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Byte)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16SByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16ByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16Int16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16UInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16Int32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16UInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16Int64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16SingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int16DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int16)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16SByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16ByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16Int16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16UInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16Int32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16UInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16Int64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16UInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16SingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt16DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt16)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32SByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32ByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32Int16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32UInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32Int32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32UInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32Int64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32SingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int32DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int32)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32SByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32ByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32Int16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32UInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32Int32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32UInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32Int64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32UInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32SingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt32DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt32)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64SByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64ByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64Int16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64UInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64Int32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64UInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64Int64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64SingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class Int64DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Int64)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64ByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64UInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64UInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64UInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64SingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class UInt64DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (UInt64)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleSByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleUInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleUInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleUInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleSingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class SingleDoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Single)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleSByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleUInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleUInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleUInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleSingleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (Single)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DoubleDoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Double)x;
            var right = (Double)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalSByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (SByte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalByteComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (Byte)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (Int16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalUInt16Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (UInt16)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (Int32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalUInt32Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (UInt32)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (Int64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalUInt64Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (UInt64)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

    private class DecimalDecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;
            
            var left = (Decimal)x;
            var right = (Decimal)y;

            if (left < right)
                return -1;
            if (left > right)
                return 1;
            return 0;
        }
    }

  }
}
