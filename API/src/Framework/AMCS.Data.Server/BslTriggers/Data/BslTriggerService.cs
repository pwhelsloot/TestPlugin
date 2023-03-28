using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.BslTriggers.Data
{
  public class BslTriggerService : EntityObjectService<BslTriggerEntity>, IBslTriggerService
  {
    public BslTriggerService(IEntityObjectAccess<BslTriggerEntity> dataAccess)
      : base(dataAccess)
    {
    }

    public IList<BslTriggerEntity> GetAllBySystemCategory(IDataSession dataSession, ISessionToken userId, string category)
    {
      return ((IBslTriggerServiceAccess)DataAccess).GetAllBySystemCategory(dataSession, userId, category);
    }

    public override int? Save(ISessionToken userId, BslTriggerEntity bslTrigger, IDataSession existingDataSession = null)
    {
      var bslTriggerEntity = bslTrigger.Id.HasValue 
        ? base.GetById(userId, bslTrigger.Id32, existingDataSession) 
        : ((IBslTriggerServiceAccess)DataAccess).GetByEntityActionGuidCategory(existingDataSession, userId, bslTrigger.TriggerEntity, bslTrigger.ActionGuid, bslTrigger.SystemCategory, bslTrigger.ActionConfiguration);

      if (bslTriggerEntity != null)
      {
        var different = bslTriggerEntity.TriggerEntity != bslTrigger.TriggerEntity
          || bslTriggerEntity.Description != bslTrigger.Description
          || bslTriggerEntity.TriggerOnCreate != bslTrigger.TriggerOnCreate
          || bslTriggerEntity.TriggerOnUpdate != bslTrigger.TriggerOnUpdate
          || bslTriggerEntity.TriggerOnDelete != bslTrigger.TriggerOnDelete
          || bslTriggerEntity.TriggerBeforeCreate != bslTrigger.TriggerBeforeCreate
          || bslTriggerEntity.TriggerBeforeUpdate != bslTrigger.TriggerBeforeUpdate
          || bslTriggerEntity.TriggerBeforeDelete != bslTrigger.TriggerBeforeDelete
          || bslTriggerEntity.Action != bslTrigger.Action
          || bslTriggerEntity.ActionConfiguration != bslTrigger.ActionConfiguration
          || bslTriggerEntity.ActionGuid != bslTrigger.ActionGuid
          || bslTriggerEntity.UseJobSystem != bslTrigger.UseJobSystem;

        if (!different)
        {
          // if nothing has changed, don't save and don't broadcast
          return bslTriggerEntity.Id32;
        }

        bslTriggerEntity.TriggerEntity = bslTrigger.TriggerEntity;
        bslTriggerEntity.TriggerOnCreate = bslTrigger.TriggerOnCreate;
        bslTriggerEntity.TriggerOnUpdate = bslTrigger.TriggerOnUpdate;
        bslTriggerEntity.TriggerOnDelete = bslTrigger.TriggerOnDelete;
        bslTriggerEntity.TriggerBeforeCreate = bslTrigger.TriggerBeforeCreate;
        bslTriggerEntity.TriggerBeforeUpdate = bslTrigger.TriggerBeforeUpdate;
        bslTriggerEntity.TriggerBeforeDelete = bslTrigger.TriggerBeforeDelete;
        bslTriggerEntity.Action = bslTrigger.Action;
        bslTriggerEntity.ActionConfiguration = bslTrigger.ActionConfiguration;
        bslTriggerEntity.ActionGuid = bslTrigger.ActionGuid;
        bslTriggerEntity.UseJobSystem = bslTrigger.UseJobSystem;
        bslTriggerEntity.Description = bslTrigger.Description;
      }
      else
      {
        bslTriggerEntity = bslTrigger;
      }

      var id = base.Save(userId, bslTriggerEntity, existingDataSession);
      existingDataSession.Broadcast(new BslTriggerChanged(id.Value));
      return id;
    }

    public override void Delete(ISessionToken userId, BslTriggerEntity bslTrigger, IDataSession existingDataSession = null)
    {
      base.Delete(userId, bslTrigger, existingDataSession);
      existingDataSession.Broadcast(new BslTriggerChanged(bslTrigger.Id32));
    }
  }
}
