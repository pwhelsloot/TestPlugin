using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLExecutableResult
  {
    int RowsAffected { get; }

    SqlCommand Command { get;  }

    object Get(string parameterName);

    T Get<T>(string parameterName);
  }
}
