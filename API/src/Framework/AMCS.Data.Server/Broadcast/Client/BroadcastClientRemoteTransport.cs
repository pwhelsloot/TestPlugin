namespace AMCS.Data.Server.Broadcast.Client
{
  using System;
  using System.Text;
  using AMCS.AzureServiceBusSupport;
  using AMCS.Channels;
  using AMCS.Data.Server.AzureServiceBus;
  using Newtonsoft.Json;
  using global::Azure.Messaging.ServiceBus;

  internal class BroadcastClientRemoteTransport : IBroadcastClientTransport
  {
    private readonly MessageQueueManager manager;
    private readonly MessageQueueNameBuilder nameBuilder;
    private bool disposed;
    private MessageSenderManager broadcastTopic;

    public event EventHandler Opened;
    public event ExceptionEventHandler Closed;

    public BroadcastClientRemoteTransport(MessageQueueManager manager, MessageQueueNameBuilder nameBuilder)
    {
      this.manager = manager;
      this.nameBuilder = nameBuilder;
    }

    public void Open()
    {
      broadcastTopic = new MessageSenderManager(manager.OpenTopic(
        nameBuilder.GetBroadcastTopicName()
      ));

      OnOpened();
    }

    public void SendMessage<T>(BroadcastMessage<T> broadcastMessage)
    {
      broadcastTopic.QueueSend(CreateMessage(JsonConvert.SerializeObject(broadcastMessage)));
    }

    protected virtual void OnOpened()
    {
      Opened?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClosed(ExceptionEventArgs e)
    {
      Closed?.Invoke(this, e);
    }

    private ServiceBusMessage CreateMessage(string json)
    {
      return new ServiceBusMessage(Encoding.UTF8.GetBytes(json));
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (broadcastTopic != null)
        {
          broadcastTopic.Dispose();
          broadcastTopic = null;
        }

        OnClosed(new ExceptionEventArgs(null));

        disposed = true;
      }
    }
  }
}
