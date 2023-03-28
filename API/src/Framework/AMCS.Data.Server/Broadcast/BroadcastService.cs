using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Broadcast.Client;
using AMCS.Data.Server.Broadcast.Receiver;
using log4net;
using Newtonsoft.Json;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastService : IBroadcastService
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(BroadcastService));

    private IBroadcastTransportFactory broadcastTransportFactory;
    private IBroadcastClientTransport broadcastClient;
    private IBroadcastReceiverTransport broadcastReceiver;
    private readonly Dictionary<string, Registration> registrations = new Dictionary<string, Registration>();
    private volatile ServiceState serviceState;
    private bool disposed;

    public BroadcastService(IBroadcastTransportFactory broadcastTransportFactory, ISetupService setupService)
    {
      this.broadcastTransportFactory = broadcastTransportFactory;
      broadcastClient = broadcastTransportFactory.CreateClientTransport();
      broadcastClient.Open();

      setupService.RegisterSetup(Open, 1000);
    }

    private void Open()
    {
      broadcastReceiver = broadcastTransportFactory.CreateReceiverTransport();
      broadcastReceiver.MessageReceived += ProcessMessagesReceived;
      broadcastReceiver.Open();

      serviceState = ServiceState.Opened;
    }
    
    public void Broadcast(object obj)
    {
      var broadcastMessage = new BroadcastMessage<object>
      {
        Type = obj.GetType().FullName,
        Data = obj
      };

      broadcastClient.SendMessage(broadcastMessage);
    }

    public void On<T>(Action<T> action)
    {
      if (serviceState != ServiceState.Opening)
        throw new InvalidOperationException();

      var typeName = typeof(T).FullName;
      if (typeName != null)
      {
        registrations[typeName] = new Registration(typeof(T), obj => action((T)obj));
      }
      else
      {
        Log.Error($"Could not add registration for { typeof(T) }");
      }
    }

    private void ProcessMessagesReceived(MessageReceivedEventArgs e)
    {
      if (registrations.TryGetValue(e.BroadcastReceiveMessage.Type, out var registration))
      {
        try
        {
          ThreadPool.QueueUserWorkItem(wcb => 
          {
            try
            {
              registration.Action?.Invoke(JsonConvert.DeserializeObject((string)e.BroadcastReceiveMessage.Data, registration.Type));
            }
            catch (Exception ex)
            {
              Log.Error($"Error invoking registered callback for {e.BroadcastReceiveMessage.Type}", ex);
            }
          });
        }
        catch (Exception ex)
        {
          Log.Error($"Error invoking registered callback for {e.BroadcastReceiveMessage.Type}", ex);
        }
      }
      else
      {
        Log.Warn($"No registration found for {e.BroadcastReceiveMessage.Type}");
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        serviceState = ServiceState.Closed;

        if (broadcastClient != null)
        {
          broadcastClient.Dispose();
          broadcastClient = null;
        }

        if (broadcastReceiver != null)
        {
          broadcastReceiver.Dispose();
          broadcastReceiver = null;
        }

        if (broadcastTransportFactory != null)
        {
          broadcastTransportFactory.Dispose();
          broadcastTransportFactory = null;
        }

        disposed = true;
      }
    }

    private class Registration
    {
      public Type Type { get; }
      public Action<object> Action { get; }

      public Registration(Type type, Action<object> action)
      {
        Type = type;
        Action = action;
      }
    }

    private enum ServiceState
    {
      Opening,
      Opened,
      Closed
    }
  }
}
