namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading.Tasks;
  using AzureServiceBus;
  using AzureServiceBusSupport;
  using Channels;
  using global::Azure.Messaging.EventGrid;
  using global::Azure.Messaging.ServiceBus;

  public class AzureAppConfigurationReceiverRemoteTransport : IAzureAppConfigurationReceiverTransport
  {
    private readonly MessageQueueManager manager;
    private readonly AzureAppConfigurationQueueNameBuilder nameBuilder;
    private ReceiveLoop azureAppConfigurationSubscription;
    private bool disposed;

    public event MessageReceivedEventHandler MessageReceived;
    public event EventHandler Opened;
    public event ExceptionEventHandler Closed;

    public AzureAppConfigurationReceiverRemoteTransport(MessageQueueManager manager, AzureAppConfigurationQueueNameBuilder nameBuilder)
    {
      this.manager = manager;
      this.nameBuilder = nameBuilder;
    }

    public void Open()
    {
      azureAppConfigurationSubscription = new ReceiveLoop(
        ServiceBusConstants.MaxOutstandingAcks,
        ProcessAzureAppConfigurationMessages,
        CreateMessageReceiver
      );

      azureAppConfigurationSubscription.Start();

      OnOpened();
    }

    private ServiceBusReceiver CreateMessageReceiver()
    {
      return manager.OpenSubscription(
        nameBuilder.AzureAppConfigurationTopicName, 
        nameBuilder.AzureAppConfigurationSubscriptionName,
        ServiceBusReceiveMode.ReceiveAndDelete
      );
    }

    private Task ProcessAzureAppConfigurationMessages(IReadOnlyList<ServiceBusReceivedMessage> messages)
    {
      foreach (var message in messages)
      {
        var eventGridEvent = EventGridEvent.Parse(BinaryData.FromBytes(message.Body));

        MessageReceived?.Invoke(new MessageReceivedEventArgs(eventGridEvent));
      }

      return Task.CompletedTask;
    }
    
    public void Close()
    {
      if (azureAppConfigurationSubscription != null)
      {
        azureAppConfigurationSubscription.Dispose();
        azureAppConfigurationSubscription = null;
      }
    }

    protected virtual void OnOpened() => Opened?.Invoke(this, EventArgs.Empty);
    protected virtual void OnClosed(ExceptionEventArgs e) => Closed?.Invoke(this, e);

    public void Dispose()
    {
      if (!disposed)
      {
        Close();

        OnClosed(new ExceptionEventArgs(null));

        disposed = true;
      }
    }
  }
}