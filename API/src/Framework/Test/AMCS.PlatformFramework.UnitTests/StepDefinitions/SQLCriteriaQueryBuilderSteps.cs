using AMCS.Data.Configuration;
using AMCS.Data.Mocking;
using AMCS.Data.Server.SQL.Querying;
using AMCS.Data.Server;
using Moq;
using System;
using TechTalk.SpecFlow;
using NUnit.Framework;
using AMCS.Data.Entity;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace AMCS.PlatformFramework.UnitTests
{
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

  [Binding]
  public class SQLCriteriaQueryBuilderSteps
  {
    private const string CRITERIA_SELECT = "SELECT";
    private const string CRITERIA_COUNT = "COUNT";
    private const string CRITERIA_EXISTS = "EXISTS";
    private const string CRITERIA_COUNT_HAS_ORDERBY = "COUNTHASORDERBY";
    private const string CRITERIA_EXISTS_HAS_ORDERBY = "EXISTSHASORDERBY";
    private const string CRITERIA_SELECT_HAS_ORDERBY = "SELECTHASORDERBY";
    private const string CRITERIA_EXISTS_WITH_FETCH = "EXISTSWITHFETCH";
    private const string CRITERIA_COUNT_WITH_FETCH = "COUNTWITHFETCH";
    private CriteriaQueryType CriteriaQueryType;
    private SQLCriteriaQueryBuilder SQLCriteriaQueryBuilder;
    private string Sql;
    private Exception exception;

    public class TestingDataServices
    {

    }
    private SQLCriteriaQueryBuilder Create(CriteriaQueryType queryType, bool fetchRequired = false)
    {
      var mock = new Mock<ITypeManager>();
      mock.Setup(ty => ty.GetTypes()).Returns(new Type[] { typeof(TestData) });
      var dataServices = new MockDataServices();
      dataServices.Add(typeof(EntityObjectManager), new EntityObjectManager(mock.Object));
      dataServices.Add(typeof(TestData), new TestingDataServices());
      dataServices.Activate();
      var criteria = Criteria.For(typeof(TestData));
      if (fetchRequired)
      {
        criteria.Fetch("FetchTest");
      }

      return new SQLCriteriaQueryBuilder(criteria, queryType);
    }

    private SQLCriteriaQueryBuilder CreateOld(CriteriaQueryType queryType)
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

    [Given(@"criteria query type (.*)")]
    public void GivenCriteriaQueryTypeSelect(string queryCriteria)
    {
      switch (queryCriteria.ToUpperInvariant())
      {
        case CRITERIA_SELECT:
        case CRITERIA_SELECT_HAS_ORDERBY:
          CriteriaQueryType = CriteriaQueryType.Select;
          break;
        case CRITERIA_COUNT:
        case CRITERIA_COUNT_HAS_ORDERBY:
        case CRITERIA_COUNT_WITH_FETCH:
          CriteriaQueryType = CriteriaQueryType.Count;
          break;
        case CRITERIA_EXISTS:
        case CRITERIA_EXISTS_HAS_ORDERBY:
        case CRITERIA_EXISTS_WITH_FETCH:
          CriteriaQueryType = CriteriaQueryType.Exists;
          break;
      }
    }

    [When(@"criteria (.*) is passed query builder")]
    public void WhenCriteriaIsPassedQueryBuilder(string queryCriteria)
    {
      switch (queryCriteria.ToUpperInvariant())
      {
        case CRITERIA_SELECT:
        case CRITERIA_COUNT:
        case CRITERIA_EXISTS:
          SQLCriteriaQueryBuilder = Create(CriteriaQueryType);
          Sql = SQLCriteriaQueryBuilder.GetSql();
          break;
        case CRITERIA_SELECT_HAS_ORDERBY:
        case CRITERIA_COUNT_HAS_ORDERBY:
        case CRITERIA_EXISTS_HAS_ORDERBY:
          SQLCriteriaQueryBuilder = Create(CriteriaQueryType);
          break;
        case CRITERIA_EXISTS_WITH_FETCH:
        case CRITERIA_COUNT_WITH_FETCH:
          try
          {
            SQLCriteriaQueryBuilder = Create(CriteriaQueryType, true);
          }
          catch (Exception ex)
          {
            exception = ex;
          }
          break;
      }
    }

    [Then(@"corresponding results are shown based on (.*)")]
    public void ThenCorrespondingResultsAreShown(string queryCriteria)
    {
      switch (queryCriteria.ToUpperInvariant())
      {
        case CRITERIA_SELECT:
          StringAssert.StartsWith("SELECT *", Sql);
          break;
        case CRITERIA_COUNT:
          StringAssert.StartsWith("SELECT COUNT(*)", Sql);
          break;
        case CRITERIA_EXISTS:
          StringAssert.StartsWith("SELECT TOP 1 1", Sql);
          break;
        case CRITERIA_SELECT_HAS_ORDERBY:
        case CRITERIA_COUNT_HAS_ORDERBY:
        case CRITERIA_EXISTS_HAS_ORDERBY:
          Assert.IsFalse(SQLCriteriaQueryBuilder.HasOrderBy());
          break;
        case CRITERIA_EXISTS_WITH_FETCH:
        case CRITERIA_COUNT_WITH_FETCH:
          Assert.AreEqual(typeof(ArgumentException), exception.GetType());
          break;
      }
    }
  }
}
