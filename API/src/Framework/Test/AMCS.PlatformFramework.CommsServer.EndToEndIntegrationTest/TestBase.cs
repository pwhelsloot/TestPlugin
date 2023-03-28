using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using AMCS.Data.Support.Security;
using AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Support;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest
{
  using Server.Configuration;

  public class TestBase : IDisposable
  {
    private readonly Random random = SharedRandom.GetRandom();

    private bool disposed;
    private Thread commsServerThread;
    private CancellationTokenSource cancellationTokenSource;
    private string connectionString;
    private string queuePrefix;

    [OneTimeSetUp]
    public void Setup()
    {
      try
      {
        File.Delete(@"..\..\..\configuration.json");
      }
      catch
      {
        // do nothing
      }

      XDocument commsServerConfiguration;

      string configurationFilePath = Path.Combine(Path.GetDirectoryName(typeof(TestCommsServerSetup).Assembly.Location)!, "commsServerConfiguration.xml");
      using (var stream = File.OpenRead(configurationFilePath))
      {
        commsServerConfiguration = XDocument.Load(stream);
      }

      var azureServiceBusStorage = commsServerConfiguration.Root?.Element("storage")?.Element("azureServiceBus");
      connectionString = GetString(azureServiceBusStorage, "connectionString");
      queuePrefix = GetString(azureServiceBusStorage, "queuePrefix");

      // clean up queues
      CommsServerQueueUtils.CleanUpAzureServiceBus(connectionString, queuePrefix);
      
      // run comms server
      cancellationTokenSource = new CancellationTokenSource();

      commsServerThread = new Thread(() => 
      {
        TestCommsServerSetup.Setup(cancellationTokenSource.Token);
      });

      commsServerThread.Start();

      // let the comms server warm up
      // call diagnostic page and make sure page loads
      DiagnosticsUtils.RunDiagnostics(new Uri(@"http://localhost:43603/diagnostics"));

      //run platform template
      TestServiceSetup.Setup();

      AdminUserId = GetAdminUserId();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
      cancellationTokenSource.Cancel();
      cancellationTokenSource.Dispose();
      commsServerThread.Join();

      // clean up queues
      CommsServerQueueUtils.CleanUpAzureServiceBus(connectionString, queuePrefix);
    }

    public ISessionToken AdminUserId { get; private set; }

    private static ISessionToken GetAdminUserId()
    {
      var userService = DataServices.Resolve<IUserService>();

      return userService.CreateSessionToken((AuthenticationUser)userService.Authenticate(
        new UserNameCredentials("admin", DataServices.Resolve<IPlatformFrameworkConfiguration>().TenantId)));
    }

    protected static void WithSession(Action<IDataSession> action)
    {
      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        action(session);

        transaction.Commit();
      }
    }

    protected static T WithSession<T>(Func<IDataSession, T> action)
    {
      T result;

      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        result = action(session);

        transaction.Commit();
      }

      return result;
    }

    protected static void AssertJsonEqual(string expected, string actual)
    {
      Assert.AreEqual(PrettifyJson(expected), PrettifyJson(actual));
    }

    protected static string PrettifyJson(string json)
    {
      using (var reader = new StringReader(json))
      using (var jsonReader = new JsonTextReader(reader))
      using (var writer = new StringWriter())
      {
        using (var jsonWriter = new JsonTextWriter(writer))
        {
          jsonWriter.Formatting = Formatting.Indented;
          jsonWriter.WriteToken(jsonReader);
        }

        return writer.ToString();
      }
    }

    protected string GetRandomLabel(string prefix)
    {
      return prefix + "_" + random.Next(100000, 999999);
    }

    private static string GetString(XElement element, string attribute)
    {
      return element.Attribute(attribute)?.Value;
    }

    public void Dispose()
    {
      if (!disposed)
      {
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;

        disposed = true;
      }
    }
  }
}
