using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.CommsServer;

namespace AMCS.Data.Server.CommsServer.Data
{
  public interface IQueueOutAccess : IEntityObjectAccess<QueueOutEntity>
  {
    void DeleteAllById(IDataSession dataSession, IEnumerable<int> ids);
    IList<QueueOutEntity> GetAllByOffset(IDataSession dataSession, int? offset, int batchSize);
  }
}
