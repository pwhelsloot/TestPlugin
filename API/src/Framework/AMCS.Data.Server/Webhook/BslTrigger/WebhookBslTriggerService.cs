namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity;
  using Entity.WebHook;
  using AMCS.Data.Server.BslTriggers;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.WebHook;
  using PluginData.Data.WebHook;

  internal class WebHookBslTriggerService : IWebHookBslTriggerService, IDelayedStartup
  {
    private const string WebHookBslCategory = "WebHooks";

    private readonly IWebHookCacheService webHookCacheService;
    private readonly IBslTriggerManager bslTriggerManager;
    private readonly IBusinessObjectService businessObjectService;
    private readonly IUserService userService;
    private readonly EntityObjectManager entityObjectManager;

    public WebHookBslTriggerService(IWebHookCacheService webHookCacheService, 
      IBslTriggerManager bslTriggerManager,
      IBusinessObjectService businessObjectService,
      IUserService userService,
      EntityObjectManager entityObjectManager)
    {
      this.webHookCacheService = webHookCacheService;
      this.bslTriggerManager = bslTriggerManager;
      this.businessObjectService = businessObjectService;
      this.userService = userService;
      this.entityObjectManager = entityObjectManager;

      this.webHookCacheService.DataRefreshed += WebHookCacheService_DataRefreshed;
    }

    public void Start()
    {
      WebHookCacheService_DataRefreshed(this, null);
    }

    private void WebHookCacheService_DataRefreshed(object sender, EventArgs e)
    {
      var cachedEntities = webHookCacheService.GetWebHooks();
      var webHookBusinessObjects = this.businessObjectService.GetAll()
        .Where(businessObject => businessObject.BusinessObject.AllowWebHooks && cachedEntities
          .Select(webHook => webHook.Name.Split(':').Last()).Contains(businessObject.BusinessObject.Name))
        .ToList();

      if (!webHookBusinessObjects.Any())
      {
        ConfigureBslTriggerSet(new List<BslTriggerEntity>());
        return;
      }

      var bslTriggerTypeList = GenerateBslTriggerTypeList(webHookBusinessObjects, cachedEntities);
      var webHookTriggerSet = CreateBslTriggerSet(bslTriggerTypeList);

      ConfigureBslTriggerSet(webHookTriggerSet.Build());
    }

    private Dictionary<Type, BslTriggerActionDto> GenerateBslTriggerTypeList(
      List<BusinessObjectResult> webHookBusinessObjects, IList<WebHookEntity> cachedEntities)
    {
      var bslTriggerTypeList = new Dictionary<Type, BslTriggerActionDto>();

      // We can have multiple web hooks pointing to the same underlying entity type, but we only want to add that type
      // once as far as BSL trigger creation
      foreach (var webHookBusinessObject in webHookBusinessObjects)
      {
        var webHookTriggers = cachedEntities
          .Where(entity => entity.Name == webHookBusinessObject.BusinessObject.Name)
          .Select(webHook => WebHookActionMapping.ToBslTriggers((WebHookTrigger)Enum.Parse(typeof(WebHookTrigger), webHook.Trigger)))
          .ToList();

        var actionDto = new BslTriggerActionDto
        {
          BeforeCreate = webHookTriggers.Any(trigger => trigger.BeforeCreate),
          BeforeDelete = webHookTriggers.Any(trigger => trigger.BeforeDelete),
          BeforeUpdate = webHookTriggers.Any(trigger => trigger.BeforeUpdate),
          Create = webHookTriggers.Any(trigger => trigger.Create),
          Delete = webHookTriggers.Any(trigger => trigger.Delete),
          Update = webHookTriggers.Any(trigger => trigger.Update)
        };

        if (actionDto.NoActionsAvailable)
          continue;

        foreach (var webHookType in webHookBusinessObject.Types)
        {
          // We only want to register a BSL trigger for a base entity, otherwise we'll be registering multiple BSL triggers
          // against the same table, which means a single web hook can be executed erroneously multiple times
          if (webHookBusinessObject.Types.Where(type => type != webHookType).Any(type => type.IsAssignableFrom(webHookType)))
            continue;

          // We will always try to default to setting an action to true if possible
          if (bslTriggerTypeList.TryGetValue(webHookType, out var existingActions))
          {
            existingActions.Create = existingActions.Create || actionDto.Create;
            existingActions.Update = existingActions.Update || actionDto.Update;
            existingActions.Delete = existingActions.Delete || actionDto.Delete;
            existingActions.BeforeCreate = existingActions.BeforeCreate || actionDto.BeforeCreate;
            existingActions.BeforeUpdate = existingActions.BeforeUpdate || actionDto.BeforeUpdate;
            existingActions.BeforeDelete = existingActions.BeforeDelete || actionDto.BeforeDelete;
          }
          else
          {
            bslTriggerTypeList.Add(webHookType, actionDto);
          }
        }
      }

      return bslTriggerTypeList;
    }

    private BslTriggerSetBuilder CreateBslTriggerSet(
      Dictionary<Type, BslTriggerActionDto> bslTriggerTypeList)
    {
      var webHookTriggerSet = new BslTriggerSetBuilder();

      foreach (var bslTriggerType in bslTriggerTypeList)
      {
        if (bslTriggerType.Value.Create || bslTriggerType.Value.Update || bslTriggerType.Value.Delete)
        {
          webHookTriggerSet.Add(bslTriggerType.Key, bslTriggerType.Value.Create, bslTriggerType.Value.Update, bslTriggerType.Value.Delete,
            typeof(WebHookBslTriggerPostCommitAction), null, true);
        }

        if (bslTriggerType.Value.BeforeCreate || bslTriggerType.Value.BeforeUpdate || bslTriggerType.Value.BeforeDelete)
        {
          // Note: pre-commit webhooks should always be run in line with the transaction. If in the future BSL triggers change to 
          // allow Before* events to run in the job system, we still need to ensure that pre-commit webhooks are not
          webHookTriggerSet.Add(bslTriggerType.Key, false, false, false, bslTriggerType.Value.BeforeCreate,
            bslTriggerType.Value.BeforeUpdate, bslTriggerType.Value.BeforeDelete,
            typeof(WebHookBslTriggerPreCommitAction), null, false);
        }
      }

      return webHookTriggerSet;
    }

    private void ConfigureBslTriggerSet(IList<BslTriggerEntity> triggerSet)
    {
      var systemToken = userService.CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        bslTriggerManager.ConfigureBslTriggerSet(dataSession, systemToken, WebHookBslCategory, triggerSet);
        transaction.Commit();
      }
    }
  }
}