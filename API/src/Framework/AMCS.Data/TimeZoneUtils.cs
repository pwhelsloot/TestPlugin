using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace AMCS.Data
{
  public static class TimeZoneUtils
  {
    public static ZonedDateTimePattern ZonedDateTimePattern => Configuration.Instance.ZonedDateTimePattern;

    public static LocalDatePattern LocalDatePattern => Configuration.Instance.LocalDatePattern;

    public static LocalTimePattern LocalTimePattern => Configuration.Instance.LocalTimePattern;

    public static DateTimeZone NeutralTimeZone => Configuration.Instance.TimeZoneConfiguration.NeutralTimeZone;

    public static TimeZoneMode TimeZoneMode => Configuration.Instance.TimeZoneConfiguration.TimeZoneMode;

    public static IDateTimeZoneProvider DateTimeZoneProvider => Configuration.Instance.TimeZoneConfiguration.DateTimeZoneProvider;

    public static OffsetDateTimePattern OffsetDateTimePattern => OffsetDateTimePattern.Rfc3339;

    public static InstantPattern InstantPattern => InstantPattern.ExtendedIso;

    public static PeriodPattern PeriodPattern => PeriodPattern.NormalizingIso;

    public static ZonedClock NeutralClock => Configuration.Instance.NeutralClock;

    public static TzdbDateTimeZoneSource DateTimeZoneSource => Configuration.Instance.TimeZoneConfiguration.DateTimeZoneSource;

    /// <summary>
    /// This method forces the static constructor to run
    /// </summary>
    public static void EnsureInitialized()
    {
      Configuration.EnsureInitialized();
    }

    private class Configuration
    {
      public static readonly Configuration Instance = new Configuration();

      private readonly ITimeZoneConfiguration timeZoneConfiguration;
      private readonly ZonedDateTimePattern zonedDateTimePattern;
      private readonly LocalDatePattern localDatePattern;
      private readonly LocalTimePattern localTimePattern;

      public ITimeZoneConfiguration TimeZoneConfiguration
      {
        get
        {
          if (timeZoneConfiguration == null)
            throw new InvalidOperationException("Time zones have not been configured");
          return timeZoneConfiguration;
        }
      }

      public ZonedDateTimePattern ZonedDateTimePattern
      {
        get
        {
          if (timeZoneConfiguration == null)
            throw new InvalidOperationException("Time zones have not been configured");
          return zonedDateTimePattern;
        }
      }

      public LocalDatePattern LocalDatePattern
      {
        get
        {
          if (timeZoneConfiguration == null)
            throw new InvalidOperationException("Time zones have not been configured");
          return localDatePattern;
        }
      }

      public LocalTimePattern LocalTimePattern
      {
        get
        {
          if (timeZoneConfiguration == null)
            throw new InvalidOperationException("Time zones have not been configured");
          return localTimePattern;
        }
      }

      public ZonedClock NeutralClock
      {
        get
        {
          if (timeZoneConfiguration == null)
            throw new InvalidOperationException("Time zones have not been configured");

          return SystemClock.Instance.InZone(timeZoneConfiguration.NeutralTimeZone);
        }
      }

      private Configuration()
      {
        if (DataServices.TryResolve<ITimeZoneConfiguration>(out var timeZoneConfiguration))
        {
          this.timeZoneConfiguration = timeZoneConfiguration;
          zonedDateTimePattern = ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<Z+HH:mm> z", timeZoneConfiguration.DateTimeZoneProvider);
          localDatePattern = LocalDatePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd");
          localTimePattern = LocalTimePattern.CreateWithInvariantCulture("HH':'mm':'ss;FFFFFFFFF");
        }
      }

      /// <summary>
      /// This method forces the static constructor to run
      /// Do not modify it.
      /// </summary>
      public static void EnsureInitialized()
      {
        _ = Instance;
      }
    }
  }
}
