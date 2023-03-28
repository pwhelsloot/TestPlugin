namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using global::Azure.Messaging.EventGrid;

  public class MessageReceivedEventArgs : EventArgs
  {
    public EventGridEvent EventGridEvent { get; }

    public MessageReceivedEventArgs(EventGridEvent eventGridEvent)
    {
      EventGridEvent = eventGridEvent;
    }
  }

  public delegate void MessageReceivedEventHandler(MessageReceivedEventArgs e);
}