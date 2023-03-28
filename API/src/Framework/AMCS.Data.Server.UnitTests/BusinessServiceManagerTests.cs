namespace AMCS.Data.Server.UnitTests
{
  using AMCS.Data.Entity;
  using AMCS.Data.Mocking;
  using AMCS.Data.Server.SQL.Querying;
  using Moq;
  using NUnit.Framework;
  using System;
  using System.Runtime.Serialization;

  public class BusinessTestEntity : EntityObject
  {
    public BusinessTestEntity()
    {
    }
  }

  [TestFixture]
  class BusinessServiceManagerTests
  {
    [Test]
    public void BusinessServiceManagerUserIdNullPassed()
    {
      var criteria = Criteria.For(typeof(BusinessTestEntity));

      IDataSession dataSession = new FakeDataSession();

      Assert.Throws<ArgumentNullException>(delegate { BusinessServiceManager.GetExistsByCriteria<BusinessTestEntity>(null, criteria, dataSession); });
    }

    [Test]
    public void BusinessServiceManagerCriteriaNullPassed()
    {
      IDataSession dataSession = new FakeDataSession();

      var mockUser = new Mock<ISessionToken>();

      Assert.Throws<ArgumentNullException>(delegate { BusinessServiceManager.GetExistsByCriteria<BusinessTestEntity>(mockUser.Object, null, dataSession); });
    }

    [Test]
    public void BusinessServiceManagerIDataSessionNullPassed()
    {
      var criteria = Criteria.For(typeof(BusinessTestEntity));

      var mockUser = new Mock<ISessionToken>();

      Assert.Throws<ArgumentNullException>(delegate { BusinessServiceManager.GetExistsByCriteria<BusinessTestEntity>(mockUser.Object, criteria, null); });
    }
  }
}
