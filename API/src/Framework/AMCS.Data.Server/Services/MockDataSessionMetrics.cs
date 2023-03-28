using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  internal class MockDataSessionMetrics : IDataMetricsEvents
  {
    public static IDataMetricsEvents Instance = new MockDataSessionMetrics();

    private MockDataSessionMetrics()
    {
    }

    public void Dispose()
    {
    }

    public void ConnectionOpen(SqlConnection connection, bool? isDetached = false)
    {
    }

    public void ConnectionClose(SqlConnection connection)
    {
    }

    public void TransactionStart(SqlConnection connection)
    {
    }

    public void TransactionCommit(SqlConnection connection)
    {
    }

    public void TransactionRollback(SqlConnection connection)
    {
    }

    public void InsertBegin(SqlConnection connection)
    {
    }

    public void InsertEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }

    public void DeleteBegin(SqlConnection connection)
    {
    }

    public void DeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }

    public void SoftDeleteBegin(SqlConnection connection)
    {
    }

    public void SoftDeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }

    public void DynamicNonQueryBegin(SqlConnection connection)
    {
    }

    public void DynamicNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }

    public void StoredProcedureNonQueryBegin(SqlConnection connection)
    {
    }

    public void StoredProcedureNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }

    public void UpdateBegin(SqlConnection connection)
    {
    }

    public void UpdateEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }

    public void ExecuteReaderBegin(SqlConnection connection)
    {
    }

    public void ExecuteReaderEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex)
    {
    }
  }
}
