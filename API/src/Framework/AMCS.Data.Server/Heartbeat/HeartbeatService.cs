namespace AMCS.Data.Server.Heartbeat
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using AMCS.ApiService.Abstractions.CommsServer;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Heartbeat;
  using AMCS.Data.Server.Services;
  using log4net;
  using AMCS.WebDiagnostics;

  public class HeartbeatService : IHeartbeatService, IDelayedStartup
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(HeartbeatService));

    private readonly double timeout;
    private readonly System.Timers.Timer diagnosticsDelay;
    private readonly IConnectionRegistry connectionRegistry;
    private Timer heartbeatTimer;
    private readonly TimeSpan heartbeatTimerTimeout;
    private readonly TimeSpan gracePeriod;

    private IList<ICommServerConnection> commServerConnections = new List<ICommServerConnection>();

    public HeartbeatService(IConnectionRegistry connectionRegistry)
      : this(connectionRegistry, TimeSpan.FromMinutes(5).TotalMilliseconds)
    {

    }

    public HeartbeatService(IConnectionRegistry connectionRegistry, double timeoutMilliseconds)
    {
      this.connectionRegistry = connectionRegistry;
      heartbeatTimerTimeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

      timeout = heartbeatTimerTimeout.TotalMinutes;
      gracePeriod = TimeSpan.FromMinutes(timeout + timeout / 2);
      diagnosticsDelay = new System.Timers.Timer(gracePeriod.TotalMilliseconds);
    }

    public void Start()
    {
      if (heartbeatTimer != null)
        return;

      diagnosticsDelay.AutoReset = false;
      diagnosticsDelay.Start();
      SyncConnectionRegistry();

      heartbeatTimer = new Timer(state =>
      {
        try
        {
          FlushConnectionRegistry();
          SendHeartbeatMessages();
        }
        catch (Exception ex)
        {
          Logger.Error("There was an error executing the heartbeat timed job.", ex);
        }
      }, null, heartbeatTimerTimeout, heartbeatTimerTimeout);

      if (DataServices.TryResolve<IDiagnosticsManager>(out var diagnosticsManager))
        diagnosticsManager.Register(GetDiagnostics);
    }

    public void SetMessageProcessing(string protocolName, string instanceName)
    {
      SetMessageProcessing(protocolName, instanceName, null);
    }

    public void SetMessageProcessing(string protocolName, string instanceName, string payload)
    {
      SetConnectionStatus(protocolName, instanceName, HeartbeatConnectionStatus.Processing, payload);
    }

    public void SetMessageProcessed(string protocolName, string instanceName)
    {
      SetConnectionStatus(protocolName, instanceName, HeartbeatConnectionStatus.Ready, null);
    }

    private void SetConnectionStatus(string protocolName, string instanceName,
      HeartbeatConnectionStatus connectionStatus, string payload)
    {
      var registryConnections = GetRegistryConnections();

      var connections = registryConnections
        .Where(connection => connection.CommsServerClient.ConnectionState == CommsServerConnectionState.Connected &&
                             connection.ProtocolName == protocolName &&
                             connection.InstanceName == instanceName)
        .ToList();

      var timestamp = DateTime.UtcNow;
      foreach (var connection in connections)
      {
        connection.ConnectionStatus = connectionStatus;
        connection.TimeStamp = timestamp;
        SetHeartbeatLatency(connection, payload);
      }
    }

    private static void SetHeartbeatLatency(ICommServerConnection connection, string payload)
    {
      if (string.IsNullOrWhiteSpace(payload))
        return;

      var heartbeatLatency = HeartbeatLatencyMessage.ReadPayload(payload);
      if (heartbeatLatency?.Created != null)
      {
        connection.HeartbeatLatencyInSeconds = (int)(DateTime.UtcNow - heartbeatLatency.Created).TotalSeconds;
      }
    }

    public void FlushConnectionRegistry()
    {
      var systemToken = DataServices.Resolve<IUserService>().CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        try
        {
          var registryConnections = GetRegistryConnections();
          var heartbeatConnections = GetHeartbeatConnections(systemToken, dataSession);

          ProcessHeartbeatConnections(heartbeatConnections, registryConnections);

          dataSession.BulkSave(systemToken, heartbeatConnections);
          transaction.Commit();
        }
        catch (Exception ex)
        {
          Logger.Error("There was an error flushing the heartbeat service connection registry.", ex);
        }
      }
    }

    private static void ProcessHeartbeatConnections(
      List<HeartbeatConnection> heartbeatConnections,
      IList<ICommServerConnection> registryConnections)
    {
      foreach (var heartbeatConnection in heartbeatConnections)
      {
        var registryConnection = registryConnections.FirstOrDefault(commServerConnection =>
          commServerConnection != null &&
          commServerConnection.ProtocolName == heartbeatConnection.ProtocolName &&
          commServerConnection.InstanceName == heartbeatConnection.InstanceName);

        if (registryConnection != null)
        {
          // if not connected it means we're running on scaled out instances and this instance does not have the connection
          if (registryConnection.CommsServerClient?.ConnectionState != CommsServerConnectionState.Connected)
            continue;
          
          if (registryConnection.ConnectionStatus == HeartbeatConnectionStatus.Unknown)
          {
            registryConnection.ConnectionStatus = HeartbeatConnectionStatus.Ready;
          }

          heartbeatConnection.Status = registryConnection.ConnectionStatus;
          heartbeatConnection.Timestamp = registryConnection.ConnectionStatus == HeartbeatConnectionStatus.Processing
            ? DateTime.UtcNow
            : registryConnection.TimeStamp;

          if (registryConnection.HeartbeatLatencyInSeconds != null)
          {
            heartbeatConnection.HeartbeatLatencyInSeconds = registryConnection.HeartbeatLatencyInSeconds;
          }
        }
        else
        {
          heartbeatConnection.Status = HeartbeatConnectionStatus.Unknown;
          heartbeatConnection.Timestamp = DateTime.UtcNow;
        }
      }
    }

    public void SendHeartbeatMessages()
    {
      var registryConnections = GetRegistryConnections()
        .Where(connection => connection.ConnectionStatus != HeartbeatConnectionStatus.Unknown &&
                             connection.CommsServerClient != null);
      
      foreach (var registryConnection in registryConnections)
      {
        try
        {
          registryConnection.SendHeartbeat(HeartbeatLatencyMessage.CreatePayload());
        }
        catch (Exception ex)
        {
          Logger.Error(
            $"There was an error sending a heartbeat for Protocol: {registryConnection.ProtocolName}, Instance: {registryConnection.InstanceName}.",
            ex);
        }
      }
    }

    public void SyncConnectionRegistry()
    {
      var systemToken = DataServices.Resolve<IUserService>().CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        try
        {
          var heartbeatConnections = GetHeartbeatConnections(systemToken, dataSession);
          var registryConnections = GetRegistryConnections();

          SyncDbConnections(registryConnections, heartbeatConnections, dataSession, systemToken);
          SyncRegistryConnections(registryConnections, heartbeatConnections, dataSession, systemToken);

          transaction.Commit();
        }
        catch (Exception ex)
        {
          Logger.Error("There was an error syncing the heartbeat service connection registry.", ex);
        }
      }
    }

    private static void SyncRegistryConnections(IList<ICommServerConnection> commServerConnections,
      IList<HeartbeatConnection> heartbeatConnections, IDataSession dataSession, ISessionToken sessionToken)
    {
      var updatedConnections =
        (from commServerConnection in commServerConnections
          where heartbeatConnections.FirstOrDefault(heartbeatConnection =>
            commServerConnection.ProtocolName == heartbeatConnection.ProtocolName &&
            commServerConnection.InstanceName == heartbeatConnection.InstanceName) == null
          select new HeartbeatConnection
          {
            ProtocolName = commServerConnection.ProtocolName,
            InstanceName = commServerConnection.InstanceName,
            Status = HeartbeatConnectionStatus.Unknown,
            Timestamp = DateTime.UtcNow
          }).ToList();

      if (updatedConnections.Count > 0)
      {
        dataSession.BulkSave(sessionToken, updatedConnections);
      }
    }

    private static void SyncDbConnections(IList<ICommServerConnection> commServerConnections,
      IList<HeartbeatConnection> heartbeatConnections, IDataSession dataSession, ISessionToken sessionToken)
    {
      foreach (var heartbeatConnection in heartbeatConnections)
      {
        if (commServerConnections.FirstOrDefault(commServerConnection =>
          commServerConnection.ProtocolName == heartbeatConnection.ProtocolName &&
          commServerConnection.InstanceName == heartbeatConnection.InstanceName) == null)
        {
          dataSession.Delete(sessionToken, heartbeatConnection, false);
        }
      }
    }

    private IList<ICommServerConnection> GetRegistryConnections()
    {
      var registryConnections = connectionRegistry
        .GetCommServerConnections();

      var uniqueList = new Dictionary<string, string>();
      foreach (var registryConnection in registryConnections)
      {
        var hash = $"{registryConnection.InstanceName}{registryConnection.ProtocolName}";
        if (uniqueList.ContainsKey(hash))
        {
          Logger.Error(
            $"Heartbeat connection - Instance: {registryConnection.InstanceName} Protocol: {registryConnection.ProtocolName} was specified more than once.");

          throw new HeartbeatDuplicateConfigurationException(
            $"Heartbeat connection - Instance: {registryConnection.InstanceName} Protocol: {registryConnection.ProtocolName} was specified more than once.");
        }
        uniqueList.Add(hash, hash);
      }

      foreach (var commServerConnection in commServerConnections)
      {
        var update = registryConnections.FirstOrDefault(registryConnection =>
          registryConnection.ProtocolName == commServerConnection.ProtocolName &&
          registryConnection.InstanceName == commServerConnection.InstanceName);

        if (update == null)
        {
          Logger.Warn(
            $"A Heartbeat Connection Record was found in the database that is missing from the Connection Registry. Protocol: {commServerConnection.ProtocolName}, Instance: {commServerConnection.InstanceName}");

          continue;
        }

        update.ConnectionStatus = commServerConnection.ConnectionStatus;
        update.HeartbeatLatencyInSeconds = commServerConnection.HeartbeatLatencyInSeconds;
        update.TimeStamp = commServerConnection.TimeStamp;
      }

      commServerConnections = registryConnections;
      return commServerConnections;
    }

    public IEnumerable<DiagnosticResult> GetDiagnostics()
    {
      if (diagnosticsDelay.Enabled)
      {
        yield return new DiagnosticResult.Success("Heartbeat",
          $"Grace Period: The heartbeat service will report diagnostics after {TimeSpan.FromMinutes(gracePeriod.TotalMinutes):g}.");

        yield break;
      }

      var systemToken = DataServices.Resolve<IUserService>().CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        var registryConnections = GetRegistryConnections();
        var heartbeatConnections = GetHeartbeatConnections(systemToken, dataSession);

        transaction.Commit();

        foreach (var registryConnection in registryConnections)
        {
          var instanceName = string.IsNullOrEmpty(registryConnection.InstanceName) ? "NA" : registryConnection.InstanceName;

          // comm server client not configured
          if (registryConnection.CommsServerClient == null)
          {
            Logger.Error(
              $"Diagnostics Failure - Heartbeat - {registryConnection.ProtocolName} - {instanceName}. No Comms Server Client has been provided for this protocol.");

            yield return new DiagnosticResult.Failure($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
              "No Comms Server Client has been provided for this protocol.");
            continue;
          }

          // push client without alternate transport
          if (registryConnection.CommsServerClient.ConnectionState == CommsServerConnectionState.NotSupported)
          {
            Logger.Error(
              $"Diagnostics Failure - Heartbeat - {registryConnection.ProtocolName} - {registryConnection.InstanceName}. The heartbeat service does not support this Comms Server Client connection.");

            yield return new DiagnosticResult.Failure($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
              "The heartbeat service does not support this Comms Server Client connection.");
            continue;
          }

          var heartbeatConnection = heartbeatConnections.SingleOrDefault(connection =>
            connection.ProtocolName == registryConnection.ProtocolName &&
            connection.InstanceName == registryConnection.InstanceName);

          // misconfiguration, no db record exists
          if (heartbeatConnection == null)
          {
            Logger.Error(
              $"Diagnostics Failure - Heartbeat - {registryConnection.ProtocolName} - {instanceName}. No corresponding record was found in the database. This is usually a sign of misconfiguration in the Connection Registry.");

            yield return new DiagnosticResult.Failure($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
              "No corresponding record was found in the database. This is usually a sign of misconfiguration in the Connection Registry.");
            continue;
          }

          // unknown status in db means no messages have been received
          if (heartbeatConnection.Status == HeartbeatConnectionStatus.Unknown)
          {
            Logger.Error(
              $"Diagnostics Failure - Heartbeat - {registryConnection.ProtocolName} - {instanceName}. Unknown Status. No messages have been received - {heartbeatConnection.PrintStatusTimeoutAndLatency()}.");

            yield return new DiagnosticResult.Failure($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
              $"Unknown Status. No messages have been received - {heartbeatConnection.PrintStatusTimeoutAndLatency()}.");
            continue;
          }

          // max latency exceeded
          if (registryConnection.HeartbeatLatencyInSeconds != null && registryConnection.HeartbeatLatencyInSeconds >
            connectionRegistry.MaxHeartbeatLatency.TotalSeconds)
          {
            Logger.Error(
              $"Diagnostics Failure - Heartbeat - {registryConnection.ProtocolName} - {instanceName}. Latency is over allowed threshold of {TimeSpan.FromMinutes(connectionRegistry.MaxHeartbeatLatency.TotalMinutes):g} - {heartbeatConnection.PrintStatusTimeoutAndLatency()}.");

            yield return new DiagnosticResult.Failure($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
              $"Latency is over allowed threshold of {TimeSpan.FromMinutes(connectionRegistry.MaxHeartbeatLatency.TotalMinutes):g} - {heartbeatConnection.PrintStatusTimeoutAndLatency()}.");
            continue;
          }

          // max timestamp exceeded 
          if (DateTime.UtcNow - registryConnection.TimeStamp > gracePeriod)
          {
            Logger.Error(
              $"Diagnostics Failure - Heartbeat - {registryConnection.ProtocolName} - {instanceName}. Timestamp is over allowed threshold of {TimeSpan.FromMinutes(connectionRegistry.MaxHeartbeatLatency.TotalMinutes):g} - {heartbeatConnection.PrintStatusTimeoutAndLatency()}.");

            yield return new DiagnosticResult.Failure($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
              $"Timestamp is over allowed threshold of {TimeSpan.FromMinutes(connectionRegistry.MaxHeartbeatLatency.TotalMinutes):g} - {heartbeatConnection.PrintStatusTimeoutAndLatency()}.");
            continue;
          }

          yield return new DiagnosticResult.Success($"Heartbeat - {registryConnection.ProtocolName} - {instanceName}",
            $"{heartbeatConnection.PrintStatusTimeoutAndLatency()}.");
        }
      }
    }

    private static List<HeartbeatConnection> GetHeartbeatConnections(ISessionToken sessionToken, IDataSession dataSession)
    {
      try
      {
        return dataSession
          .GetAll<HeartbeatConnection>(sessionToken, false)
          .ToList();
      }
      catch (Exception ex)
      {
        Logger.Error("There was an error getting heartbeat connections from the database.", ex);
        return null;
      }
    }
  }
}
