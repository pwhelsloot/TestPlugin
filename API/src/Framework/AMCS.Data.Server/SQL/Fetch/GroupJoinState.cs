using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Fetch
{
  internal enum GroupJoinState
  {
    Excluded,
    InnerJoin,
    LeftJoin
  }
}
