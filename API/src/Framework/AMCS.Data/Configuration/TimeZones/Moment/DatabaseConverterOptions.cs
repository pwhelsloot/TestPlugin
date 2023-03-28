using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration.TimeZones.Moment
{
  [Flags]
  public enum DatabaseConverterOptions
  {
    None = 0,
    TrimIntervals = 1,
    IncludeCountries = 2
  }
}
