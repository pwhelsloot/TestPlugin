namespace AMCS.Data.Server.WebHook
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Xml.Serialization;
  using BslTrigger;
  using Data.Configuration;
  using Data.Util.Extension;
  using Entity;
  using Entity.Webhook;
  using PluginData.Data.WebHook;
  using Services;

  public static class WebHookUtils
  {
    public static WebHookTrigger GetWebHookTrigger(BslAction action)
    {
      switch (action)
      {
        case BslAction.BeforeCreate: return WebHookTrigger.PreInsert;
        case BslAction.BeforeUpdate: return WebHookTrigger.PreUpdate;
        case BslAction.BeforeDelete: return WebHookTrigger.PreDelete;
        case BslAction.Create: return WebHookTrigger.Insert;
        case BslAction.Update: return WebHookTrigger.Update;
        case BslAction.Delete: return WebHookTrigger.Delete;
        default: throw new ArgumentOutOfRangeException(nameof(action), action, null);
      }
    }

    public static string GetWebHookDescription(WebHookTrigger trigger)
    {
      var fieldInfo = trigger.GetType().GetField(trigger.ToString());
      var attribute = fieldInfo?.GetCustomAttribute<XmlEnumAttribute>()?.Name;

      return attribute;
    }

    public static string GetWebHookExecuteTriggerType(WebHookTrigger trigger)
    {
      var triggerType = trigger.HasPreCommitTrigger()
        ? WebHookConstants.WebHookPreCommitExecutorKey
        : WebHookConstants.WebHookPostCommitExecutorKey;

      return triggerType;
    }

    public static bool TryGetWebHookTriggerByDescription(string description, out WebHookTrigger? webHookTrigger)
    {
      webHookTrigger = null;

      var descriptions = description.Split(',');
      foreach (var item in descriptions)
      {
        var triggerToAdd = (WebHookTrigger?)typeof(WebHookTrigger).GetFields()
          .SingleOrDefault(flag => string.Equals(flag.GetCustomAttribute<XmlEnumAttribute>()?.Name, item.Trim(), StringComparison.OrdinalIgnoreCase))?.GetValue(null);

        if (!triggerToAdd.HasValue) 
          continue;

        if (webHookTrigger == null)
          webHookTrigger = triggerToAdd;
        else
          webHookTrigger |= triggerToAdd;
      }

      return webHookTrigger != null;
    }

    public static Guid GetEntityGuid(ISessionToken userId, int id, string incomingType)
    {
      if (id == default)
        return Guid.Empty;

      try
      {
        using (var session = BslDataSessionFactory.GetDataSession(userId))
        using (var transaction = session.CreateTransaction())
        {
          var foundType = DataServices.Resolve<EntityObjectManager>().Entities
            .FindByTypeName(incomingType.Split(',')[0])?.Type;
          if (foundType == null)
            return Guid.Empty;

          var guid = DataServices.Resolve<IDataAccessIdService>().GetGuidById(session, foundType, id);

          return guid ?? Guid.Empty;
        }
      }
      catch
      {
        return Guid.Empty;
      }
    }

    public static Guid GetEntityGuid(WebHookConfiguration configuration)
    {
      if (configuration.EntityGuid.HasValue && configuration.EntityGuid != Guid.Empty)
        return configuration.EntityGuid.Value;

      if (configuration.EntityObject?.GUID != null)
        return configuration.EntityObject.GUID.Value;

      if (configuration.UserId == null || configuration.EntityId == default || string.IsNullOrWhiteSpace(configuration.EntityTypeName))
        return Guid.Empty;

      return GetEntityGuid(configuration.UserId, configuration.EntityId, configuration.EntityTypeName);
    }
  }
}