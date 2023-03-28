namespace AMCS.Data.Entity.Webhook
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Xml.Serialization;
  using AMCS.PluginData.Data.WebHook;

  public static class WebHookTriggerUtil
  {
    public static bool HasPostCommitTrigger(this WebHookTrigger trigger)
    {
      return trigger.HasFlag(WebHookTrigger.Delete)
             || trigger.HasFlag(WebHookTrigger.Insert)
             || trigger.HasFlag(WebHookTrigger.Update)
             || trigger.HasFlag(WebHookTrigger.Save);
    }

    public static bool HasPreCommitTrigger(this WebHookTrigger trigger)
    {
      return trigger.HasFlag(WebHookTrigger.PreDelete)
             || trigger.HasFlag(WebHookTrigger.PreInsert)
             || trigger.HasFlag(WebHookTrigger.PreUpdate)
             || trigger.HasFlag(WebHookTrigger.PreSave);
    }

    public static bool OnlyContains(this WebHookTrigger trigger, WebHookTrigger expected)
    {
      return (trigger | expected) == expected;
    }

    public static string PrintTriggers(WebHookTrigger? onlyInclude = null)
    {
      var availableTriggers = Enum.GetValues(typeof(WebHookTrigger)).Cast<WebHookTrigger>().ToList();

      var results = new StringBuilder();

      foreach (var availableTrigger in availableTriggers.Select((item, index) => new { Item = item, Index = index }))
      {
        if (availableTrigger.Item == WebHookTrigger.None)
          continue;

        if (onlyInclude.HasValue && !onlyInclude.Value.HasFlag(availableTrigger.Item))
          continue;

        results.Append($"{GetWebHookDescription(availableTrigger.Item)},");
      }

      return results.ToString().Trim(',');
    }

    public static string GetWebHookDescription(WebHookTrigger trigger)
    {
      var fieldInfo = trigger.GetType().GetField(trigger.ToString());
      var attribute = fieldInfo?.GetCustomAttribute<XmlEnumAttribute>()?.Name;

      return attribute;
    }
  }
}