using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  internal class SQLExpression : IExpression
  {
    public SQLFragment SQL { get; }

    public object[] Values { get; }

    public SQLExpression(SQLFragment sql, params object[] values)
    {
      SQL = sql;
      Values = values;
    }
  }
}
