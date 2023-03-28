namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Api;
  using Data.Configuration;
  using Entity;
  using Entity.WebHook;
  using log4net;
  using PluginData.Data.WebHook;
  using Services;
  using AMCS.Data.Server.BslTriggers;
  using Webhook.Exceptions;
  using Support;
  using AMCS.Data.Server.Webhook;
  using Entity.Webhook;

  internal class WebHookBslTriggerExecuteService : IWebHookBslTriggerExecuteService
  {
    private const string CoalesceKey = "CoalesceKey";

    private static readonly ILog Log = LogManager.GetLogger(typeof(WebHookBslTriggerExecuteService));

    private readonly EntityObjectManager entityObjectManager;

    public WebHookBslTriggerExecuteService(EntityObjectManager entityObjectManager)
    {
      this.entityObjectManager = entityObjectManager;
    }

    public void Execute(ISessionToken userId, BslTriggerRequest request, IBslActionContext bslActionContext)
    {
      var businessObjectService = DataServices.Resolve<IBusinessObjectService>();
      var entityType = request.EntityType.Split(',')[0];
      var executingType = entityObjectManager.Entities.FindByTypeName(entityType).Type;

      if (executingType == null)
        throw new WebHookExecuteException($"No existing type was found for {entityType}");

      var businessObjects = businessObjectService.GetAll();
      var filteredBusinessObjects =
        businessObjects.Where(businessObject => businessObject.Types.Contains(executingType));
      var triggerType = WebHookUtils.GetWebHookTrigger(request.Action);
      var webHookManager = DataServices.Resolve<IWebHookInternalManager>();
      var key = bslActionContext.Context.TryGetValue(CoalesceKey, out _);

      var isValidForCoalesce = true;
      if (key)
      {
        isValidForCoalesce = false;
      }
      else
      {
        bslActionContext.Context.Add(CoalesceKey, CoalesceKey);
      }

      var webHookConfiguration = new WebHookConfiguration
      {
        TriggerType = triggerType,
        UserId = userId,
        EntityGuid = request.GUID ?? WebHookUtils.GetEntityGuid(userId, request.Id, request.EntityType),
        ExistingSession = request.DataSession,
        EntityObject = request.EntityObject,
        TenantId = userId?.TenantId,
        TransactionId = request.DataSession?.IsTransaction() == true
          ? request.DataSession.GetTransactionId().ToString()
          : Guid.Empty.ToString()
      };

      var tasks = filteredBusinessObjects.Select(businessObject =>
        webHookManager.RaiseAsync(webHookConfiguration, businessObject.BusinessObject.Name, isValidForCoalesce));

      Task.Run(() => Task.WhenAll(tasks)).GetAwaiter().GetResult();
    }
  }
}
