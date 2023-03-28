using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace AMCS.Data.Entity
{
  public static class DateStorageConverterFactory
  {
    public static IDateStorageConverter CreateConverter(Type propertyType, DateStorage dateStorage)
    {
      // These storage converters ignore whether the target types are nullable. We expect
      // the incoming value to be set regardless of whether the underlying property type
      // is nullable.
      propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

      switch (dateStorage)
      {
        case DateStorage.UTC:
          if (propertyType == typeof(Instant))
            return new UTCInstantConverter();
          if (propertyType == typeof(ZonedDateTime))
            return new UTCZonedConverter();
          break;
        case DateStorage.Offset:
          if (propertyType == typeof(OffsetDateTime))
            return new OffsetConverter();
          break;
        case DateStorage.Neutral:
          if (propertyType == typeof(Instant))
            return new NeutralInstantConverter();
          if (propertyType == typeof(ZonedDateTime))
            return new NeutralZonedConverter();
          break;
        case DateStorage.Zoned:
          if (propertyType == typeof(ZonedDateTime))
            return new ZonedConverter();
          break;
        case DateStorage.Date:
          if (propertyType == typeof(LocalDate))
            return new LocalDateConverter();
          break;
        case DateStorage.Time:
          if (propertyType == typeof(LocalTime))
            return new LocalTimeConverter();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(dateStorage), dateStorage, null);
      }

      throw new DateConversionException($"Cannot construct a date conversion for data storage {dateStorage} and property type {propertyType}");
    }

    private abstract class DateStorageConverter<TStorage, TNative> : IDateStorageConverter
    {
      public object FromStorage(object value, DateTimeZone timeZone)
      {
        if (!(value is TStorage storageValue))
          throw new DateConversionException($"Cannot convert value of type {value.GetType()} with {GetType().Name}");

        return FromStorage(storageValue, timeZone);
      }

      protected abstract TNative FromStorage(TStorage value, DateTimeZone timeZone);

      public object ToStorage(object value, DateTimeZone timeZone)
      {
        if (!(value is TNative nativeValue))
          throw new DateConversionException($"Cannot convert value of type {value.GetType()} with {GetType().Name}");

        return ToStorage(nativeValue, timeZone);
      }

      protected abstract TStorage ToStorage(TNative value, DateTimeZone timeZone);
    }

    private class UTCInstantConverter : DateStorageConverter<DateTime, Instant>
    {
      protected override Instant FromStorage(DateTime value, DateTimeZone timeZone)
      {
        return Instant.FromDateTimeUtc(value);
      }

      protected override DateTime ToStorage(Instant value, DateTimeZone timeZone)
      {
        return value.ToDateTimeUtc();
      }
    }

    private class UTCZonedConverter : DateStorageConverter<DateTime, ZonedDateTime>
    {
      protected override ZonedDateTime FromStorage(DateTime value, DateTimeZone timeZone)
      {
        return Instant.FromDateTimeUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc)).InUtc();
      }

      protected override DateTime ToStorage(ZonedDateTime value, DateTimeZone timeZone)
      {
        return value.ToDateTimeUtc();
      }
    }

    private class OffsetConverter : DateStorageConverter<DateTimeOffset, OffsetDateTime>
    {
      protected override OffsetDateTime FromStorage(DateTimeOffset value, DateTimeZone timeZone)
      {
        return OffsetDateTime.FromDateTimeOffset(value);
      }

      protected override DateTimeOffset ToStorage(OffsetDateTime value, DateTimeZone timeZone)
      {
        return value.ToDateTimeOffset();
      }
    }

    private class NeutralInstantConverter : DateStorageConverter<DateTime, Instant>
    {
      protected override Instant FromStorage(DateTime value, DateTimeZone timeZone)
      {
        return TimeZoneUtils.NeutralTimeZone.AtLeniently(LocalDateTime.FromDateTime(value)).ToInstant();
      }

      protected override DateTime ToStorage(Instant value, DateTimeZone timeZone)
      {
        return value.InZone(TimeZoneUtils.NeutralTimeZone).LocalDateTime.ToDateTimeUnspecified();
      }
    }

    private class NeutralZonedConverter : DateStorageConverter<DateTime, ZonedDateTime>
    {
      protected override ZonedDateTime FromStorage(DateTime value, DateTimeZone timeZone)
      {
        return TimeZoneUtils.NeutralTimeZone.AtLeniently(LocalDateTime.FromDateTime(value));
      }

      protected override DateTime ToStorage(ZonedDateTime value, DateTimeZone timeZone)
      {
        var neutralTimeZone = TimeZoneUtils.NeutralTimeZone;

        if (value.Zone != neutralTimeZone)
          value = value.WithZone(neutralTimeZone);

        return value.LocalDateTime.ToDateTimeUnspecified();
      }
    }

    private class ZonedConverter : DateStorageConverter<DateTime, ZonedDateTime>
    {
      protected override ZonedDateTime FromStorage(DateTime value, DateTimeZone timeZone)
      {
        if (timeZone == null)
          throw new DateConversionException("Missing time zone while loading a zoned date time");

        return timeZone.AtLeniently(LocalDateTime.FromDateTime(value));
      }

      protected override DateTime ToStorage(ZonedDateTime value, DateTimeZone timeZone)
      {
        if (value.Zone != timeZone)
          value = value.WithZone(timeZone);

        return value.LocalDateTime.ToDateTimeUnspecified();
      }
    }

    private class LocalDateConverter : DateStorageConverter<DateTime, LocalDate>
    {
      protected override LocalDate FromStorage(DateTime value, DateTimeZone timeZone)
      {
        return LocalDate.FromDateTime(value);
      }

      protected override DateTime ToStorage(LocalDate value, DateTimeZone timeZone)
      {
        return value.ToDateTimeUnspecified();
      }
    }

    private class LocalTimeConverter : DateStorageConverter<DateTime, LocalTime>
    {
      private static readonly DateTime Offset = new DateTime(1970, 1, 1);

      protected override LocalTime FromStorage(DateTime value, DateTimeZone timeZone)
      {
        return LocalTime.FromTicksSinceMidnight(value.TimeOfDay.Ticks);
      }

      protected override DateTime ToStorage(LocalTime value, DateTimeZone timeZone)
      {
        return Offset + new TimeSpan(value.TickOfDay);
      }
    }
  }
}
