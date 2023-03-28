using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.Data.Configuration.TimeZones
{
  public interface IDateTimeZoneProviderProvider : IDisposable
  {
    IDateTimeZoneProvider DateTimeZoneProvider { get; }

    TzdbDateTimeZoneSource DateTimeZoneSource { get; }

    event EventHandler DateTimeZoneProviderChanged;
  }
}
