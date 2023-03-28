using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Mocking;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.UnitTests.TestProperties;
using Moq;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices.ComTypes;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests
{
  [Binding]
  public class DataServerServiceManagerSteps
  {
    private const string PARAMETER_CRITERIA = "CRITERIA";
    private const string PARAMETER_USERID = "USERID";
    private const string PARAMETER_DATASESSION = "DATASESSION";
    private const string SERVICE_DATASESSIONEXTENSION = "DATASESSIONEXTENSION";
    private const string SERVICE_BUSINESSOBJECTSMANAGER = "BUSINESSSERVICEMANAGER";
    private const string SERVICE_DATASERVICES = "DATASERVICES";
    private const string SERVICE_ENTITYOBJECT = "ENTITYOBJECTSERVICE";
    private const string SERVICE_ENTITYOBJECT_ADAPTER = "ENTITYOBJECTSERVICEADAPTER";
    private ICriteria criteria = Criteria.For(typeof(DataServerTestEntity));
    private IDataSession dataSession = new FakeDataSession();
    private Mock<ISessionToken> mockUser = new Mock<ISessionToken>();
    private Mock<IEntityObjectAccess<DataServerTestEntity>> DataAccess = new Mock<IEntityObjectAccess<DataServerTestEntity>>();
    private EntityObjectService<DataServerTestEntity> Service;
    private Exception exception;
    private Mock<IEntityObjectService<DataServerTestEntity>> EntityService;
    private MockDataServices mockDataServices;
    private Mock<ISQLReadable> IsqRead;
    private IEntityObjectService<DataServerTestEntity> EntityObjectService;

    [Given(@"parameter (.*) is Null")]
    public void GivenParameterIsNull(string paramter)
    {
      switch (paramter.ToUpperInvariant())
      {
        case PARAMETER_CRITERIA:
          criteria = null;
          break;
        case PARAMETER_USERID:
          mockUser = null;
          break;
        case PARAMETER_DATASESSION:
          dataSession = null;
          break;
      }
    }

    [When(@"(.*) is called")]
    public void WhenCorrespondingDataServiceIsCalled(string service)
    {
      switch (service.ToUpperInvariant())
      {
        case SERVICE_BUSINESSOBJECTSMANAGER:
          try
          {
            if (mockUser != null)
            {
              BusinessServiceManager.GetExistsByCriteria<DataServerTestEntity>(mockUser.Object, criteria, dataSession);
            }
            else
            {
              BusinessServiceManager.GetExistsByCriteria<DataServerTestEntity>(null, criteria, dataSession);
            }
          }
          catch (Exception ex)
          {
            exception = ex;
          }
          break;
        case SERVICE_DATASESSIONEXTENSION:
          try
          {
            if (mockUser != null)
            {
              dataSession.GetExistsByCriteria<DataServerTestEntity>(mockUser.Object, criteria);
            }
            else
            {
              dataSession.GetExistsByCriteria<DataServerTestEntity>(null, criteria);
            }
          }
          catch (Exception ex)
          {
            exception = ex;
          }
          break;
        case SERVICE_DATASERVICES:
          mockDataServices.Add(typeof(IEntityObjectService<DataServerTestEntity>), EntityService.Object);
          mockDataServices.Activate();
          break;
        case SERVICE_ENTITYOBJECT:
          try
          {
            Service = new EntityObjectService<DataServerTestEntity>(DataAccess.Object);
            if (mockUser != null)
            {
              Service.GetExistsByCriteria(mockUser.Object, criteria, dataSession);
            }
            else
            {
              Service.GetExistsByCriteria(null, criteria, dataSession);
            }
          }
          catch (Exception ex)
          {
            exception = ex;
          }
          break;
        case SERVICE_ENTITYOBJECT_ADAPTER:
          try
          {
            if (mockUser != null)
            {
              EntityObjectService = new EntityObjectService<DataServerTestEntity>(DataAccess.Object);
              EntityObjectService.GetExistsByCriteria(mockUser.Object, criteria, dataSession);
            }
            else
            {
              EntityObjectService = new EntityObjectService<DataServerTestEntity>(DataAccess.Object);
              EntityObjectService.GetExistsByCriteria(null, criteria, dataSession);
            }
          }
          catch (Exception ex)
          {
            exception = ex;
          }
          break;
      }
    }

    [Given(@"mock setup return object as (.*)")]
    public void GivenMockSetupReturnObjectAs(object sqlReturnObjectType)
    {
      if (sqlReturnObjectType.Equals("null"))
      {
        sqlReturnObjectType = null;
      }
      IsqRead = new Mock<ISQLReadable>();
      IsqRead.Setup(ir => ir.SingleOrDefaultScalar<object>()).Returns(sqlReturnObjectType);
      DataAccess.Setup(da => da
         .GetByCriteria(dataSession, mockUser.Object, It.IsAny<ICriteria>()
         , CriteriaQueryType.Exists))
         .Returns(IsqRead.Object);
      Assert.IsNotNull(IsqRead.Object);
    }

    [Then(@"null exception is thrown")]
    public void ThenNullExceptionIsThrown()
    {
      Assert.AreEqual(typeof(ArgumentNullException), exception.GetType());
    }

    [Given(@"various parameters")]
    public void GivenVariousParameters()
    {
      EntityService = new Mock<IEntityObjectService<DataServerTestEntity>>();
      mockDataServices = new MockDataServices();
    }

    [Then(@"appropriate response is shown")]
    public void ThenAppropriateResponseIsShown()
    {
      Assert.IsFalse(dataSession.GetExistsByCriteria<DataServerTestEntity>(mockUser.Object, criteria));
    }

    [Then(@"appropriate response is shown as expected result (.*)")]
    public void ThenAppropriateResponseIsShown(bool expectedResult)
    {
      Assert.AreEqual(expectedResult, Service.GetExistsByCriteria(mockUser.Object, criteria, dataSession));
    }
  }
}