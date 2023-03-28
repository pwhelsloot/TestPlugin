using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace AMCS.Data.Configuration.TimeZones
{
  public class SystemNeutralTimeZoneIdProvider : INeutralTimeZoneIdProvider
  {
    public string GetNeutralTimeZoneId() => DateTimeZoneProviders.Tzdb.GetSystemDefault().Id;
  }
}
