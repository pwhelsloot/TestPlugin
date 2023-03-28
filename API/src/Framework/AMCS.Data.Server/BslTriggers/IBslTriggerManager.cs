using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.BslTriggers
{
  public interface IBslTriggerManager
  {
    void ConfigureBslTriggerSet(IDataSession dataSession, ISessionToken userId, string category, IList<BslTriggerEntity> triggerSet);

    void RaiseEntity(IDataSession session, ISessionToken userId, Type entityType, BslAction bslAction, int id, Guid? guid, EntityObject entityObject);

    void Raise(ISessionToken userId, IList<BslPendingTrigger> bslPendingTriggers);

    void RaiseJobSystemJob(ISessionToken userId, IList<BslPendingTrigger> bslPendingTriggers);

    void ExecuteBslTriggersFromJobSystem(ISessionToken userId, BslTriggerJobRequest bslTriggerJobRequest);

    IList<BslTriggerActionEntity> GetBslTriggerActions();

    IList<BslTriggerEnabledEntity> GetBslTriggerEnabledEntities();
  }
}
