using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using AMCS.CommsServer.Server;
using AMCS.CommsServer.Server.AppApi;
using AMCS.CommsServer.Server.Configuration;
using AMCS.CommsServer.Server.Support;
using AMCS.PlatformFramework.CommsServer.Protocols;
using AMCS.PlatformFramework.CommsServer.Protocols.Client;
using AMCS.PlatformFramework.CommsServer.Protocols.Server;

namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest
{
  public static class TestCommsServerSetup
  {
    public static void Setup(CancellationToken cancellationToken)
    {
      CommsServerConfiguration configuration;

      string configurationFilePath = Path.Combine(Path.GetDirectoryName(typeof(TestCommsServerSetup).Assembly.Location), "commsServerConfiguration.xml");
      using (var stream = File.OpenRead(configurationFilePath))
      {
        configuration = CommsServerXmlConfigurationParser.Parse(stream);
      }

      var properties = new Properties
      {
        [Constants.AuthenticationKeyPropertyName] = "let-me-in"
      };

      configuration.Protocols.Add(new ServerProtocol(), properties);
      configuration.Protocols.Add(new ClientProtocol(), properties);

      using (var app = CommsServerApp.Start("http://localhost:43603", configuration))
      {
        app.WaitForShutdown(cancellationToken);
      }
    }
  }
}