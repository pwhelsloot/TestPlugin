using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration.TimeZones
{
  public class FixedNeutralTimeZoneIdProvider : INeutralTimeZoneIdProvider
  {
    private readonly string neutralTimeZoneId;

    public string GetNeutralTimeZoneId() => neutralTimeZoneId;

    public FixedNeutralTimeZoneIdProvider(string neutralTimeZoneId)
    {
      this.neutralTimeZoneId = neutralTimeZoneId;
    }
  }
}
