namespace AMCS.Data.Server.Webhook.Engine.Actions
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server.WebHook.BslTrigger;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Entity;
  using Entity.Plugin;
  using Entity.UserDefinedField;
  using Exceptions;
  using log4net;
  using Plugin;
  using UserDefinedField;

  internal class WebHookUdfDataAction : IWebHookDataAction
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(WebHookUdfDataAction));
    
    private readonly IPluginSystem pluginSystem;

    public WebHookUdfDataAction(IPluginSystem pluginSystem)
    {
      this.pluginSystem = pluginSystem;
    }

    public async Task Execute((WebHookEntity WebHook, HttpResponseMessage Response)[] data, WebHookConfiguration configuration)
    {
      // UDF might not be configured for the given system, which is a perfectly reasonable setup
      if (!DataServices.TryResolve<IUdfDataService>(out var udfDataService) ||
          !DataServices.TryResolve<IUdfMetadataCacheService>(out var udfMetadataCacheService))
      {
        Log.Debug($"UDF Services not registered for {pluginSystem.FullyQualifiedName}; Skipping UDF data action");
        return;
      }

      if (!configuration.EntityObject.GUID.HasValue)
      {
        Log.Warn($"EntityObject must have GUID to insert UDF data for {configuration.EntityObject.GetType()}");
        return;
      }

      var results = new List<(string, Dictionary<string, object>)>();

      IEnumerable<Task<(WebHookEntity WebHook, string Content)>> contentDataTasks = data.Select(async item =>
      {
        var contentResponse =  item.Response.Content.Headers.ContentType.MediaType == "application/json"
          ? await item.Response.Content.ReadAsStringAsync()
          : string.Empty;

        return (item.WebHook, contentResponse);
      });

      var contentData = await Task.WhenAll(contentDataTasks);

      // Filtering the content string on "$udf" could certainly provide false-positives, e.g., an entity has a property
      // that == "$udf", but those will ultimately be ignored. 
      foreach (var contentItem in contentData.Where(item => item.Content.Contains(UdfConstants.UdfProperty)))
      {
        if (contentItem.WebHook.SystemCategory == PluginHelper.GetCorePluginFullName())
          ParseCoreUdfData(udfMetadataCacheService, contentItem, results);
        else
          ParseNonCoreUdfData(udfMetadataCacheService, contentItem, results);
      }

      if (results.Any())
      {
        udfDataService.Write(configuration.EntityObject.GUID.Value, configuration.EntityObject.GetType(), results, configuration.UserId, configuration.ExistingSession);
      }
    }

    /// If the web hook is being executed from a specific plugin, e.g., scale, then we only allow scale properties to be set
    private void ParseNonCoreUdfData(IUdfMetadataCacheService udfMetadataCacheService,
      (WebHookEntity WebHook, string Content) contentItem, List<(string, Dictionary<string, object>)> results)
    {
      var items = udfMetadataCacheService.GetUdfMetadata().Namespaces
        .SingleOrDefault(item => item.Name == contentItem.WebHook.SystemCategory);

      if (items == null || !items.Fields.Any())
      {
        Log.Debug(
          $"No UDF metadata installed for plugin {contentItem.WebHook.SystemCategory} in {pluginSystem.FullyQualifiedName} for web hook {contentItem.WebHook.Name}");
        return;
      }

      using (var jsonDocument = JsonDocument.Parse(contentItem.Content))
      {
        var udfRoot = GetUdfRootData(jsonDocument, contentItem);

        if (!udfRoot.Value.TryGetProperty(contentItem.WebHook.SystemCategory, out var udfPluginRoot) ||
            udfPluginRoot.ValueKind != JsonValueKind.Object)
        {
          throw new WebHookExecuteException(
            $"No {contentItem.WebHook.SystemCategory} UDF data found for web hook {contentItem.WebHook.SystemCategory}:{contentItem.WebHook.Name}");
        }

        var udfData = udfPluginRoot.EnumerateObject().Select(item => (item.Name, item.Value));
        var udfFieldItems = new Dictionary<string, object>();

        ParseUdfProperties(contentItem.WebHook.SystemCategory, contentItem.WebHook.Name, results, udfData, items, udfFieldItems);
      }
    }

    /// If the web hook is being executed from Core, then we allow all existing UDF properties to be set
    private void ParseCoreUdfData(IUdfMetadataCacheService udfMetadataCacheService,
      (WebHookEntity WebHook, string Content) contentItem, List<(string, Dictionary<string, object>)> results)
    {
      using (var jsonDocument = JsonDocument.Parse(contentItem.Content))
      {
        var udfRoot = GetUdfRootData(jsonDocument, contentItem);
        var cachedItems = udfMetadataCacheService.GetUdfMetadata().Namespaces;

        foreach (var plugin in udfRoot.Value.EnumerateObject())
        {
          var existingUdfItems = cachedItems.SingleOrDefault(item => item.Name == plugin.Name);
          if (existingUdfItems == null || !existingUdfItems.Fields.Any())
          {
            throw new WebHookExecuteException(
              $"No UDF metadata installed for plugin {contentItem.WebHook.SystemCategory} in {pluginSystem.FullyQualifiedName} for web hook {contentItem.WebHook.Name}");
          }

          var udfData = plugin.Value.EnumerateObject().Select(item => (item.Name, item.Value));
          var udfFieldItems = new Dictionary<string, object>();

          ParseUdfProperties(plugin.Name, contentItem.WebHook.Name, results, udfData, existingUdfItems, udfFieldItems);
        }
      }
    }

    private static void ParseUdfProperties(string systemCategory, string webhookName, List<(string, Dictionary<string, object>)> results,
      IEnumerable<(string Name, JsonElement Value)> udfData, IUdfNamespace items, Dictionary<string, object> udfFieldItems)
    {
      foreach (var udf in udfData)
      {
        if (items.Fields.All(field => field.FieldName != udf.Name))
        {
          throw new WebHookExecuteException(
            $"No UDF field found for {systemCategory}:{udf.Name}");
        }

        udfFieldItems[udf.Name] = udf.Value.ToString();
      }

      if (udfFieldItems.Any())
        results.Add((systemCategory, udfFieldItems));
    }

    private JsonProperty GetUdfRootData(JsonDocument jsonDocument, (WebHookEntity WebHook, string Content) contentItem)
    {
      var udfRoot = jsonDocument.RootElement.EnumerateObject()
        .SingleOrDefault(item => item.NameEquals(UdfConstants.UdfProperty));

      if (udfRoot.Value.ValueKind != JsonValueKind.Object)
      {
        throw new WebHookExecuteException(
          $"Object parsed with {UdfConstants.UdfProperty} but invalid value kind of {udfRoot.Value.ValueKind} found for web hook {contentItem.WebHook.SystemCategory}:{contentItem.WebHook.Name}");
      }

      return udfRoot;
    }
  }
}