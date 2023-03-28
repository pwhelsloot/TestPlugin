using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Abstractions.CommsServer
{
  public class CommsServerEndpoint
  {
    public string Url { get; }

    public string AuthenticationPayload { get; }

    public CommsServerProtocol Protocol { get; }

    public string AzureServiceBusConnectionStringName { get; }

    public CommsServerEndpoint(string url, string authenticationPayload, CommsServerProtocol protocol, string azureServiceBusConnectionStringName)
    {
      Url = url;
      AuthenticationPayload = authenticationPayload;
      Protocol = protocol;
      AzureServiceBusConnectionStringName = azureServiceBusConnectionStringName;
    }
  }
}
