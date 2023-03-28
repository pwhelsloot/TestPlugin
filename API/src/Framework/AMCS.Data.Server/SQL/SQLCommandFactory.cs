using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLCommandFactory : ISQLCommandFactory
  {
    private readonly CommandType commandType;
    private readonly string commandText;
    private readonly IList<SqlParameter> parameters;

    public string DefaultSearchResultId => commandText;

    public SQLCommandFactory(CommandType commandType, string commandText, params SqlParameter[] parameters)
      : this(commandType, commandText, (IList<SqlParameter>)parameters)
    {
    }

    public SQLCommandFactory(CommandType commandType, string commandText, IList<SqlParameter> parameters)
    {
      this.commandType = commandType;
      this.commandText = commandText;
      this.parameters = parameters;
    }

    public SqlCommand CreateCommand(SQLDataSession dataSession)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      var command = new SqlCommand(commandText, dataSession.Connection, dataSession.Transaction);

      command.CommandType = commandType;

      foreach (var parameter in parameters)
      {
        command.Parameters.Add(parameter);
      }

      return command;
    }
  }
}
