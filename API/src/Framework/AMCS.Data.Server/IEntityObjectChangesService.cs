using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public interface IEntityObjectChangesService : IEntityObjectService
  {
    byte[] GetHighestRowVersion(ISessionToken userId, IDataSession existingDataSession = null);

    IList<EntityObject> GetChanges(ISessionToken userId, IEntityObjectChangesFilter filter, IDataSession existingDataSession = null);
  }
}
