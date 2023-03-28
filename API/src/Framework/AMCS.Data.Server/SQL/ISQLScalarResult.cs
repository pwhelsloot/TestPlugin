using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLScalarResult
  {
    object Value { get; }

    SqlCommand Command { get; }
  }
}
