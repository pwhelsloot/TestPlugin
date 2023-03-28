namespace AMCS.Data.Server.WebHook
{
  using System;
  using System.Linq;
  using Services;
  using Support.Security;
  using System.Collections.Generic;
  using Entity;
  using Plugin;
  using AMCS.Data.Server.SQL.Querying;
  using AMCS.Data.Support;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.WebHook;
  using Data.Configuration;
  using Entity.Webhook;
  using Entity.WebHook;
  using Webhook.Exceptions;
  using AMCS.PluginData.Data.Configuration;

  public class WebHookService : IWebHookService
  {
    private readonly IWebHookCacheService webHookCacheService;
    private readonly IBusinessObjectService businessObjectService;
    private readonly IServiceRootResolver serviceRootResolver;
    private readonly List<WebHookRegistrationDto> webHookRegistrations = new List<WebHookRegistrationDto>();

    public WebHookService(
      IWebHookCacheService webHookCacheService, 
      IBusinessObjectService businessObjectService, 
      IPluginMetadataService pluginMetadataService, 
      IServiceRootResolver serviceRootResolver)
    {
      this.webHookCacheService = webHookCacheService;
      this.businessObjectService = businessObjectService;
      this.serviceRootResolver = serviceRootResolver;
      pluginMetadataService.Register(AddMetadataRegistry);
    }

    public IList<WebHookEntity> GetWebHooks()
    {
      return webHookCacheService.GetWebHooks();
    }

    public void SaveWebHooks(IList<WebHookEntity> webHooks, string category, ISessionToken sessionToken,
      IDataSession dataSession)
    {
      EnsureRestWebHooksPublished();
      
      webHookCacheService.Publish(webHooks, category, sessionToken, dataSession, BroadcastMode.Batch);

      void EnsureRestWebHooksPublished()
      {
        // Assume it's coming from a MEX job if all webhooks InstalledViaRest == false
        if (webHooks.All(webHook => !webHook.InstalledViaRest))
        {
          var existingRestWebHookCriteria = Criteria.For(typeof(WebHookEntity))
            .Add(Expression.Eq(nameof(WebHookEntity.SystemCategory), category))
            .Add(Expression.Eq(nameof(WebHookEntity.InstalledViaRest), true));

          var existingWebHooks = dataSession.GetAllByCriteria<WebHookEntity>(sessionToken, existingRestWebHookCriteria);

          webHooks.AddRange(existingWebHooks);
        }
      }
    }

    public Guid SaveWebHook(PluginWebHook webHook, string systemCategory, ISessionToken userId, IDataSession dataSession)
    {
      var mappedWebHook = MapWebHookToEntity(webHook, systemCategory, Guid.Parse(userId.TenantId));

      // Note: we need to fetch from the DB here instead of the cache, because there's a possibility that
      // the cache hasn't been synced yet and we could end up erroneously deleting web hooks. Going straight
      // to the DB however prevents this problem
      var criteria = Criteria.For(typeof(WebHookEntity))
        .Add(Expression.Eq(nameof(WebHookEntity.SystemCategory), mappedWebHook.SystemCategory));
      var existingWebHooks = dataSession.GetAllByCriteria<WebHookEntity>(userId, criteria);

      var existingWebHook = existingWebHooks.SingleOrDefault(entity =>
        entity.Name == mappedWebHook.Name && entity.TenantId == mappedWebHook.TenantId);
      
      if (existingWebHook != null)
      {
        existingWebHook.Name = mappedWebHook.Name;
        existingWebHook.Format = mappedWebHook.Format;
        existingWebHook.HttpMethod = mappedWebHook.HttpMethod;
        existingWebHook.Url = mappedWebHook.Url;
        existingWebHook.BasicCredentials = mappedWebHook.BasicCredentials;
        existingWebHook.Headers = mappedWebHook.Headers;
        existingWebHook.Filter = mappedWebHook.Filter;
        existingWebHook.Trigger = mappedWebHook.Trigger;
        existingWebHook.InstalledViaRest = true;
      }
      else
      {
        mappedWebHook.InstalledViaRest = true;
        mappedWebHook.GUID = Guid.NewGuid();

        existingWebHooks.Add(mappedWebHook);
      }

      webHookCacheService.Publish(existingWebHooks, systemCategory, userId, dataSession, BroadcastMode.Batch);

      return existingWebHook?.GUID ??
             mappedWebHook.GUID ?? throw new WebHookInstallException("Could not save WebHook");
    }

    public void DeleteWebHook(string webHookGuid, ISessionToken userId, IDataSession dataSession)
    {
      var currentWebHooks = webHookCacheService.GetWebHooks();
      var webHookToDelete = currentWebHooks.SingleOrDefault(entity => entity.GUID == Guid.Parse(webHookGuid));
      
      if (webHookToDelete == null)
      {
        throw new WebHookDeleteException(
          $"Could not find web hook with target plugin with an ID of {webHookGuid}");
      }

      var allOtherWebHooks = currentWebHooks.Where(webHook =>
        webHook.SystemCategory == webHookToDelete.SystemCategory && webHook.GUID != Guid.Parse(webHookGuid)).ToList();
      
      webHookCacheService.Publish(allOtherWebHooks, webHookToDelete.SystemCategory, userId, dataSession, BroadcastMode.Batch);
    }

    public WebHookEntity MapWebHookToEntity(BaseWebHook webHook, string systemCategory, Guid tenantId)
    {
      var businessObjects = businessObjectService.GetAll();
      if (businessObjects.All(businessObject => businessObject.BusinessObject.Name != webHook.GetSanitizedName()))
        throw new WebHookInstallException($"WebHook {webHook.Name} not specified as a business object");

      if (businessObjects
            .SingleOrDefault(businessObject => businessObject.BusinessObject.Name == webHook.GetSanitizedName())
            ?.BusinessObject.AllowWebHooks == false)
      {
        throw new WebHookInstallException($"Business object does not allow {webHook.Name} to be used as a webHook");
      }

      var associatedBusinessObject = businessObjects
        .Single(businessObject => businessObject.BusinessObject.Name == webHook.GetSanitizedName());

      if (!string.IsNullOrWhiteSpace(webHook.Filter) &&
          string.IsNullOrWhiteSpace(associatedBusinessObject.BusinessObject.MappedApiEntity))
      {
        throw new WebHookInstallException("WebHook filters can only be used when a mapped API entity is used");
      }

      if (webHook.Format == WebHookFormat.Full &&
          string.IsNullOrWhiteSpace(associatedBusinessObject.BusinessObject.MappedApiEntity))
      {
        throw new WebHookInstallException("WebHook format of type full must be associated with a mapped API entity");
      }

      if (webHook.Trigger.HasPreCommitTrigger() && webHook.Format == WebHookFormat.Coalesce)
      {
        throw new WebHookInstallException(
          $"Using pre-commit trigger with format coalesce is not supported for WebHook {webHook.Name}");
      }

      var mappedEntity = new WebHookEntity
      {
        SystemCategory = systemCategory,
        Name = webHook.GetSanitizedName(),
        Trigger = webHook.Trigger.ToString(),
        Format = (int)webHook.Format,
        HttpMethod = webHook.HttpMethod,
        Url = webHook.Url,
        Filter = webHook.Filter,
        TenantId = tenantId
      };

      if (!string.IsNullOrEmpty(webHook.BasicCredentials?.Username))
      {
        mappedEntity.BasicCredentials =
          StringEncryptor.DefaultEncryptor.Encrypt(
            $"{webHook.BasicCredentials.Username}:{webHook.BasicCredentials.Password}");
      }

      if (webHook.Headers?.Count > 0)
        mappedEntity.Headers = string.Join(Environment.NewLine, webHook.Headers.Select(header => header).ToArray());

      return mappedEntity;
    }

    public bool ChangeDetectedInWebHook(PluginWebHook webHook, string systemCategory, Guid tenantId)
    {
      var existingWebHook = webHookCacheService.GetWebHooks()
        .SingleOrDefault(hook => hook.SystemCategory == systemCategory && hook.TenantId == tenantId &&
                                 hook.Name == webHook.GetSanitizedName());

      if (existingWebHook == null)
        return true;

      var mappedEntity = MapWebHookToEntity(webHook, existingWebHook.SystemCategory, tenantId);

      mappedEntity.WebHookId = existingWebHook.WebHookId;
      mappedEntity.GUID = existingWebHook.GUID;
      mappedEntity.Environment = serviceRootResolver.ServiceRoot;
      mappedEntity.InstalledViaRest = existingWebHook.InstalledViaRest;

      return !mappedEntity.IsEqualTo(existingWebHook);
    }

    public void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies)
    {
      foreach (var webHookRegistration in webHookRegistrations)
      {
        pluginMetadata.WebHooks.Add(webHookRegistration.WebHook);
      }
    }

    public WebHookRegistrationDto Register(string name, WebHookFormat format, WebHookTrigger trigger, Action<WebHookCallback> callback, string filter)
    {
      var webHookRegistration = new WebHookRegistrationDto(name, format, trigger, callback, filter);
      
      if (webHookRegistrations.Any(w => w.RegistrationKey == webHookRegistration.RegistrationKey))
      {
        throw new InvalidOperationException("Web Hook already registered");
      }
      
      webHookRegistrations.Add(webHookRegistration);

      return webHookRegistration;
    }

    public WebHookRegistrationDto UnRegister(WebHookRegistrationDto webHookRegistration)
    {
      return webHookRegistrations.Remove(webHookRegistration) ? webHookRegistration : null;
    }

    public void ExecuteCallback(WebHookCallback webHookCallback)
    {
      var webHookRegistration = webHookRegistrations.SingleOrDefault(w => w.RegistrationKey == webHookCallback.RegistrationKey);
      webHookRegistration?.Callback(webHookCallback);
    }
  }
}
