using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Schema.Access
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// 
  /// Defines operations that are required to be performed on a database.
  /// </summary>
  public interface IDatabaseInterface
  {
    string ConnectionString { get; }
    IsolationLevel TransactionIsolationLevel { get; set; }

    int ExecuteNonQuery(string sql, IDictionary<string, object> parameters = null);
    IDataReader ExecuteReader(string sql, IDictionary<string, object> parameters = null);
    DataTable GetDataTable(string sql, IDictionary<string, object> parameters = null);
    DataTable GetDataTable(string sql, IDictionary<string, object> parameters, bool executeWithinNewTransaction);
  }
}
