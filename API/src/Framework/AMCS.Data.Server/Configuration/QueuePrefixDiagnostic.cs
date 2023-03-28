using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Broadcast;
using AMCS.JobSystem.Scheduler.Api;
using AMCS.WebDiagnostics;

namespace AMCS.Data.Server.Configuration
{

  public interface IQueuePrefixDiagnosticService: IDelayedStartup { }

  public class QueuePrefixDiagnosticService: IQueuePrefixDiagnosticService
  {

    public QueuePrefixDiagnosticService()
    {
    }

    private IEnumerable<DiagnosticResult> GetDiagnostics()
    {

      var diagnostics = new List<DiagnosticResult>();
      bool hasServerConfiguration = DataServices.TryResolve<IServerConfiguration>(out var serverConfiguration);

      if (hasServerConfiguration && DataServices.TryResolve<IBroadcastService>(out var _))
      {
        if (string.IsNullOrWhiteSpace(serverConfiguration.Broadcast.QueuePrefix))
        {
          diagnostics.Add(new DiagnosticResult.Failure("Broadcast Service", "Queue prefix was not defined"));
        }
        else
        {
          diagnostics.Add(new DiagnosticResult.Success("Broadcast Service"));
        }
      }

      if (hasServerConfiguration && DataServices.TryResolve<SchedulerClient>(out var _))
      {
        if (string.IsNullOrWhiteSpace(serverConfiguration.JobSystem.QueuePrefix))
        {
          diagnostics.Add(new DiagnosticResult.Failure("Job System", "Queue prefix was not defined"));
        }
        else
        {
          diagnostics.Add(new DiagnosticResult.Success("Job System"));
        }
      
      }
      return diagnostics;
    }

    public void Start()
    {
      if (DataServices.TryResolve<IDiagnosticsManager>(out var diagnosticManager))
        diagnosticManager.Register(GetDiagnostics);
    }
  }
}


