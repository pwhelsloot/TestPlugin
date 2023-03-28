namespace AMCS.Data.Server.Plugin.Core
{
  using System;
  using System.Text;
  using Newtonsoft.Json;
  using System.Net.Http;
  using System.Threading.Tasks;
  using System.Collections.Generic;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry;
  using AMCS.PluginData.Services;
  using AMCS.Data.Entity.Tenants;
  using Http;
  using log4net;

  public class CorePluginHttpService : ICorePluginHttpService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CorePluginHttpService));

    private readonly IHttpClient httpClient;
    private readonly IHttpRetryService httpRetryService;
    private readonly IPluginSerializationService pluginSerializationService;

    private readonly Dictionary<MetadataRegistryType, string> registryUrlMaps =
      new Dictionary<MetadataRegistryType, string>
      {
        {MetadataRegistryType.WebHooks, "web-hooks.xml"},
        {MetadataRegistryType.BusinessObjects, "business-objects.xml"},
        {MetadataRegistryType.WorkflowActivities, "workflow-activities.xml"},
        {MetadataRegistryType.UiComponents, "ui-components.xml"},
        {MetadataRegistryType.Plugins, "plugins.xml"}
      };

    public CorePluginHttpService(
      IHttpClient httpClient,
      IHttpRetryService httpRetryService,
      IPluginSerializationService pluginSerializationService)
    {
      this.httpClient = httpClient;
      this.httpRetryService = httpRetryService;
      this.pluginSerializationService = pluginSerializationService;
    }

    public async Task<T> GetMetadataRegistry<T>(ITenant tenant, MetadataRegistryType metadataRegistryType)
      where T : IMetadataRegistryItem
    {
      var requestCounter = 0;
      
      var responseMessage =
        await httpRetryService.ExecuteHttpWithRetry(ExecuteRestApiCall);

      responseMessage.EnsureSuccessStatusCode();
      var content = await responseMessage.Content.ReadAsStringAsync();
      var metadataRegistry = pluginSerializationService.Deserialize<T>(content);
      
      return metadataRegistry;

      async Task<HttpResponseMessage> ExecuteRestApiCall()
      {
        var message = new HttpRequestMessage(
          HttpMethod.Get,
          $"{tenant.CoreAppServiceRoot.TrimEnd('/')}/plugins/metadata/{registryUrlMaps[metadataRegistryType]}");

        Logger.Info(
          $"Attempt {++requestCounter} of {HttpRetryService.MaxRetryAttempts}: Getting metadata registry {metadataRegistryType} from {message.RequestUri}");
        
        return await SendMessageAsync(message, tenant);
      }
    }

    public async Task CreateWorkflowInstance(
      ITenant tenant,
      string workflowProviderName,
      Guid workflowDefinitionId,
      string userContext,
      string startParameters)
    {
      var requestCounter = 0;

      var responseMessage =
        await httpRetryService.ExecuteHttpWithRetry(ExecuteRestApiCall);

      responseMessage.EnsureSuccessStatusCode();
      
      async Task<HttpResponseMessage> ExecuteRestApiCall()
      {
        var message = new HttpRequestMessage(
          HttpMethod.Post,
          $"{tenant.CoreAppServiceRoot.TrimEnd('/')}/services/api/workflow/Instances");

        message.Content = new StringContent(
          JsonConvert.SerializeObject(new
          {
            ProviderName = workflowProviderName,
            WorkflowDefinitionId = workflowDefinitionId,
            State = startParameters,
            UserContext = userContext,
            StatusId = 0,
            WorkflowInstanceId = Guid.Empty,
            tenant.TenantId,
            Started = DateTime.MinValue.ToString("yyyy-MM-dd")
          }), Encoding.UTF8, "application/json");

        Logger.Info(
          $"Attempt {++requestCounter} of {HttpRetryService.MaxRetryAttempts}: Create workflow definition {workflowDefinitionId} with start parameters {startParameters} on workflow provider {workflowProviderName}");

        return await SendMessageAsync(message, tenant);
      }
    }

    private async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage message, ITenant tenant)
    {
      var response = await httpClient.SendAsyncWithCoreCredentials(message, tenant);
      return response;
    }
  }
}