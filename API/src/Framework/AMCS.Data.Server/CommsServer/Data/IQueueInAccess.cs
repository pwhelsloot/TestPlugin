using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;

namespace AMCS.Data.Server.CommsServer.Data
{
  public interface IQueueInAccess : IEntityObjectAccess<QueueInEntity>
  {
    void DeleteByMessageId(IDataSession dataSession, string messageId);
    bool HasMessageId(IDataSession dataSession, string messageId);
    IList<QueueInEntity> GetAll(IDataSession dataSession);
  }
}
