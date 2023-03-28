namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using System;
  using System.Runtime.InteropServices;
  using AMCS.Data;
  using BslTriggers;
  using Entity;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.DataContracts;
  using Microsoft.ApplicationInsights.Extensibility;
  using Webhook.Exceptions;

  [Guid("E1C38B2C-CDAC-4EBB-91F5-5152A255522F")]
  public class WebHookBslTriggerPreCommitAction : IBslAction
  {
    public void Execute(ISessionToken userId, BslTriggerRequest request, IBslActionContext bslActionContext)
    {
      var telemetryClient = DataServices.ResolveKeyed<TelemetryClient>(WebHookConstants.WebHookPreCommitExecutorKey);
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
        Name = $"WebHook Pre Commit BSL Trigger - {request.EntityType}",
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