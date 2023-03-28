namespace AMCS.Data.Entity.String
{
  using System;
  using System.Reflection;
  using Util.Extension;

  public static class EnumStringValueHelper
  {
    /// <summary>
    /// Gets the string value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string GetStringValue(System.Enum value)
    {
      string output = null;
      Type type = value.GetType();

      FieldInfo fi = type.GetField(value.ToString());
      StringValueAttribute[] attrs =
        fi.GetCustomAttributes(typeof(StringValueAttribute),
          false) as StringValueAttribute[];
      if (attrs.Length > 0)
      {
        output = attrs[0].StringValue;
      }

      return output;
    }
  }
}
