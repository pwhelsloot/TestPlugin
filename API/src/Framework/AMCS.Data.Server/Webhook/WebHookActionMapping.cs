namespace AMCS.Data.Server.WebHook
{
  using System;
  using System.Collections.Generic;
  using AMCS.Data.Server.WebHook.BslTrigger;
  using Entity;
  using PluginData.Data.WebHook;

  internal static class WebHookActionMapping
  {
    public static BslTriggerActionDto ToBslTriggers(WebHookTrigger trigger)
    {
      return new BslTriggerActionDto
      {
        Create = trigger.HasFlag(WebHookTrigger.Save) || trigger.HasFlag(WebHookTrigger.Insert),
        Update = trigger.HasFlag(WebHookTrigger.Save) || trigger.HasFlag(WebHookTrigger.Update),
        Delete = trigger.HasFlag(WebHookTrigger.Delete),
        BeforeCreate = trigger.HasFlag(WebHookTrigger.PreSave) || trigger.HasFlag(WebHookTrigger.PreInsert),
        BeforeUpdate = trigger.HasFlag(WebHookTrigger.PreSave) || trigger.HasFlag(WebHookTrigger.PreUpdate),
        BeforeDelete = trigger.HasFlag(WebHookTrigger.PreDelete)
      };
    }

    public static WebHookTrigger FromBslAction(BslAction action)
    {
      switch (action)
      {
        case BslAction.Create:
          return WebHookTrigger.Insert;
        case BslAction.Update:
          return WebHookTrigger.Update;
        case BslAction.Delete:
          return WebHookTrigger.Delete;
        case BslAction.BeforeCreate:
          return WebHookTrigger.PreInsert;
        case BslAction.BeforeUpdate:
          return WebHookTrigger.PreUpdate;
        case BslAction.BeforeDelete:
          return WebHookTrigger.PreDelete;
        default:
          throw new ArgumentOutOfRangeException(nameof(action), action, null);
      }
    }
  }
}