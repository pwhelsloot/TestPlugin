using AMCS.Data.Server.SQL.Fetch;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLStoredProcedureCommandBuilder : SQLParameterizedCommandBuilder<ISQLQuery>, ISQLQuery
  {
    private readonly string name;

    public SQLStoredProcedureCommandBuilder(SQLDataSession dataSession, FetchInfo fetchInfo, string name)
      : base(dataSession, fetchInfo)
    {
      this.name = name;
    }

    protected override ISQLCommandFactory CreateCommandFactory()
    {
      return new SQLCommandFactory(
        CommandType.StoredProcedure,
        name,
        CreateParameters());
    }

    public ISQLReadable Execute()
    {
      return ExecuteReader();
    }

    ISQLExecutableResult ISQLQuery.ExecuteNonQuery()
    {
      DataSession.Metrics?.StoredProcedureNonQueryBegin(DataSession.Connection);

      ISQLExecutableResult result;

      try
      {
        result = ExecuteNonQuery();

        DataSession.Metrics?.StoredProcedureNonQueryEnd(DataSession.Connection, result.Command, result.RowsAffected);
      }
      catch (Exception ex)
      {
        DataSession.Metrics?.StoredProcedureNonQueryEnd(DataSession.Connection, null, 0, ex);
        throw;
      }

      return result;
    }
  }
}
