using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.BslTriggers.Data
{
  public class BslTriggerEnabledService : EntityObjectService<BslTriggerEnabledEntity>
  {
    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      var bslTriggerEnabledEntities = DataServices.Resolve<AMCS.Data.Server.BslTriggers.IBslTriggerManager>().GetBslTriggerEnabledEntities();
      return new ApiQuery(bslTriggerEnabledEntities.ToList<EntityObject>(), bslTriggerEnabledEntities.Count);
    }
  }
}
