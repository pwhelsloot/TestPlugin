namespace AMCS.Data.Server.AzureServiceBus
{
  using BslTriggers;
  using global::Azure.Messaging.ServiceBus;

  public interface IAzureServiceBusConnectionManager
  {
    void SendMessage(AzureServiceBusBslActionConfiguration configuration, ServiceBusMessage message);
  }
}
