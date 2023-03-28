using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class UnparsedNumber
  {
    public string Text { get; }
    public Type Type { get; }
    public NumberStyles NumberStyles { get; }

    public UnparsedNumber(string text, Type type, NumberStyles numberStyles)
    {
      if (text == null)
        throw new ArgumentNullException(nameof(text));
      if (type == null)
        throw new ArgumentNullException(nameof(type));

      Text = text;
      Type = type;
      NumberStyles = numberStyles;
    }

    public object Parse()
    {
      try
      {
        if (Type == typeof(int))
          return int.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
        if (Type == typeof(uint))
          return uint.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
        if (Type == typeof(long))
          return long.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
        if (Type == typeof(ulong))
          return ulong.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
        if (Type == typeof(float))
          return float.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
        if (Type == typeof(double))
          return double.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
        if (Type == typeof(decimal))
          return decimal.Parse(Text, NumberStyles, CultureInfo.InvariantCulture);
      }
      catch (FormatException ex)
      {
        throw new ParseException($"Cannot parse number '{Text}'", ex);
      }

      throw new ParseException($"Unexpected type {Type} while parsing number");
    }
  }
}
