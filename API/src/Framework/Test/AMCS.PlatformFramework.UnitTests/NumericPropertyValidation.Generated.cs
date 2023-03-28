using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.UnitTests
{
  partial class NumericPropertyValidation
  {
    public class PropertyObject
    {
      public static PropertyObject WithValue(int value)
      {
        var obj = new PropertyObject();

        obj.SByteProperty = (SByte)value;
        obj.ByteProperty = (Byte)value;
        obj.Int16Property = (Int16)value;
        obj.UInt16Property = (UInt16)value;
        obj.Int32Property = (Int32)value;
        obj.UInt32Property = (UInt32)value;
        obj.Int64Property = (Int64)value;
        obj.UInt64Property = (UInt64)value;
        obj.SingleProperty = (Single)value;
        obj.DoubleProperty = (Double)value;
        obj.DecimalProperty = (Decimal)value;

        return obj;
      }

      public SByte SByteProperty { get; set; }
      public Byte ByteProperty { get; set; }
      public Int16 Int16Property { get; set; }
      public UInt16 UInt16Property { get; set; }
      public Int32 Int32Property { get; set; }
      public UInt32 UInt32Property { get; set; }
      public Int64 Int64Property { get; set; }
      public UInt64 UInt64Property { get; set; }
      public Single SingleProperty { get; set; }
      public Double DoubleProperty { get; set; }
      public Decimal DecimalProperty { get; set; }
    }
  }
}
