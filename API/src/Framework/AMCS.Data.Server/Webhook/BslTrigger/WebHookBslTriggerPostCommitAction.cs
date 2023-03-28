namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using System;
  using System.Runtime.InteropServices;
  using AMCS.Data.Server.BslTriggers;
  using Entity;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.DataContracts;
  using Microsoft.ApplicationInsights.Extensibility;
  using log4net;

  [Guid("9A7C6C3F-96F2-4456-A5AD-0EB2FC3AACB5")]
  public class WebHookBslTriggerPostCommitAction : IBslAction
  {
    public void Execute(ISessionToken userId, BslTriggerRequest request, IBslActionContext bslActionContext)
    {
      var telemetryClient = DataServices.ResolveKeyed<TelemetryClient>(WebHookConstants.WebHookPostCommitExecutorKey);
      using (var operation = TrackRequest(request, telemetryClient))
      {
        try
        {
          DataServices.Resolve<IWebHookBslTriggerExecuteService>().Execute(userId, request, bslActionContext);
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
    
    private IOperationHolder<RequestTelemetry> TrackRequest(BslTriggerRequest request, TelemetryClient telemetryClient)
    {
#if NETFRAMEWORK
      if (TelemetryConfiguration.Active.DisableTelemetry)
#else
      if (telemetryClient.TelemetryConfiguration.DisableTelemetry)
#endif
        return null;

      var requestTelemetry = new RequestTelemetry
      {
        Name = $"WebHook Post Commit BSL Trigger - {request.EntityType}",
        Properties =
        {
          { "Action", request.Action.ToString() },
          { "ID", request.Id.ToString() },
          { "GUID", request.GUID?.ToString() }
        }
      };

      return telemetryClient.StartOperation(requestTelemetry);
    }
  }
}