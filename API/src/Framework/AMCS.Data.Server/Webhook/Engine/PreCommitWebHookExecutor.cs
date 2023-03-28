namespace AMCS.Data.Server.Webhook.Engine
{
  using AMCS.Data.Server.WebHook.BslTrigger;
  using AMCS.Data.Entity.WebHook;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Actions;
  using PluginData.Data.WebHook;
  using Entity.Tenants;
  using HttpCallbacks;
  using Plugin;
  using PluginData.Data.MetadataRegistry.Plugins;
  using Services;
  using Validations;
  using WebHook;
  using Webhook.Exceptions;

  internal class PreCommitWebHookExecutor : IWebHookExecutor
  {
    private readonly IPluginRegistryService pluginRegistryService;
    private readonly ITenantManager tenantManager;
    private readonly IEnumerable<IWebHookDataAction> actions = new List<IWebHookDataAction>();

    public PreCommitWebHookExecutor(IPluginRegistryService pluginRegistryService, ITenantManager tenantManager, IEnumerable<IWebHookDataAction> actions)
    {
      this.pluginRegistryService = pluginRegistryService;
      this.tenantManager = tenantManager;
      this.actions = actions;
    }

    public async Task ExecuteAll(List<WebHookEntity> webHooks, WebHookConfiguration configuration)
    {
      var taskList = await GenerateExecutionTasksInStableOrder(webHooks, configuration);
      var results = await Task.WhenAll(taskList);
      
      await DataServices.ResolveKeyed<IWebHookValidation>(configuration.TriggerType.ToString()).Validate(results);

      // We don't process data of any sorts for pre-delete web hooks
      if (configuration.TriggerType == WebHookTrigger.PreDelete)
        return;
      
      var executeDataActions = actions.Select(action => action.Execute(results, configuration));
      await Task.WhenAll(executeDataActions);
    }

    private async Task<List<Task<(WebHookEntity WebHook, HttpResponseMessage Response)>>> GenerateExecutionTasksInStableOrder(
      List<WebHookEntity> webHooks, WebHookConfiguration configuration)
    {
      var list = new List<Task<(WebHookEntity WebHook, HttpResponseMessage Response)>>();
      
      await StableWebHookSort(webHooks, configuration);

      foreach (var webHook in webHooks)
      {
        var format = (WebHookFormat)webHook.Format;

        if (!DataServices.TryResolveKeyed<IWebHookTriggerService>(format, out var callbackService))
          throw new WebHookExecuteException($"Unknown format {format} specified specified for webHook callback factory");

        var task = Task.Run(async () =>
        {
          var response = await callbackService.Execute(webHook, configuration);
          return (webHook, response);
        });

        list.Add(task);
      }

      return list;

      async Task<PluginRegistry> CheckForUpdatedRegistry(ITenant currentTenant, List<WebHookEntity> webHooksCopy)
      {
        var registryUpdated = false;
        var pluginRegistry = pluginRegistryService.Get(currentTenant);
        if (pluginRegistry == null)
        {
          await pluginRegistryService.Update();
          registryUpdated = true;
          pluginRegistry = pluginRegistryService.Get(currentTenant);

          if (pluginRegistry == null)
            throw new WebHookExecuteException("No plugin registry found during webhook execution");
        }

        // If the registry is missing any plugins specified in the list of webhooks, attempt to update registry as long as we
        // didn't attempt to update it above
        var missingRegistry = webHooksCopy.FirstOrDefault(webHook =>
          pluginRegistry.Plugins.All(registry => registry.Name != webHook.SystemCategory));
        if (missingRegistry != null)
        {
          // If we already attempted to update registry, then throw error
          if (registryUpdated)
            throw new WebHookExecuteException(
              $"No registry entry found for {missingRegistry.SystemCategory} for webhook {missingRegistry.Name}");

          await pluginRegistryService.Update();
          pluginRegistry = pluginRegistryService.Get(currentTenant);

          missingRegistry = webHooksCopy.FirstOrDefault(webHook =>
            pluginRegistry.Plugins.All(registry => registry.Name != webHook.SystemCategory));
          
          if (missingRegistry != null)
            throw new WebHookExecuteException(
              $"No registry entry found for {missingRegistry.SystemCategory} for webhook {missingRegistry.Name}");
        }

        return pluginRegistry;
      }

      async Task StableWebHookSort(List<WebHookEntity> pWebHooks, WebHookConfiguration pConfiguration)
      {
        var webHooksCopy = pWebHooks.ToList();
        var tenantId = pConfiguration.TenantId;
        var currentTenant = tenantManager.GetTenant(tenantId);

        if (currentTenant == null)
        {
          throw new WebHookExecuteException(
            $"Tenant {tenantId} expected for WebHook entity execution of {WebHookUtils.GetWebHookExecuteTriggerType(pConfiguration.TriggerType)}");
        }

        var pluginRegistry = await CheckForUpdatedRegistry(currentTenant, webHooksCopy);

        foreach (var webHook in webHooksCopy)
        {
          var currentPlugin = pluginRegistry.Plugins.SingleOrDefault(plgin => plgin.Name == webHook.SystemCategory);
          if (currentPlugin == null)
          {
            await pluginRegistryService.Update();
            pluginRegistry = pluginRegistryService.Get(currentTenant);
            currentPlugin = pluginRegistry.Plugins.SingleOrDefault(plgin => plgin.Name == webHook.SystemCategory);

            if (currentPlugin == null)
              throw new WebHookExecuteException(
                $"Could not find plugin entry of {webHook.SystemCategory} in {JsonSerializer.Serialize(pluginRegistry)}");
          }

          foreach (var otherPlugin in pluginRegistry.Plugins.Where(plgin => plgin.Name != webHook.SystemCategory ))
          {
            if (otherPlugin.Dependencies.All(dependency => dependency.Name != currentPlugin.Name)) 
              continue;

            pWebHooks.Remove(webHook);
            pWebHooks.Add(webHook);
          }
        }
      }
    }
  }
}