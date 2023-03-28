namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using System;
  using Entity;
  using PluginData.Data.WebHook;
  using Webhook.Exceptions;

  public class WebHookConfiguration
  {
    public int EntityId { get; set; }

    public Guid? EntityGuid { get; set; }

    public EntityObject EntityObject { get; set; }

    public string EntityTypeName { get; set; }

    private WebHookTrigger triggerType;

    public WebHookTrigger TriggerType
    {
      get => triggerType;
      set
      {
        if ((value & (value - 1)) != 0)
          throw new WebHookExecuteException("You can only execute a single web hook trigger type at a time.");

        triggerType = value;
      }
    }

    public string TransactionId { get; set; }

    public string TenantId { get; set; }

    public ISessionToken UserId { get; set; }

    public IDataSession ExistingSession { get; set; }
  }
}