using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLPreparedParameter
  {
    public string ColumnName { get; }

    public SqlParameter Parameter { get; }

    public SQLPreparedParameter(string columnName, SqlParameter parameter)
    {
      ColumnName = columnName; ;
      Parameter = parameter;
    }
  }
}
