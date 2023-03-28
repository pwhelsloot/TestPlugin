using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration.TimeZones
{
  public interface INeutralTimeZoneIdProvider
  {
    string GetNeutralTimeZoneId();
  }
}
