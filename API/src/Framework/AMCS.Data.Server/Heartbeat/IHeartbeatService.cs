namespace AMCS.Data.Server.Heartbeat
{
  using System.Collections.Generic;
  using AMCS.WebDiagnostics;

  public interface IHeartbeatService
  {
    void SetMessageProcessing(string protocolName, string instanceName);
    void SetMessageProcessing(string protocolName, string instanceName, string payload);
    void SetMessageProcessed(string protocolName, string instanceName);
    void SyncConnectionRegistry();
    void FlushConnectionRegistry();
    void SendHeartbeatMessages();
    IEnumerable<DiagnosticResult> GetDiagnostics();
  }
}
