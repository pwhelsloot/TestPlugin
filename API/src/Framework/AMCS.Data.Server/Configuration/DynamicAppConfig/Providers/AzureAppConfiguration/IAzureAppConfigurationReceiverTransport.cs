namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using Channels;

  public interface IAzureAppConfigurationReceiverTransport : IDisposable
  {
    event EventHandler Opened;

    event ExceptionEventHandler Closed;

    event MessageReceivedEventHandler MessageReceived;

    void Open();

    void Close();
  }
}