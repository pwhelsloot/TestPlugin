using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  partial class Cast
  {
    private static Dictionary<Type, Cast> BuildCasts()
    {
      var casts = new Dictionary<Type, Cast>();

      casts.Add(typeof(Char), new CharCast());
      casts.Add(typeof(SByte), new SByteCast());
      casts.Add(typeof(Byte), new ByteCast());
      casts.Add(typeof(Int16), new Int16Cast());
      casts.Add(typeof(UInt16), new UInt16Cast());
      casts.Add(typeof(Int32), new Int32Cast());
      casts.Add(typeof(UInt32), new UInt32Cast());
      casts.Add(typeof(Int64), new Int64Cast());
      casts.Add(typeof(UInt64), new UInt64Cast());

      return casts;
    }

    private class CharCast : Cast
    {
      public override Type ReturnType => typeof(Char);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (Char)value;
      }
    }

    private class SByteCast : Cast
    {
      public override Type ReturnType => typeof(SByte);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (SByte)value;
      }
    }

    private class ByteCast : Cast
    {
      public override Type ReturnType => typeof(Byte);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (Byte)value;
      }
    }

    private class Int16Cast : Cast
    {
      public override Type ReturnType => typeof(Int16);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (Int16)value;
      }
    }

    private class UInt16Cast : Cast
    {
      public override Type ReturnType => typeof(UInt16);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (UInt16)value;
      }
    }

    private class Int32Cast : Cast
    {
      public override Type ReturnType => typeof(Int32);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (Int32)value;
      }
    }

    private class UInt32Cast : Cast
    {
      public override Type ReturnType => typeof(UInt32);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (UInt32)value;
      }
    }

    private class Int64Cast : Cast
    {
      public override Type ReturnType => typeof(Int64);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (Int64)value;
      }
    }

    private class UInt64Cast : Cast
    {
      public override Type ReturnType => typeof(UInt64);

      public override object GetValue(object value)
      {
        if (value == null)
          return null;

        return (UInt64)value;
      }
    }
  }
}
