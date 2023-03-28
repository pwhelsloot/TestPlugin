using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;
using AMCS.Data.Server.CommsServer.Data;

namespace AMCS.Data.Server.CommsServer
{
  public class QueueOutService : EntityObjectService<QueueOutEntity>, IQueueOutService
  {
    private new IQueueOutAccess DataAccess => (IQueueOutAccess)base.DataAccess;

    public QueueOutService(IEntityObjectAccess<QueueOutEntity> dataAccess)
      : base(dataAccess)
    {
    }

    public void DeleteAllById(IEnumerable<int> ids, IDataSession dataSession)
    {
      DataAccess.DeleteAllById(dataSession, ids);
    }

    public IList<QueueOutEntity> GetAllByOffset(int? offset, int batchSize, IDataSession dataSession)
    {
      return DataAccess.GetAllByOffset(dataSession, offset, batchSize);
    }
  }
}
