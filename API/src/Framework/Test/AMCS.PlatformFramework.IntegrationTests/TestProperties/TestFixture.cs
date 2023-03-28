using AMCS.Data;
using AMCS.PlatformFramework.Server.User;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests
{
  [TestFixture]
  public class TestFixture : TestBase
  {
    [Test]
    public void EnsureAdminUser()
    {
      // There's nothing to do here, because the test service setup does this.

      var userEntity = WithSession(session =>
      {
        return DataServices.Resolve<IUserService>().GetByName(AdminUserId, "admin", session);
      });

      Assert.IsNotNull(userEntity);
    }
  }
}
