namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using AzureServiceBus;
  using AzureServiceBusSupport;

  public class AzureAppConfigurationRemoteTransportFactory : IAzureAppConfigurationTransportFactory
  {
    private MessageQueueManager manager;
    private bool disposed;
    private readonly AzureAppConfigurationQueueNameBuilder nameBuilder;

    public AzureAppConfigurationRemoteTransportFactory(AzureAppConfigurationRemoteTransportFactoryConfiguration configuration)
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

      nameBuilder = new AzureAppConfigurationQueueNameBuilder(
        configuration.InstanceName,
        configuration.TopicName);
    }

    public IAzureAppConfigurationReceiverTransport CreateReceiverTransport()
    {
      return new AzureAppConfigurationReceiverRemoteTransport(manager, nameBuilder);
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