using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal interface ISQLCommandFactory
  {
    string DefaultSearchResultId { get; }

    SqlCommand CreateCommand(SQLDataSession dataSession);
  }
}
