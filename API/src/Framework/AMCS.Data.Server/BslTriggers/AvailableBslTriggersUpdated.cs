namespace AMCS.Data.Server.BslTriggers
{
  public class AvailableBslTriggersUpdated
  {
    public int? BslTriggerEntityId { get; }

    public AvailableBslTriggersUpdated(int? bslTriggerEntityId)
    {
      BslTriggerEntityId = bslTriggerEntityId;
    }
  }
}