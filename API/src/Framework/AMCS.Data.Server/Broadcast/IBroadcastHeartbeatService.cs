using System.Collections.Generic;
using AMCS.Data.Configuration;
using AMCS.WebDiagnostics;

namespace AMCS.Data.Server.Broadcast
{
  public interface IBroadcastHeartbeatService : IDelayedStartup
  {
    IEnumerable<DiagnosticResult> GetDiagnostics();
  }
}