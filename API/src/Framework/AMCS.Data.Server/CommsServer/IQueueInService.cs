using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;

namespace AMCS.Data.Server.CommsServer
{
  public interface IQueueInService : IEntityObjectService<QueueInEntity>
  {
    void DeleteByMessageId(string messageId, IDataSession dataSession);
    bool HasMessageId(string messageId, IDataSession dataSession);
    IList<QueueInEntity> GetAll(IDataSession dataSession);
  }
}
