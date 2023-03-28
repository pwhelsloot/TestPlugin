using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.Data.Configuration.TimeZones
{
  public class EmbeddedDateTimeZoneProviderProvider : IDateTimeZoneProviderProvider
  {
    public IDateTimeZoneProvider DateTimeZoneProvider => DateTimeZoneProviders.Tzdb;

    public TzdbDateTimeZoneSource DateTimeZoneSource => TzdbDateTimeZoneSource.Default;

    public event EventHandler DateTimeZoneProviderChanged;

    protected virtual void OnDateTimeZoneProviderChanged()
    {
      DateTimeZoneProviderChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
    }
  }
}
