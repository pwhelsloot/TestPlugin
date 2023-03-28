using AMCS.CommsServer.PushClient;
using AMCS.Data.Configuration;

namespace AMCS.ApiService.CommsServer
{
  internal class CommsServerProtocolConfiguration
  {
    public Endpoint Endpoint { get; set; }

    public string Key { get; set; }

    public string Instance { get; set; }

    public string TenantId { get; set; }

    public string BaseUrl { get; set; }

    public bool EnableCompression { get; set; }

    public IConnectionString AzureServiceBusConnectionString { get; set; }
  }
}
