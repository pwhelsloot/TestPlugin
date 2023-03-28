namespace AMCS.Data.Server.Webhook.Engine.HttpCallbacks
{
  using System;
  using System.Net.Http;
  using System.Threading.Tasks;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server.Http;
  using AMCS.Data.Server.WebHook.BslTrigger;
  using AMCS.PluginData.Data.WebHook;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.DataContracts;
  using Microsoft.ApplicationInsights.Extensibility;
  using WebHook;

  internal class HttpPreCommitCallback : IHttpCallback
  {
    private readonly IAmcsHttpClientFactory httpClientFactory;
    private readonly IHttpCallbackService httpCallbackService;
    private readonly TelemetryClient telemetryClient;

    public HttpPreCommitCallback(IAmcsHttpClientFactory httpClientFactory, IHttpCallbackService httpCallbackService)
    {
      this.httpClientFactory = httpClientFactory;
      this.httpCallbackService = httpCallbackService;
      this.telemetryClient = DataServices.ResolveKeyed<TelemetryClient>(WebHookConstants.WebHookPreCommitExecutorKey);
    }

    public Task<HttpResponseMessage> TrackCallback(WebHookEntity entity, Func<Task<HttpResponseMessage>> callback)
    {
      using (var operation = TrackRequest(entity))
      {
        try
        {
          return callback();
        }
        catch (Exception ex)
        {
          if (operation != null)
          {
            operation.Telemetry.Success = false;
            telemetryClient.TrackException(ex);
          }

          throw;
        }
      }
    }

    private IOperationHolder<RequestTelemetry> TrackRequest(WebHookEntity webHook)
    {
#if NETFRAMEWORK
      if (TelemetryConfiguration.Active.DisableTelemetry)
#else
      if (telemetryClient.TelemetryConfiguration.DisableTelemetry)
#endif
        return null;

      var requestTelemetry = new RequestTelemetry
      {
        Name = $"PRE-COMMIT WebHook - {webHook.Name}",
        Properties =
        {
          { "Source Plugin", webHook.SystemCategory },
          { "WebHook Format", ((WebHookFormat)webHook.Format).ToString() }
        }
      };

      return telemetryClient.StartOperation(requestTelemetry);
    }
  }
}
