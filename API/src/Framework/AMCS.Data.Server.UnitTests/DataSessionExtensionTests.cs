namespace AMCS.Data.Server.UnitTests
{
  using AMCS.Data.Entity;
  using AMCS.Data.Mocking;
  using AMCS.Data.Server.SQL.Querying;
  using Moq;
  using NUnit.Framework;
  using System;
  using System.Runtime.Serialization;

  public class TestEntity : EntityObject
  {
    public TestEntity()
    {
    }
  }

  [TestFixture]
  class DataSessionExtensionTests
  {
    [Test]
    public void DataSessionCallingQueryExists()
    {

      var entityService = new Mock<IEntityObjectService<TestEntity>>();

      var mockUser = new Mock<ISessionToken>();

      var criteria = Criteria.For(typeof(TestEntity));

      var dataServices = new MockDataServices();

      IDataSession dataSession = new FakeDataSession();

      dataServices.Add(typeof(IEntityObjectService<TestEntity>), entityService.Object);   

      dataServices.Activate();

      Assert.IsFalse(dataSession.GetExistsByCriteria<TestEntity>(mockUser.Object, criteria));
    }

    [Test]
    public void DataSessionUserIdNullPassed()
    {
      var criteria = Criteria.For(typeof(TestEntity));

      IDataSession dataSession = new FakeDataSession();

      Assert.Throws<ArgumentNullException>(delegate { dataSession.GetExistsByCriteria<TestEntity>(null, criteria); });
    }

    [Test]
    public void DataSessionCriteriaNullPassed()
    {
      IDataSession dataSession = new FakeDataSession();

      var mockUser = new Mock<ISessionToken>();

      Assert.Throws<ArgumentNullException>(delegate { dataSession.GetExistsByCriteria<TestEntity>(mockUser.Object, null); });
    }

    [Test]
    public void DataSessionIDataSessionNullPassed()
    {
      var criteria = Criteria.For(typeof(TestEntity));

      var mockUser = new Mock<ISessionToken>();

      IDataSession dataSession = null;

      Assert.Throws<ArgumentNullException>(delegate { dataSession.GetExistsByCriteria<TestEntity>(mockUser.Object, criteria); });
    }
  }
}
