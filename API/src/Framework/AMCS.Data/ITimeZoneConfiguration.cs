using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration.TimeZones.Moment;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.Data
{
  public interface ITimeZoneConfiguration
  {
    DateTimeZone NeutralTimeZone { get; }

    IDateTimeZoneProvider DateTimeZoneProvider { get; }

    TzdbDateTimeZoneSource DateTimeZoneSource { get; }

    TimeZoneMode TimeZoneMode { get; }

    IMomentTimeZoneDatabaseCache MomentTimeZoneDatabaseCache { get; }
  }
}
