using System.Collections.Generic;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.BslTriggers.Data
{
  public interface IBslTriggerService : IEntityObjectService<BslTriggerEntity>
  {
    IList<BslTriggerEntity> GetAllBySystemCategory(IDataSession dataSession, ISessionToken userId, string category);
  }
}