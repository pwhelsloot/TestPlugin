using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [Serializable]
  [ApiExplorer(Methods = ApiExplorerMethods.AllQuery)]
  [EntityTable("BslTrigger", nameof(BslTriggerId))]
  public class BslTriggerEntity : EntityObject, ICacheCoherentEntity
  {
    [EntityMember]
    public int? BslTriggerId { get; set; }

    [EntityMember]
    public string TriggerEntity { get; set; }

    [EntityMember]
    public string Description { get; set; }

    [EntityMember]
    public bool TriggerOnCreate { get; set; }

    [EntityMember]
    public bool TriggerOnUpdate { get; set; }

    [EntityMember]
    public bool TriggerOnDelete { get; set; }

    [EntityMember]
    public bool TriggerBeforeCreate { get; set; }

    [EntityMember]
    public bool TriggerBeforeUpdate { get; set; }

    [EntityMember]
    public bool TriggerBeforeDelete { get; set; }

    [EntityMember]
    public string Action { get; set; }

    [EntityMember]
    public string ActionConfiguration { get; set; }

    [EntityMember]
    public Guid ActionGuid { get; set; }

    [EntityMember]
    public bool UseJobSystem { get; set; }

    [EntityMember]
    public string SystemCategory { get; set; }

    public override int? GetId() => BslTriggerId;

    private readonly string[] validatedProperties =
    {
      nameof(TriggerBeforeCreate),
      nameof(TriggerBeforeUpdate),
      nameof(TriggerBeforeDelete)
    };

    public override string[] GetValidatedProperties() => validatedProperties;

    protected override string GetValidationError(string propertyName)
    {
      string error = null;

      switch (propertyName)
      {
        case "TriggerBeforeCreate":
          if (TriggerBeforeCreate && UseJobSystem)
            error = "TriggerBeforeCreate can not be used in conjunction with UseJobSystem";
          break;

        case "TriggerBeforeUpdate":
          if (TriggerBeforeUpdate && UseJobSystem)
            error = "TriggerBeforeUpdate can not be used in conjunction with UseJobSystem";
          break;

        case "TriggerBeforeDelete":
          if (TriggerBeforeDelete && UseJobSystem)
            error = "TriggerBeforeDelete can not be used in conjunction with UseJobSystem";
          break;
      }

      return error;
    }

    public bool IsEqualTo(object obj)
    {
      var bslTrigger = (BslTriggerEntity)obj;
      
      var different = TriggerEntity != bslTrigger.TriggerEntity
                      || TriggerOnCreate != bslTrigger.TriggerOnCreate
                      || TriggerOnUpdate != bslTrigger.TriggerOnUpdate
                      || TriggerOnDelete != bslTrigger.TriggerOnDelete
                      || TriggerBeforeCreate != bslTrigger.TriggerBeforeCreate
                      || TriggerBeforeUpdate != bslTrigger.TriggerBeforeUpdate
                      || TriggerBeforeDelete != bslTrigger.TriggerBeforeDelete
                      || Action != bslTrigger.Action
                      || ActionConfiguration != bslTrigger.ActionConfiguration
                      || ActionGuid != bslTrigger.ActionGuid
                      || UseJobSystem != bslTrigger.UseJobSystem;

      return !different;
    }
  }
}
