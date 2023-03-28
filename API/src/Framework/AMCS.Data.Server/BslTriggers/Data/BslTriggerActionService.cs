using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.BslTriggers.Data
{
  public class BslTriggerActionService : EntityObjectService<BslTriggerActionEntity>
  {
    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      var bslTriggerActions = DataServices.Resolve<AMCS.Data.Server.BslTriggers.IBslTriggerManager>().GetBslTriggerActions();
      return new ApiQuery(bslTriggerActions.ToList<EntityObject>(), bslTriggerActions.Count);
    }
  }
}
