using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal enum SQLReaderMode
  {
    First,
    FirstOrDefault,
    FirstScalar,
    FirstOrDefaultScalar,
    Single,
    SingleOrDefault,
    SingleScalar,
    SingleOrDefaultScalar
  }
}
