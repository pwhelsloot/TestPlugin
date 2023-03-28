using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.AzureServiceBusSupport;
using AMCS.Data.Server.AzureServiceBus;
using AMCS.Data.Server.Broadcast.Client;
using AMCS.Data.Server.Broadcast.Receiver;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastRemoteTransportFactory : IBroadcastTransportFactory
  {
    private MessageQueueManager manager;
    private MessageQueueNameBuilder nameBuilder;
    private bool disposed;

    public BroadcastRemoteTransportFactory(BroadcastRemoteTransportFactoryConfiguration configuration)
    {
      manager = new MessageQueueManager(
        configuration.ConnectionString,
        new MessageQueueConfiguration(
          configuration.AutoDeleteOnIdle,
          configuration.MaxDeliveryCount ?? ServiceBusConstants.MaxDeliveryCount,
          ServiceBusConstants.EnablePartitioning,
          ServiceBusConstants.QueueSizeMb,
          ServiceBusConstants.LockDuration,
          ServiceBusConstants.DefaultMessageTimeToLive,
          ServiceBusConstants.RetryOptions
        )
      );

      nameBuilder = new MessageQueueNameBuilder(
        configuration.InstanceName,
        configuration.QueuePrefix
      );
    }

    public IBroadcastClientTransport CreateClientTransport()
    {
      return new BroadcastClientRemoteTransport(manager, nameBuilder);
    }

    public IBroadcastReceiverTransport CreateReceiverTransport()
    {
      return new BroadcastReceiverRemoteTransport(manager, nameBuilder);
    }
    
    public void Dispose()
    {
      if (!disposed)
      {
        if (manager != null)
        {
          manager.Dispose();
          manager = null;
        }

        disposed = true;
      }
    }
  }
}
