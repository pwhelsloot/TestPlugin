using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;
using AMCS.Data.Server.SQL;

namespace AMCS.Data.Server.CommsServer.Data.SQL
{
  public class SQLQueueInAccess : SQLEntityObjectAccess<QueueInEntity>, IQueueInAccess
  {
    public void DeleteByMessageId(IDataSession dataSession, string messageId)
    {
      dataSession.Query("DELETE FROM [comms].[QueueIn] WHERE [MessageId] = @MessageId")
        .Set("@MessageId", messageId)
        .ExecuteNonQuery();
    }

    public bool HasMessageId(IDataSession dataSession, string messageId)
    {
      return dataSession.Query("SELECT 1 FROM [comms].[QueueIn] WHERE [MessageId] = @MessageId")
        .Set("@MessageId", messageId)
        .Execute()
        .FirstOrDefaultScalar() != null;
    }

    public IList<QueueInEntity> GetAll(IDataSession dataSession)
    {
      return dataSession.Query("SELECT * FROM [comms].[QueueIn] ORDER BY [QueueInId]")
        .Execute()
        .List<QueueInEntity>();
    }
  }
}
