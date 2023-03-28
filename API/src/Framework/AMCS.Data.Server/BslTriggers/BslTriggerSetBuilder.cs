using AMCS.Data.Entity;
using AMCS.JobSystem.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslTriggerSetBuilder
  {
    private readonly List<BslTriggerEntity> bslTriggers;

    public BslTriggerSetBuilder()
    {
      bslTriggers = new List<BslTriggerEntity>();
    }

    public BslTriggerSetBuilder Add(BslTriggerEntity bslTrigger)
    {
      bslTriggers.Add(bslTrigger);
      return this;
    }

    public BslTriggerSetBuilder Add(Type entity, bool create, bool update, bool delete, Type action, string actionConfiguration, bool useJobSystem, string description = null)
    {
      bslTriggers.Add(new BslTriggerEntity
      {
        TriggerEntity = entity.FullName,
        Description = description,
        TriggerOnCreate = create,
        TriggerOnUpdate = update,
        TriggerOnDelete = delete,
        Action = action.FullName,
        ActionConfiguration = actionConfiguration,
        ActionGuid = action.GUID,
        UseJobSystem = useJobSystem
      });

      return this;
    }

    public BslTriggerSetBuilder Add(Type entity, bool create, bool update, bool delete, bool beforeCreate, bool beforeUpdate, bool beforeDelete, Type action, string actionConfiguration, bool useJobSystem)
    {
      bslTriggers.Add(new BslTriggerEntity
      {
        TriggerEntity = entity.FullName,
        TriggerOnCreate = create,
        TriggerOnUpdate = update,
        TriggerOnDelete = delete,
        TriggerBeforeCreate = beforeCreate,
        TriggerBeforeUpdate = beforeUpdate,
        TriggerBeforeDelete = beforeDelete,
        Action = action.FullName,
        ActionConfiguration = actionConfiguration,
        ActionGuid = action.GUID,
        UseJobSystem = useJobSystem
      });

      return this;
    }

    public IList<BslTriggerEntity> Build()
    {
      return bslTriggers;
    }
  }
}
