namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;

  public interface IAzureAppConfigurationTransportFactory : IDisposable
  {
    /// <summary>
    /// Create a new receiver transport.
    /// </summary>
    /// <returns>A new receiver transport.</returns>
    IAzureAppConfigurationReceiverTransport CreateReceiverTransport();
  }
}