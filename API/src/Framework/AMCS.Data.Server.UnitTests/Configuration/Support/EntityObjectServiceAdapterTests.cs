namespace AMCS.Data.Server.UnitTests
{
  using AMCS.Data.Entity;
  using AMCS.Data.Server.SQL.Querying;
  using Moq;
  using NUnit.Framework;
  using System;
  using System.Runtime.Serialization;

  public class EntityAdapterTest : EntityObject
  {
    public EntityAdapterTest()
    {
    }
  }

  [TestFixture]
  class EntityObjectServiceAdapterTests
  {
    [Test]
    public void EntityObjectServiceAdapterEntityNull()
    {
      var dataAccess = new Mock<IEntityObjectAccess<EntityAdapterTest>>();

      var criteria = Criteria.For(typeof(BusinessTestEntity));

      IDataSession dataSession = new FakeDataSession();

      var mockUser = new Mock<ISessionToken>();

      IEntityObjectService<EntityAdapterTest> eos = new EntityObjectService<EntityAdapterTest>(dataAccess.Object);

      Assert.Throws<ArgumentNullException>(delegate { eos.GetExistsByCriteria(null, criteria, dataSession); });

    }
  }
}
