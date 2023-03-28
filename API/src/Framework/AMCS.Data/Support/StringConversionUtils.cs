namespace AMCS.Data.Support
{
  using System;
  using System.Globalization;
  using NodaTime;

  public static class UntypedStringConversionUtils
  {
    /// <summary>
    /// Helper method for converting string objects to a target type
    /// Refer to https://confluence.amcsgroup.com/display/APD/Data+types+and+serialization for full list of supported types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="untypedValue"></param>
    /// <returns></returns>
    public static T ParseUntypedString<T>(object untypedValue)
    {
      TimeZoneUtils.EnsureInitialized();

      if (untypedValue is string value)
      {
        if (typeof(T) == typeof(string))
          return Parse<T>(untypedValue);

        if (typeof(T) == typeof(bool))
          return Parse<T>(bool.Parse(value));

        if (typeof(T) == typeof(Guid))
          return Parse<T>(Guid.Parse(value));

        if (typeof(T) == typeof(int))
          return Parse<T>(int.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture));

        if (typeof(T) == typeof(decimal))
          return Parse<T>(decimal.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture));

        if (typeof(T) == typeof(DateTime))
          return Parse<T>(JsonUtil.ParseDateTime(value));

        if (typeof(T) == typeof(DateTimeOffset))
          return Parse<T>(JsonUtil.ParseDateTimeOffset(value));

        if (typeof(T) == typeof(byte[]) && value is string stringValue)
          return Parse<T>(Convert.FromBase64String(stringValue));

        if (typeof(T) == typeof(Instant))
          return Parse<T>(TimeZoneUtils.InstantPattern.Parse(value).GetValueOrThrow());

        if (typeof(T) == typeof(Period))
          return Parse<T>(TimeZoneUtils.PeriodPattern.Parse(value).GetValueOrThrow());

        if (typeof(T) == typeof(LocalDate))
          return Parse<T>(TimeZoneUtils.LocalDatePattern.Parse(value).GetValueOrThrow());

        if (typeof(T) == typeof(LocalTime))
          return Parse<T>(TimeZoneUtils.LocalTimePattern.Parse(value).GetValueOrThrow());

        if (typeof(T) == typeof(OffsetDateTime))
          return Parse<T>(TimeZoneUtils.OffsetDateTimePattern.Parse(value).GetValueOrThrow());

        if (typeof(T) == typeof(ZonedDateTime))
          return Parse<T>(TimeZoneUtils.ZonedDateTimePattern.Parse(value).GetValueOrThrow());

        if (typeof(T).IsEnum && int.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _))
          return Parse<T>(Enum.ToObject(typeof(T), value));
      }

      return (T)ValueCoercion.Coerce(untypedValue, typeof(T));
    }

    private static T Parse<T>(object value)
    {
      return (T)value;
    }
  }
}