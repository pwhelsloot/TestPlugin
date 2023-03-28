namespace AMCS.Data.Server.Webhook.Engine.HttpCallbacks
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using AMCS.Data.Entity;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server.Configuration;
  using AMCS.Data.Server.Http;
  using AMCS.Data.Server.Util;
  using AMCS.Data.Server.WebHook;
  using AMCS.Data.Server.WebHook.BslTrigger;
  using AMCS.Data.Server.Webhook.Exceptions;
  using Entity.Webhook;
  using Newtonsoft.Json;
  using PluginData.Data.WebHook;

  internal class WebHookFullService : IWebHookTriggerService
  {
    private readonly IAmcsHttpClientFactory amcsHttpClientFactory;
    private readonly IHttpCallbackService httpCallbackService;
    private readonly IServerConfiguration serverConfiguration;

    public WebHookFullService(
      IHttpCallbackService httpCallbackService,
      IAmcsHttpClientFactory amcsHttpClientFactory,
      IServerConfiguration serverConfiguration)
    {
      this.amcsHttpClientFactory = amcsHttpClientFactory;
      this.httpCallbackService = httpCallbackService;
      this.serverConfiguration = serverConfiguration;
    }

    public Task<HttpResponseMessage> Execute(WebHookEntity entity, WebHookConfiguration configuration)
    {
      // We want to make sure to append to existing url parameters if they exist
      var sanitizedUrl = QueryHelper.AddQueryString(entity.Url, new Dictionary<string, string>
      {
        {"id", WebHookUtils.GetEntityGuid(configuration).ToString()},
        {"action", WebHookUtils.GetWebHookDescription(configuration.TriggerType)},
        {"txid", configuration.TransactionId}
      });

      var serializedEntity = JsonConvert.SerializeObject(configuration.EntityObject);
      var triggerType = WebHookUtils.GetWebHookExecuteTriggerType(configuration.TriggerType);

      if (!DataServices.TryResolveKeyed<IHttpCallback>(triggerType, out var httpCallback))
        throw new WebHookExecuteException($"Could not find associated http callback for {triggerType}");

      return httpCallback.TrackCallback(entity,
        async () => await GenerateHttpCallback(entity, serializedEntity, configuration.TriggerType, sanitizedUrl));
    }

    private async Task<HttpResponseMessage> GenerateHttpCallback(WebHookEntity entity, string content, WebHookTrigger trigger, string sanitizedUrl)
    {
      var httpClient = amcsHttpClientFactory.CreatePlatformClient();

      var requestMessage = httpCallbackService.GenerateRequestMessage(entity, new Uri(sanitizedUrl));
      
      if (!string.IsNullOrEmpty(content))
        requestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");

      CancellationToken cancellationToken = default;
      if (trigger.HasPreCommitTrigger())
      {
        var maxTimeout = serverConfiguration.MaxPreCommitExecutionDurationInSeconds ?? 30;
        var source = new CancellationTokenSource(TimeSpan.FromSeconds(maxTimeout));
        cancellationToken = source.Token;
      }

      var result = await httpClient.SendAsync(requestMessage, cancellationToken);
      return result;
    }
  }
}