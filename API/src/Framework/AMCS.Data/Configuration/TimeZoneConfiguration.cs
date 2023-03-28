using System;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Configuration.TimeZones.Moment;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.Data.Configuration
{
  public class TimeZoneConfiguration : ITimeZoneConfiguration, IDisposable
  {
    private IDateTimeZoneProviderProvider dateTimeZoneProviderProvider;
    private volatile DateTimeZone neutralTimeZone;
    private volatile IDateTimeZoneProvider dateTimeZoneProvider;
    private volatile TzdbDateTimeZoneSource dateTimeZoneSource;
    private bool disposed;

    public DateTimeZone NeutralTimeZone
    {
      get
      {
        if (StrictMode.IsDisableNeutralTimeZone)
          throw new StrictModeException(
            "Neutral Time Zone cannot be used in conjunction with Strict Mode with Disable Neutral Time Zone");
     
        return neutralTimeZone;
      }
    }

    public IDateTimeZoneProvider DateTimeZoneProvider => dateTimeZoneProvider;

    public TzdbDateTimeZoneSource DateTimeZoneSource => dateTimeZoneSource;

    public TimeZoneMode TimeZoneMode { get; }

    public IMomentTimeZoneDatabaseCache MomentTimeZoneDatabaseCache { get; }

    public TimeZoneConfiguration(IDateTimeZoneProviderProvider dateTimeZoneProviderProvider)
      : this(null, dateTimeZoneProviderProvider, TimeZoneMode.Enforced)
    {
    }
    
    public TimeZoneConfiguration(
      INeutralTimeZoneIdProvider neutralTimeZoneIdProvider, 
      IDateTimeZoneProviderProvider dateTimeZoneProviderProvider)
      : this(neutralTimeZoneIdProvider, dateTimeZoneProviderProvider, TimeZoneMode.Enforced)
    {
    }

    public TimeZoneConfiguration(
      INeutralTimeZoneIdProvider neutralTimeZoneIdProvider, 
      IDateTimeZoneProviderProvider dateTimeZoneProviderProvider, 
      TimeZoneMode timeZoneMode)
    {
      TimeZoneMode = timeZoneMode;

      this.dateTimeZoneProviderProvider = dateTimeZoneProviderProvider;
      
      // We resolve the neutral time zone eagerly to ensure it's available immediately.
      var neutralTimeZoneId = neutralTimeZoneIdProvider?.GetNeutralTimeZoneId();
      if (!StrictMode.IsDisableNeutralTimeZone && string.IsNullOrEmpty(neutralTimeZoneId))
        throw new InvalidOperationException("Neutral time zone could not be resolved");

      MomentTimeZoneDatabaseCache = new MomentTimeZoneDatabaseCache(dateTimeZoneProviderProvider);

      dateTimeZoneProviderProvider.DateTimeZoneProviderChanged += (s, e) => ReloadDateTimeZoneProvider();

      ReloadDateTimeZoneProvider();

      void ReloadDateTimeZoneProvider()
      {
        dateTimeZoneProvider = dateTimeZoneProviderProvider.DateTimeZoneProvider;
        dateTimeZoneSource = dateTimeZoneProviderProvider.DateTimeZoneSource;
        
        if(!string.IsNullOrWhiteSpace(neutralTimeZoneId))
          neutralTimeZone = dateTimeZoneProvider[neutralTimeZoneId];
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (dateTimeZoneProviderProvider != null)
        {
          dateTimeZoneProviderProvider.Dispose();
          dateTimeZoneProviderProvider = null;
        }

        disposed = true;
      }
    }
  }
}
