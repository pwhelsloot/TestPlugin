#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLCommandReaderFactory : ISQLReaderFactory
  {
    private readonly SQLDataSession dataSession;
    private readonly ISQLCommandFactory commandFactory;
    private readonly bool useExtendedTimeout;
    private readonly bool bypassPerformanceLogging;

    public string DefaultSearchResultId => commandFactory.DefaultSearchResultId;

    public SQLCommandReaderFactory(SQLDataSession dataSession, ISQLCommandFactory commandFactory, bool useExtendedTimeout, bool bypassPerformanceLogging)
    {
      this.dataSession = dataSession;
      this.commandFactory = commandFactory;
      this.useExtendedTimeout = useExtendedTimeout;
      this.bypassPerformanceLogging = bypassPerformanceLogging;
    }

    public ISQLReaderResult GetReader()
    {
      return new SQLReaderResult(this, commandFactory.CreateCommand(dataSession));
    }

    private class SQLReaderResult : ISQLReaderResult
    {
      private readonly SQLCommandReaderFactory owner;
      private SqlCommand command;
      private bool disposed;

      public SQLReaderResult(SQLCommandReaderFactory owner, SqlCommand command)
      {
        this.owner = owner;
        this.command = command;
      }

      public IDataReader GetReader()
      {
        owner.dataSession.Metrics?.ExecuteReaderBegin(this.owner.dataSession.Connection);

        IDataReader reader;
        if (owner.useExtendedTimeout)
          reader = SQLDataAccessHelper.ExecuteReaderWithExtendedTimeout(owner.dataSession, command, owner.bypassPerformanceLogging);
        else
          reader = SQLDataAccessHelper.ExecuteReader(owner.dataSession, command, owner.bypassPerformanceLogging);

        var metrics = owner.dataSession.Metrics;
        if (metrics != null)
        {
          try
          {
            int rows = 0;

            var proxy = new DataReaderProxy(reader);

            proxy.NextRow += (s, e) => rows++;
            proxy.Disposed += (s, e) => metrics.ExecuteReaderEnd(this.owner.dataSession.Connection, command, rows);

            reader = proxy;
          }
          catch (Exception ex)
          {
            metrics.ExecuteReaderEnd(null, command, 0, ex);
            throw;
          }
        }

        return reader;
      }

      public ISQLExecutableResult GetResult()
      {
        return SQLExecutableResult.FromCommand(command, 0);
      }

      public void Dispose()
      {
        if (!disposed)
        {
          if (command != null)
          {
            command.Dispose();
            command = null;
          }

          disposed = true;
        }
      }
    }
  }
}
