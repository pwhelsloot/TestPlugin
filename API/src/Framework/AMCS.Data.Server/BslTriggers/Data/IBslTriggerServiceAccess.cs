using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.BslTriggers.Data
{
  public interface IBslTriggerServiceAccess : IEntityObjectAccess<BslTriggerEntity>
  {
    IList<BslTriggerEntity> GetAllBySystemCategory(IDataSession dataSession, ISessionToken userId, string category);

    BslTriggerEntity GetByEntityActionGuidCategory(IDataSession dataSession, ISessionToken userId, string entity, Guid actionGuid, string category, string actionConfiguration);
  }
}
