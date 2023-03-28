using System;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.IntegrationTests.TestProperties;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.StepDefinitions
{
  [Binding]
  public class MyEntityObjectSteps : TestBase
  {
    private const string MYENTITYOBJECTSERVICE = "MYENTITYOBJECTSERVICE";
    private const string MYENTITYOBJECTSERVICE3 = "MYENTITYOBJECTSERVICE3";
    private const string MYENTITYOBJECT1 = "MYENTITYOBJECT1";
    private const string MYENTITYOBJECT2 = "MYENTITYOBJECT2";
    private const string MYENTITYOBJECT3 = "MYENTITYOBJECT3";
    private const string MYSQLENTITYOBJECTACCESS = "MYSQLENTITYOBJECTACCESS";
    private const string MYSQLENTITYOBJECTACCESS1 = "MYSQLENTITYOBJECTACCESS1";
    private ScenarioContext scenarioContext;
    private int MyId = 0;
    private Type? EntityType;
    private Type? MyEntityType;
    private string EntityObjectService = string.Empty;

    public MyEntityObjectSteps(ScenarioContext scenarioContext)
    {
      this.scenarioContext = scenarioContext;
    }
    [Given(@"an (.*) entity object service (.*) entity object")]
    public void GivenAnMyEntityObjectServiceEntityObjectServiceMyEntityObjectEntityObject(string entityObjectService, string entityObject)
    {
      EntityObjectService = entityObjectService;
      switch (entityObjectService.ToUpperInvariant())
      {
        case MYENTITYOBJECTSERVICE:
          switch (entityObject.ToUpperInvariant())
          {
            case MYENTITYOBJECT1:
              scenarioContext[entityObject] = typeof(MyEntityObjectService<MyEntityObject1>);
              break;
            case MYENTITYOBJECT2:
              scenarioContext[entityObject] = typeof(MyEntityObjectService<MyEntityObject2>);
              break;
            case MYENTITYOBJECT3:
              scenarioContext[entityObject] = typeof(MyEntityObjectService<MyEntityObject3>);
              break;
          }
          break;
        case MYSQLENTITYOBJECTACCESS:
          switch (entityObject.ToUpperInvariant())
          {
            case MYENTITYOBJECT2:
              scenarioContext[entityObject] = typeof(MySQLEntityObjectAccess<MyEntityObject2>);
              break;
            case MYENTITYOBJECT3:
              scenarioContext[entityObject] = typeof(MySQLEntityObjectAccess<MyEntityObject3>);
              break;
          }
          break;
        case MYSQLENTITYOBJECTACCESS1:
          scenarioContext[entityObject] = typeof(MySQLEntityObjectAccess1);
          break;
      }
      // Assert.AreEqual(43, (entityObject)DataServices.Resolve<IEntityObjectService<entityObject.cla>>()).MyMethod().MyId);
    }

    [When(@"entity object service (.*) entity object (.*) is passed to data services")]
    public void WhenEntityObjectServiceMyEntityObjectServiceEntityObjectMyEntityObjectIsPassedToDataServices(string entityObjectService, string entityObject)
    {
      switch (entityObjectService.ToUpperInvariant())
      {
        case MYENTITYOBJECTSERVICE:
          switch (entityObject.ToUpperInvariant())
          {
            case MYENTITYOBJECT1:
              MyId = ((MyEntityObjectService<MyEntityObject1>)DataServices.Resolve<IEntityObjectService<MyEntityObject1>>()).MyMethod().MyId;
              EntityType = DataServices.Resolve<IEntityObjectService<MyEntityObject1>>().GetType();
              break;
            case MYENTITYOBJECT2:
              MyId = ((MyEntityObjectService<MyEntityObject2>)DataServices.Resolve<IEntityObjectService<MyEntityObject2>>()).MyMethod().MyId;
              EntityType = DataServices.Resolve<IEntityObjectService<MyEntityObject2>>().GetType();
              break;
            case MYENTITYOBJECT3:
              MyId = ((MyEntityObjectService<MyEntityObject3>)DataServices.Resolve<IEntityObjectService<MyEntityObject3>>()).MyMethod().MyId;
              break;
          }
          break;
        case MYSQLENTITYOBJECTACCESS:
          switch (entityObject.ToUpperInvariant())
          {
            case MYENTITYOBJECT2:
              EntityType = DataServices.Resolve<IEntityObjectAccess<MyEntityObject2>>().GetType();
              MyEntityType = DataServices.Resolve<IMyEntityObjectAccess<MyEntityObject2>>().GetType();
              break;
            case MYENTITYOBJECT3:
              EntityType = DataServices.Resolve<IEntityObjectAccess<MyEntityObject3>>().GetType();
              MyEntityType = DataServices.Resolve<IMyEntityObjectAccess<MyEntityObject3>>().GetType();
              break;
          }
          break;
        case MYSQLENTITYOBJECTACCESS1:
          EntityType = DataServices.Resolve<IEntityObjectAccess<MyEntityObject1>>().GetType();
          MyEntityType = DataServices.Resolve<IMyEntityObjectAccess<MyEntityObject1>>().GetType();
          break;
      }
      if (entityObject.ToUpperInvariant().Equals(MYENTITYOBJECTSERVICE3))
      {
        EntityType = EntityType = DataServices.Resolve<IEntityObjectService<MyEntityObject3>>().GetType();
      }
      //scenarioContext["gg"]= ((scenarioContext.Get<Type>(entityObject))DataServices.Resolve<IEntityObjectService<MyEntityObject1>>()).MyMethod().MyId;
    }

    [Then(@"ID (.*) and type (.*) matches expected result")]
    public void ThenIDAndTypeMyEntityObjectMatchesExpectedResult(int expectedId, string entityObject)
    {
      switch (EntityObjectService.ToUpperInvariant())
      {
        case MYENTITYOBJECTSERVICE:
          switch (entityObject.ToUpperInvariant())
          {
            case MYENTITYOBJECT1:
              Assert.AreEqual(expectedId, MyId);
              Assert.AreEqual(typeof(MyEntityObjectService<MyEntityObject1>), EntityType);
              break;
            case MYENTITYOBJECT2:
              Assert.AreEqual(expectedId, MyId);
              Assert.AreEqual(typeof(MyEntityObjectService<MyEntityObject2>), EntityType);
              break;
            case MYENTITYOBJECT3:
              Assert.AreEqual(expectedId, MyId);
              break;
          }
          break;
        case MYSQLENTITYOBJECTACCESS:
          switch (entityObject.ToUpperInvariant())
          {
            case MYENTITYOBJECT2:
              Assert.AreEqual(typeof(MySQLEntityObjectAccess<MyEntityObject2>), EntityType);
              Assert.AreEqual(typeof(MySQLEntityObjectAccess<MyEntityObject2>), MyEntityType);
              break;
            case MYENTITYOBJECT3:
              Assert.AreEqual(typeof(MySQLEntityObjectAccess<MyEntityObject3>), EntityType);
              Assert.AreEqual(typeof(MySQLEntityObjectAccess<MyEntityObject3>), MyEntityType);
              break;
          }
          break;
        case MYSQLENTITYOBJECTACCESS1:
          Assert.AreEqual(typeof(MySQLEntityObjectAccess1), EntityType);
          Assert.AreEqual(typeof(MySQLEntityObjectAccess1), MyEntityType);

          break;
      }
      if (entityObject.ToUpperInvariant().Equals(MYENTITYOBJECTSERVICE3))
      {
        Assert.AreEqual(typeof(MyEntityObjectService3), EntityType);
      }
    }
  }
}