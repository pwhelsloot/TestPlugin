using AMCS.Data.Server.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface IDataMetricsEvents
  {
    void ConnectionOpen(SqlConnection connection, bool? isDetached = false);

    void ConnectionClose(SqlConnection connection);

    void TransactionStart(SqlConnection connection);

    void TransactionCommit(SqlConnection connection);

    void TransactionRollback(SqlConnection connection);

    void InsertBegin(SqlConnection connection);

    void InsertEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);

    void DeleteBegin(SqlConnection connection);

    void DeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);

    void SoftDeleteBegin(SqlConnection connection);

    void SoftDeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);

    void DynamicNonQueryBegin(SqlConnection connection);

    void DynamicNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);

    void StoredProcedureNonQueryBegin(SqlConnection connection);

    void StoredProcedureNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);

    void UpdateBegin(SqlConnection connection);

    void UpdateEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);

    void ExecuteReaderBegin(SqlConnection connection);

    void ExecuteReaderEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null);
  }
}
