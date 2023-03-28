using AMCS.Data.Server.SQL.Fetch;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLQueryCommandBuilder : SQLParameterizedCommandBuilder<ISQLQuery>, ISQLQuery
  {
    private readonly string sql;

    public SQLQueryCommandBuilder(SQLDataSession dataSession, FetchInfo fetchInfo, string sql)
      : base(dataSession, fetchInfo)
    {
      this.sql = sql;
    }

    protected override ISQLCommandFactory CreateCommandFactory()
    {
      var parameters = CreateParameters();

      // Expand the value of literals if a query contains the {=parameter} construct.
      string translatedSql = SQLLiteralExpander.Translate(sql, parameters);
      // Fix issues with Azure compatibility.
      translatedSql = SQLAzureCompatibility.Translate(translatedSql);

      return new SQLCommandFactory(
        CommandType.Text,
        translatedSql,
        parameters);
    }

    public ISQLReadable Execute()
    {
      return ExecuteReader();
    }

    ISQLExecutableResult ISQLQuery.ExecuteNonQuery()
    {
      DataSession.Metrics?.DynamicNonQueryBegin(DataSession.Connection);

      ISQLExecutableResult result;

      try
      {
        result = ExecuteNonQuery();

        DataSession.Metrics?.DynamicNonQueryEnd(DataSession.Connection, result.Command, result.RowsAffected);
      }
      catch (Exception ex)
      {
        DataSession.Metrics?.DynamicNonQueryEnd(DataSession.Connection, null, 0, ex);
        throw;
      }

      return result;
    }
  }
}
