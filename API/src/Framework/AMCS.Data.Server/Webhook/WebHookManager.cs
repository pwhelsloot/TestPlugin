namespace AMCS.Data.Server.WebHook
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using AMCS.Data.Server.Webhook;
  using Api;
  using BslTrigger;
  using Data.Configuration;
  using Entity;
  using Entity.Webhook;
  using Entity.WebHook;
  using log4net;
  using PluginData.Data.WebHook;
  using Services;
  using Webhook.Engine;
  using Webhook.Exceptions;

  internal class WebHookManager : IWebHookInternalManager
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(WebHookManager));

    private readonly IWebHookService webHookService;
    private readonly IBusinessObjectService businessObjectService;
    private readonly IServiceRootResolver serviceRootResolver;
    private readonly EntityObjectManager entityObjectManager;

    public WebHookManager(EntityObjectManager entityObjectManager, IWebHookService webHookService, 
      IBusinessObjectService businessObjectService, IServiceRootResolver serviceRootResolver)
    {
      this.webHookService = webHookService;
      this.businessObjectService = businessObjectService;
      this.entityObjectManager = entityObjectManager;
      this.serviceRootResolver = serviceRootResolver;
    }

    public Task RaiseSaveAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null)
    {
      var webHookConfiguration =
        GenerateWebHookConfiguration(userId, entityGuid, existingDataSession, WebHookTrigger.Save);
      return RaiseAsync(webHookConfiguration, businessObjectName);
    }

    public Task RaiseInsertAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null)
    {
      var webHookConfiguration =
        GenerateWebHookConfiguration(userId, entityGuid, existingDataSession, WebHookTrigger.Insert);
      return RaiseAsync(webHookConfiguration, businessObjectName);
    }

    public Task RaiseUpdateAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null)
    {
      var webHookConfiguration =
        GenerateWebHookConfiguration(userId, entityGuid, existingDataSession, WebHookTrigger.Update);
      return RaiseAsync(webHookConfiguration, businessObjectName);
    }

    public Task RaiseDeleteAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null)
    {
      var webHookConfiguration =
        GenerateWebHookConfiguration(userId, entityGuid, existingDataSession, WebHookTrigger.Delete);
      return RaiseAsync(webHookConfiguration, businessObjectName);
    }

    // Note: this method is internal to AMCS.Data.Server via the internal interface that it's implemented from. In the future we may change that
    // as web hooks mature and we need to give developers more flexibility
    public async Task RaiseAsync(WebHookConfiguration configuration, string businessObjectName, bool isValidForCoalesce = true)
    {
      if (configuration.EntityObject != null && configuration.TriggerType.HasPostCommitTrigger())
        throw new WebHookExecuteException("You cannot call Raise with a pre-commit entity object when executing a post commit trigger");

      if (configuration.TriggerType.HasPreCommitTrigger() && configuration.ExistingSession == null)
        throw new WebHookExecuteException("You must supply an existing data session when raising pre-commit web hooks");

      var triggerCommitType = configuration.TriggerType.HasPreCommitTrigger()
        ? WebHookConstants.WebHookPreCommitExecutorKey
        : WebHookConstants.WebHookPostCommitExecutorKey;
      var webHooks = FetchWebHooks(configuration.UserId, businessObjectName, configuration.TriggerType, isValidForCoalesce);

      if (!webHooks.Any())
        return;
      
      UpdateWebHookConfiguration(webHooks, configuration, businessObjectName);

      if (configuration.EntityObject != null && webHooks.Any(webHook => !string.IsNullOrWhiteSpace(webHook.Filter)))
      {
        var copy = webHooks.ToList();
        foreach (var webHook in copy.Where(webHook => !string.IsNullOrWhiteSpace(webHook.Filter)))
        {
          if (!FilterMatch.IsMatch(webHook.Filter, configuration.EntityObject))
            webHooks.Remove(webHook);
        }
      }

      var webHookExecutor = DataServices.ResolveKeyed<IWebHookExecutor>(triggerCommitType);
      await webHookExecutor.ExecuteAll(webHooks, configuration);
    }
    
    private List<WebHookEntity> FetchWebHooks(ISessionToken userId, string businessObjectName, WebHookTrigger trigger, bool isValidForCoalesce)
    {
      var webHooks = new List<WebHookEntity>();
      
      foreach (var webHook in webHookService.GetWebHooks().Where(webHook =>
                 string.Equals(webHook.Name, businessObjectName, StringComparison.OrdinalIgnoreCase) 
                 && webHook.TenantId == Guid.Parse(userId.TenantId)))
      {
        if (!string.Equals(webHook.Environment.Trim('/'), serviceRootResolver.ServiceRoot.Trim('/'),
              StringComparison.OrdinalIgnoreCase))
        {
          Log.Error($"Error while processing WebHook {webHook.SystemCategory}:{webHook.Name} - did not match current environment");
          continue;
        }

        var cachedTrigger = (WebHookTrigger)Enum.Parse(typeof(WebHookTrigger), webHook.Trigger, true);

        if (!cachedTrigger.HasFlag(trigger))
        {
          var triggerIsSaveCompatible = cachedTrigger.HasFlag(WebHookTrigger.Save) &&
                                        (trigger == WebHookTrigger.Insert || trigger ==  WebHookTrigger.Update);
          var triggerIsPreSaveCompatible = cachedTrigger.HasFlag(WebHookTrigger.PreSave) &&
                                           (trigger == WebHookTrigger.PreInsert || trigger == WebHookTrigger.PreUpdate);

          if (!triggerIsSaveCompatible && !triggerIsPreSaveCompatible)
            continue;
        }

        if ((WebHookFormat)webHook.Format == WebHookFormat.Coalesce && !isValidForCoalesce)
          continue;

        webHooks.Add(webHook);
      }

      return webHooks;
    }

    private void UpdateWebHookConfiguration(List<WebHookEntity> webHooks, WebHookConfiguration configuration, string businessObjectName)
    {
      if (webHooks.Any(webHook => (WebHookFormat)webHook.Format == WebHookFormat.Full) && configuration.TriggerType.HasPostCommitTrigger())
      {
        var businessObject = businessObjectService.Get(businessObjectName);
        var type = entityObjectManager.Entities
          .FindByTypeName(businessObject.BusinessObject.MappedApiEntity.Split(',')[0]).Type;
        if (type == null)
          throw new WebHookExecuteException("No associated type was found with a full formatted web hook");

        EntityObject entity;

        if (configuration.ExistingSession == null)
        {
          using (var session = BslDataSessionFactory.GetDataSession())
          using (var transaction = session.CreateTransaction())
          {
            entity = session.GetByGuid(configuration.UserId, type, configuration.EntityGuid.Value);
          }
        }
        else
        {
          entity = configuration.ExistingSession.GetByGuid(configuration.UserId, type, configuration.EntityGuid.Value);
        }

        configuration.EntityObject = entity;
      }
    }
    
    private WebHookConfiguration GenerateWebHookConfiguration(ISessionToken userId, Guid entityGuid,
      IDataSession existingDataSession, WebHookTrigger trigger)
    {
      return new WebHookConfiguration
      {
        TriggerType = WebHookTrigger.Delete,
        EntityGuid = entityGuid,
        UserId = userId,
        TenantId = userId.TenantId,
        TransactionId = existingDataSession?.IsTransaction() == true
          ? existingDataSession.GetTransactionId().ToString()
          : Guid.Empty.ToString()
      };
    }
  }
}