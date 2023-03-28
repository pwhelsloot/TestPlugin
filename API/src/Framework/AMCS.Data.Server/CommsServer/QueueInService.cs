using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;
using AMCS.Data.Server.CommsServer.Data;

namespace AMCS.Data.Server.CommsServer
{
  public class QueueInService : EntityObjectService<QueueInEntity>, IQueueInService
  {
    private new IQueueInAccess DataAccess => (IQueueInAccess)base.DataAccess;

    public QueueInService(IEntityObjectAccess<QueueInEntity> dataAccess)
      : base(dataAccess)
    {
    }

    public void DeleteByMessageId(string messageId, IDataSession dataSession)
    {
      DataAccess.DeleteByMessageId(dataSession, messageId);
    }

    public bool HasMessageId(string messageId, IDataSession dataSession)
    {
      return DataAccess.HasMessageId(dataSession, messageId);
    }

    public IList<QueueInEntity> GetAll(IDataSession dataSession)
    {
      return DataAccess.GetAll(dataSession);
    }
  }
}
