using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;

namespace AMCS.Data.Server.CommsServer
{
  public interface IQueueOutService : IEntityObjectService<QueueOutEntity>
  {
    void DeleteAllById(IEnumerable<int> ids, IDataSession dataSession);
    IList<QueueOutEntity> GetAllByOffset(int? offset, int batchSize, IDataSession dataSession);
  }
}
