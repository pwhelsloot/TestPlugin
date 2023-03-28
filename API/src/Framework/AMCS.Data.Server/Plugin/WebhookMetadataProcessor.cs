namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Entity.WebHook;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.WebHook;
  using log4net;
  using PluginData.Data.WebHook;

  public class WebHookMetadataProcessor : IWebHookMetadataProcessor
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(WebHookMetadataProcessor));

    private readonly IWebHookService webHookService;
    private readonly ITenantManager tenantManager;

    public WebHookMetadataProcessor(IWebHookService webHookService, ITenantManager tenantManager)
    {
      this.webHookService = webHookService;
      this.tenantManager = tenantManager;
    }

    public void Process(
      IList<PluginData.Data.Metadata.WebHooks.WebHook> webHooks, 
      string fullyQualifiedPluginName, 
      ISessionToken sessionToken, 
      IDataSession dataSession)
    {
      var existingWebHooks = dataSession.GetAllByTemplate(sessionToken,
        new WebHookEntity {SystemCategory = fullyQualifiedPluginName}, false);
      var webHookList = new List<WebHookEntity>();

      // Products that are multi-tenant will be responsible for ensuring MEX is initialized for each new tenant added
      var otherTenantWebHooks = existingWebHooks.Where(webHook => webHook.TenantId != Guid.Parse(sessionToken.TenantId));
      webHookList.AddRange(otherTenantWebHooks);

      var currentTenants = tenantManager.GetAllTenants();
      if (currentTenants.All(tenant => !string.Equals(tenant.TenantId, sessionToken.TenantId, StringComparison.OrdinalIgnoreCase)))
        return;

      foreach (var webHook in webHooks)
      {
        var pluginWebHook = webHookService.MapWebHookToEntity(webHook, fullyQualifiedPluginName, Guid.Parse(sessionToken.TenantId));

        var dbWebHook = existingWebHooks.SingleOrDefault(hook =>
          string.Equals(hook.Name, webHook.GetSanitizedName(), StringComparison.OrdinalIgnoreCase) 
          && webHook.Trigger.HasFlag((WebHookTrigger)Enum.Parse(typeof(WebHookTrigger), hook.Trigger)) 
          && hook.TenantId == Guid.Parse(sessionToken.TenantId));

        pluginWebHook.WebHookId = dbWebHook?.Id32;
        webHookList.Add(pluginWebHook);
      }

      webHookService.SaveWebHooks(webHookList, fullyQualifiedPluginName, sessionToken, dataSession);
    }
  }
}