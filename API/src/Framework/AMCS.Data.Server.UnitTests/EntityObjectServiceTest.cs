namespace AMCS.Data.Server.UnitTests
{
  using AMCS.Data.Entity;
  using AMCS.Data.Server.SQL;
  using AMCS.Data.Server.SQL.Querying;
  using Moq;
  using NUnit.Framework;
  using System;
  using System.Runtime.Serialization;

  public class TestEntityService : EntityObject
  {
    public TestEntityService()
    {
    }
  }

  [TestFixture]
  class EntityObjectServiceTest
  {
    [Test]
    public void EntityObjectServiceQueryExists()
    {

      var dataAccess = new Mock<IEntityObjectAccess<TestEntityService>>();

      var mockUser = new Mock<ISessionToken>();

      var isqRead = new Mock<ISQLReadable>();

      isqRead.Setup(ir => ir.SingleOrDefaultScalar<object>()).Returns(1);

      IDataSession dataSession = new FakeDataSession();

      Assert.IsNotNull(isqRead.Object);

      var criteria = Criteria.For(typeof(TestEntityService));

      dataAccess.Setup(da => da.GetByCriteria(dataSession, mockUser.Object, It.IsAny<ICriteria>(), CriteriaQueryType.Exists))
      .Returns(isqRead.Object);

      var service = new EntityObjectService<TestEntityService>(dataAccess.Object);

      Assert.IsTrue(service.GetExistsByCriteria(mockUser.Object, criteria, dataSession));

    }

    [Test]
    public void EntityObjectServiceQueryExistsReturnException()
    {

      var dataAccess = new Mock<IEntityObjectAccess<TestEntityService>>();

      var mockUser = new Mock<ISessionToken>();

      var isqRead = new Mock<ISQLReadable>();

      isqRead.Setup(ir => ir.SingleOrDefaultScalar<object>()).Returns(1);

      IDataSession dataSession = null;

      Assert.IsNotNull(isqRead.Object);

      var criteria = Criteria.For(typeof(TestEntityService));

      dataAccess.Setup(da => da.GetByCriteria(dataSession, mockUser.Object, It.IsAny<ICriteria>(), CriteriaQueryType.Exists))
      .Returns(isqRead.Object);

      var service = new EntityObjectService<TestEntityService>(dataAccess.Object);

      Assert.Throws<ArgumentNullException>(delegate { service.GetExistsByCriteria(mockUser.Object, criteria, dataSession); });

    }

    [Test]
    public void EntityObjectServiceQueryExistsReturnsFalse()
    {

      var dataAccess = new Mock<IEntityObjectAccess<TestEntityService>>();

      var mockUser = new Mock<ISessionToken>();

      var isqRead = new Mock<ISQLReadable>();

      isqRead.Setup(ir => ir.SingleOrDefaultScalar<object>()).Returns(null);

      IDataSession dataSession = new FakeDataSession();

      Assert.IsNotNull(isqRead.Object);

      var criteria = Criteria.For(typeof(TestEntityService));

      dataAccess.Setup(da => da.GetByCriteria(dataSession, mockUser.Object, It.IsAny<ICriteria>(), CriteriaQueryType.Exists))
      .Returns(isqRead.Object);

      var service = new EntityObjectService<TestEntityService>(dataAccess.Object);

      Assert.IsFalse(service.GetExistsByCriteria(mockUser.Object, criteria, dataSession));

    }

    [Test]
    public void EntityObjectServiceUserIdNullPassed()
    {
      var criteria = Criteria.For(typeof(TestEntity));

      IDataSession dataSession = new FakeDataSession();

      var dataAccess = new Mock<IEntityObjectAccess<TestEntityService>>();

      var service = new EntityObjectService<TestEntityService>(dataAccess.Object);

      Assert.Throws<ArgumentNullException>(delegate { service.GetExistsByCriteria(null, criteria, dataSession); });
    }

    [Test]
    public void EntityObjectServiceCriteriaNullPassed()
    {
      IDataSession dataSession = new FakeDataSession();

      var mockUser = new Mock<ISessionToken>();

      var dataAccess = new Mock<IEntityObjectAccess<TestEntityService>>();

      var service = new EntityObjectService<TestEntityService>(dataAccess.Object);

      Assert.Throws<ArgumentNullException>(delegate { service.GetExistsByCriteria(mockUser.Object, null, dataSession); });
    }
  }
}

