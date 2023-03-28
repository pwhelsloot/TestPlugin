using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLScalarResult : ISQLScalarResult
  {
    public static readonly SQLScalarResult Empty = new SQLScalarResult(null, null);

    public object Value { get; }

    public SqlCommand Command { get; }

    public SQLScalarResult(object value, SqlCommand command)
    {
      this.Value = value;
      Command = command;
    }
  }
}
