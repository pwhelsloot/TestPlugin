namespace AMCS.Data.Server.AzureServiceBus
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using AMCS.AzureServiceBusSupport;
  using AMCS.Data.Server.BslTriggers;
  using log4net;
  using global::Azure.Messaging.ServiceBus;

  public class AzureServiceBusConnectionManager : IAzureServiceBusConnectionManager, IDisposable
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(AzureServiceBusConnectionManager));

    private readonly object syncRoot = new object();

    private Timer timer;
    private Stopwatch stopwatch = Stopwatch.StartNew();
    private bool disposed;

    private readonly Dictionary<(string, string), Connection> connections = new Dictionary<(string, string), Connection>();

    public AzureServiceBusConnectionManager()
    {
      timer = new Timer(RemoveIdleConnections, null, new TimeSpan(), TimeSpan.FromMinutes(30));
    }

    public void SendMessage(AzureServiceBusBslActionConfiguration configuration, ServiceBusMessage message)
    {
      lock (syncRoot)
      {
        if (!connections.TryGetValue((configuration.ConnectionString, configuration.Name), out var connection))
        {
          connection = new Connection
          {
            MessageSenderManager = new MessageSenderManager(
              new ServiceBusClient(
                  configuration.ConnectionString, 
                  new ServiceBusClientOptions { RetryOptions = ServiceBusConstants.RetryOptions })
                .CreateSender(configuration.Name))
          };

          connections.Add((configuration.ConnectionString, configuration.Name), connection);
        }

        connection.MessageSenderManager.QueueSend(message);
        connection.LastActive = stopwatch.Elapsed;
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (timer != null)
        {
          timer.Dispose();
          timer = null;
        }

        disposed = true;
      }
    }

    private void RemoveIdleConnections(object state)
    {
      try
      {
        var expiredConnections = new List<(string, string)>();

        lock (syncRoot)
        {
          foreach (var connection in connections)
          {
            if (stopwatch.Elapsed - connection.Value.LastActive > TimeSpan.FromHours(1))
            {
              connection.Value.MessageSenderManager.Dispose();
              expiredConnections.Add(connection.Key);
            }
          }

          foreach (var expiredConnection in expiredConnections)
          {
            connections.Remove(expiredConnection);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Info("Could not remove expired message sender from connections", ex);
      }
    }

    private class Connection
    {
      public TimeSpan LastActive { get; set; }

      public MessageSenderManager MessageSenderManager { get; set; }
    }
  }
}
