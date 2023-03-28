namespace AMCS.Data.Server.Broadcast.Receiver
{
  using AMCS.Channels;
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.AzureServiceBusSupport;
  using AzureServiceBus;
  using AMCS.WebDiagnostics;
  using log4net;
  using Newtonsoft.Json;
  using global::Azure.Messaging.ServiceBus;

  internal class BroadcastReceiverRemoteTransport : IBroadcastReceiverTransport
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(BroadcastReceiverRemoteTransport));

    private readonly MessageQueueManager manager;
    private readonly MessageQueueNameBuilder nameBuilder;
    private ReceiveLoop broadcastSubscription;
    private bool disposed;

    public event MessageReceivedEventHandler MessageReceived;
    public event EventHandler Opened;
    public event ExceptionEventHandler Closed;

    public BroadcastReceiverRemoteTransport(MessageQueueManager manager, MessageQueueNameBuilder nameBuilder)
    {
      this.manager = manager;
      this.nameBuilder = nameBuilder;

      DataServices.Resolve<IDiagnosticsManager>().Register(GetDiagnostics);
    }

    public void Open()
    {
      broadcastSubscription = new ReceiveLoop(
        ServiceBusConstants.MaxOutstandingAcks,
        ProcessBroadcastMessages,
        CreateMessageReceiver
      );
      broadcastSubscription.Start();

      OnOpened();
    }

    private ServiceBusReceiver CreateMessageReceiver()
    {
      return manager.OpenSubscription(
        nameBuilder.GetBroadcastTopicName(),
        nameBuilder.GetBroadcastSubscriptionName(),
        ServiceBusReceiveMode.ReceiveAndDelete
      );
    }

    private Task ProcessBroadcastMessages(IReadOnlyList<ServiceBusReceivedMessage> messages)
    {
      foreach (var message in messages)
      {
        var dto = JsonConvert.DeserializeObject<BroadcastReceiveMessage>(Encoding.UTF8.GetString(message.Body.ToArray()));

        MessageReceived?.Invoke(new MessageReceivedEventArgs(dto));
      }

      return Task.CompletedTask;
    }

    private IEnumerable<DiagnosticResult> GetDiagnostics()
    {
      const string diagnosticName = "Broadcast Service Transport";
      if (broadcastSubscription?.UnhandledException != null)
        yield return new DiagnosticResult.Failure(diagnosticName, "Exception in broadcast service transport found",
          broadcastSubscription.UnhandledException);
      else
        yield return new DiagnosticResult.Success(diagnosticName);
    }

    public void Close(CloseMode mode)
    {
      if (mode.HasFlag(CloseMode.Receive) && broadcastSubscription != null)
      {
        broadcastSubscription.Dispose();
        broadcastSubscription = null;
      }
    }

    protected virtual void OnOpened() => Opened?.Invoke(this, EventArgs.Empty);
    protected virtual void OnClosed(ExceptionEventArgs e) => Closed?.Invoke(this, e);

    public void Dispose()
    {
      if (!disposed)
      {
        Close(CloseMode.Both);

        OnClosed(new ExceptionEventArgs(null));

        disposed = true;
      }
    }
  }
}
