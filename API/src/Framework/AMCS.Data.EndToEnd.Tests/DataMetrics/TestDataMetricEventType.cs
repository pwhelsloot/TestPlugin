using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public enum TestDataMetricEventType
  {
    DynamicNonQuery,
    StoredProcedureNonQuery,
    Reader,
    OpenConnection,
    OpenDetachedConnection,
    ClosedConnection,
    TransactionCommit,
    TransactionRollback,
    TransactionStart,
    // Everything below here (higher than 10) is an update. This simplifies
    // the IsUpdate/IsQuery extension methods.
    Insert = 10,
    Update,
    Delete,
    SoftDelete
  }
}
