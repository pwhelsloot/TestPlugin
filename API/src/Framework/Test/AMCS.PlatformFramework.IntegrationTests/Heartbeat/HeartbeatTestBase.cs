using System.Linq;
using AMCS.Data;
using AMCS.Data.Entity.Heartbeat;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  public abstract class HeartbeatTestBase : TestBase
  {
    [TearDown]
    public void Cleanup()
    {
      var connectionRegistry = DataServices.Resolve<FakeConnectionRegistry>();
      connectionRegistry.ClearCommServerConnections();
      
      var systemToken = DataServices.Resolve<IUserService>().CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      {
        dataSession.StartTransaction();
        var heartbeatConnections = dataSession
          .GetAll<HeartbeatConnection>(systemToken, false)
          .ToList();

        heartbeatConnections.ForEach(heartbeatConnection =>
          dataSession.Delete<HeartbeatConnection>(systemToken, heartbeatConnection.Id32, false));
        
        dataSession.CommitTransaction();
      }
    }
  }
}