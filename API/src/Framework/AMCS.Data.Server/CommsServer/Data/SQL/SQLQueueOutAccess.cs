using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;
using AMCS.Data.Server.SQL;

namespace AMCS.Data.Server.CommsServer.Data.SQL
{
  public class SQLQueueOutAccess : SQLEntityObjectAccess<QueueOutEntity>, IQueueOutAccess
  {
    public void DeleteAllById(IDataSession dataSession, IEnumerable<int> ids)
    {
      dataSession.Query("DELETE FROM [comms].[QueueOut] WHERE [QueueOutId] IN (SELECT ID FROM @Ids)")
        .SetIdList("@Ids", "Id", ids, "dbo.IdTableType")
        .ExecuteNonQuery();
    }

    public IList<QueueOutEntity> GetAllByOffset(IDataSession dataSession, int? offset, int batchSize)
    {
      if (offset.HasValue)
      {
        return dataSession.Query("SELECT TOP {=BatchSize} * FROM [comms].[QueueOut] WHERE [QueueOutId] > @Offset ORDER BY [QueueOutId]")
          .Set("@BatchSize", batchSize)
          .Set("@Offset", offset.Value)
          .Execute()
          .List<QueueOutEntity>();
      }
      else
      {
        return dataSession.Query("SELECT TOP {=BatchSize} * FROM [comms].[QueueOut] ORDER BY [QueueOutId]")
          .Set("@BatchSize", batchSize)
          .Execute()
          .List<QueueOutEntity>();
      }
    }
  }
}
