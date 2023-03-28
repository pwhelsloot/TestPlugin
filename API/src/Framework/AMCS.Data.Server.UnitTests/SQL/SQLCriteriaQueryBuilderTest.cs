namespace AMCS.Data.Server.UnitTests.SQL
{
  using System;
  using System.Runtime.Serialization;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity;
  using AMCS.Data.Mocking;
  using AMCS.Data.Server;
  using AMCS.Data.Server.SQL.Querying;
  using Moq;
  using NUnit.Framework;

  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class TestData : EntityObject
  {
    [DataMember(Name = "CustomerId")]
    public int CustomerId
    {
      get;
      set;
    }
    public override string GetTableName()
    {
      return "CreditNote";
    }


  }
  [TestFixture]
  public class SQLCriteriaQueryBuilderTest
  {
    public class TestingDataServices
    {

    }

    [Test]
    public void QueryTypeSelect()
    {
      var query = Create(CriteriaQueryType.Select);

      var sql = query.GetSql();

      StringAssert.StartsWith("SELECT *", sql);
    }

    [Test]
    public void QueryTypeCount()
    {
      var query = Create(CriteriaQueryType.Count);

      var sql = query.GetSql();

      StringAssert.StartsWith("SELECT COUNT(*)", sql);
    }

    [Test]
    public void QueryTypeExists()
    {
      var query = Create(CriteriaQueryType.Exists);

      var sql = query.GetSql();

      StringAssert.StartsWith("SELECT TOP 1 1", sql);
    }

    [Test]
    public void QueryTypeExistsHasOrderBy()
    {
      var query = Create(CriteriaQueryType.Exists);

      Assert.IsFalse(query.HasOrderBy());
    }

    [Test]
    public void QueryTypeCountHasOrderBy()
    {
      var query = Create(CriteriaQueryType.Count);

      Assert.IsFalse(query.HasOrderBy());
    }

    [Test]
    public void QueryTypeSelectHasOrderBy()
    {
      var query = Create(CriteriaQueryType.Select);

      Assert.IsFalse(query.HasOrderBy());
    }

    private SQLCriteriaQueryBuilder Create(CriteriaQueryType queryType)
    {

      var mock = new Mock<ITypeManager>();

      mock.Setup(ty => ty.GetTypes()).Returns(new Type[] { typeof(TestData) });

      var dataServices = new MockDataServices();

      dataServices.Add(typeof(EntityObjectManager), new EntityObjectManager(mock.Object));

      dataServices.Add(typeof(TestData), new TestingDataServices());

      dataServices.Activate();

      var queryBuilder = new SQLCriteriaQueryBuilder(Criteria.For(typeof(TestData)), queryType);

      return queryBuilder;
    }


    [Test]
    public void QueryTypeExistWithFetch()
    {
      var mock = new Mock<ITypeManager>();

      mock.Setup(ty => ty.GetTypes()).Returns(new Type[] { typeof(TestData) });

      var dataServices = new MockDataServices();

      dataServices.Add(typeof(EntityObjectManager), new EntityObjectManager(mock.Object));

      dataServices.Add(typeof(TestData), new TestingDataServices());

      dataServices.Activate();

      var criteria = Criteria.For(typeof(TestData));

      criteria.Fetch("FetchTest");

      Assert.Throws<ArgumentException>(delegate { var queryBuilder = new SQLCriteriaQueryBuilder(criteria, CriteriaQueryType.Exists); });

    }

    [Test]
    public void QueryTypeCountWithFetch()
    {
      var mock = new Mock<ITypeManager>();

      mock.Setup(ty => ty.GetTypes()).Returns(new Type[] { typeof(TestData) });

      var dataServices = new MockDataServices();

      dataServices.Add(typeof(EntityObjectManager), new EntityObjectManager(mock.Object));

      dataServices.Add(typeof(TestData), new TestingDataServices());

      dataServices.Activate();

      var criteria = Criteria.For(typeof(TestData));

      criteria.Fetch("FetchTest");

      Assert.Throws<ArgumentException>(delegate { var queryBuilder = new SQLCriteriaQueryBuilder(criteria, CriteriaQueryType.Count); });

    }


  }
}
