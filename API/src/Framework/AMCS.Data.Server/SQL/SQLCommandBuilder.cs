#pragma warning disable 0618

using AMCS.Data.Server.SQL.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  // TODO: Use options consistently
  internal abstract class SQLCommandBuilder<T> : SQLExecutable<T>
    where T : ISQLExecutable<T>
  {
    public SQLDataSession DataSession { get; }

    private readonly FetchInfo fetchInfo;

    protected SQLCommandBuilder(SQLDataSession dataSession, FetchInfo fetchInfo = null)
    {
      DataSession = dataSession;
      this.fetchInfo = fetchInfo;
    }

    protected abstract ISQLCommandFactory CreateCommandFactory();

    protected ISQLExecutableResult ExecuteNonQuery()
    {
      using (var command = CreateCommandFactory().CreateCommand(DataSession))
      {
        int rowsAffected = 0;
        if (command != null)
          rowsAffected = SQLDataAccessHelper.ExecuteNonQuery(DataSession, command, IsUseExtendedTimeout);

        var result = SQLExecutableResult.FromCommand(command, rowsAffected);

        if (result == SQLExecutableResult.Empty)
        {
          result = SQLExecutableResult.GetCommand(command);
        }

        return result;
      }
    }

    protected ISQLReadable ExecuteReader()
    {
      return new SQLReadable(new SQLCommandReaderFactory(DataSession, CreateCommandFactory(), IsUseExtendedTimeout, IsBypassPerformanceLogging), fetchInfo);
    }

    protected ISQLScalarResult ExecuteScalar()
    {
      using (var cmd = CreateCommandFactory().CreateCommand(DataSession))
      {
        var result = SQLDataAccessHelper.ExecuteScaler(DataSession, cmd);
        return new SQLScalarResult(result, cmd);
      }
    }
  }
}
