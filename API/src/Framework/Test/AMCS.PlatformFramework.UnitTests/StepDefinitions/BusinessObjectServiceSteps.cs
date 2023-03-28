using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class BusinessObjectServiceSteps
  {
    private const string DUMMY_OBJECT = "DUMMYOBJECT";
    private const string DUMMY_ENTITY = "DUMMYENTITY";
    private ScenarioContext scenarioContext;
    private const string SERVICE = "Service";
    private const string RESULT = "Result";
    private Exception exception = new Exception();
    private const string MULTIPLE_OBJECT_INSTANCE_ERROR_MESSAGE = "Multiple instances of business object of DummyObject found";
    private const string INVALID_TABLE_NAME_ERROR_MESSAGE = "Business object DummyObject has invalid TableName; Must follow [schema].[table] pattern";
    private const string MULTIPLE_OBJECT_INSTANCE_XML = "InvalidBOMultipleObjectNames.xml";
    private const string INVALID_TABLE_NAME_XML = "InvalidBOTableSchema.xml";
    private string InputInvalidData = string.Empty;
    public BusinessObjectServiceSteps(ScenarioContext scenarioContext)
    {
      this.scenarioContext = scenarioContext;
      scenarioContext["typeManager"] = TypeManager.FromAssemblies(typeof(DummyEntity).Assembly);
    }

    [Given(@"businessObjectService is started")]
    public void GivenBusinessObjectServiceIsStarted()
    {
      scenarioContext[SERVICE] = new BusinessObjectService(typeof(DummyEntity).Assembly, scenarioContext.Get<TypeManager>("typeManager"), "ValidBo.xml");
      scenarioContext[SERVICE] = new BusinessObjectService(typeof(DummyEntityCaseInsensitiveEntity).Assembly, scenarioContext.Get<TypeManager>("typeManager"), "ValidBo.xml");
      ((IDelayedStartup)scenarioContext.Get<BusinessObjectService>(SERVICE)).Start();
    }

    [Given(@"invalid input data (.*)")]
    public void GivenInvalidInputDataInvalidBOMultipleObjectNames_Xml(string inputInvalidData)
    {
      InputInvalidData = inputInvalidData;
      scenarioContext[SERVICE] = new BusinessObjectService(typeof(DummyEntity).Assembly, scenarioContext.Get<TypeManager>("typeManager"), inputInvalidData);
    }

    [When(@"entity is fetched using (.*)")]
    public void WhenEntityIsFetchedUsingDummyObject(string inputType)
    {
      switch (inputType.ToUpperInvariant())
      {
        case DUMMY_OBJECT:
          scenarioContext[RESULT] = scenarioContext.Get<BusinessObjectService>(SERVICE).Get(inputType);
          break;
        case DUMMY_ENTITY:
          scenarioContext[RESULT] = scenarioContext.Get<BusinessObjectService>(SERVICE).Get(typeof(DummyEntity));
          break;
      }
    }

    [When(@"all of objects are requested using service")]
    public void WhenAllOfObjectsAreRequestedUsingService()
    {
      scenarioContext[RESULT] = scenarioContext.Get<BusinessObjectService>(SERVICE).GetAll();
    }

    [When(@"business service is started")]
    public void WhenBusinessServiceIsStarted()
    {
      try
      {
        ((IDelayedStartup)scenarioContext.Get<BusinessObjectService>(SERVICE)).Start();
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    [Then(@"exception is thrown")]
    public void ThenExceptionIsThrown()
    {
      Assert.AreEqual(typeof(InvalidOperationException), exception.GetType());
      switch (InputInvalidData)
      {
        case MULTIPLE_OBJECT_INSTANCE_XML:
          Assert.AreEqual("Multiple instances of business object of DummyObject found", exception.Message);
          break;
        case INVALID_TABLE_NAME_XML:
          Assert.AreEqual("Business object DummyObject has invalid TableName; Must follow [schema].[table] pattern", exception.Message);
          break;
      }
    }

    [Then(@"all of objects are returned")]
    public void ThenAllOfObjectsAreReturned()
    {
      Assert.AreEqual(3, scenarioContext.Get<IList<BusinessObjectResult>>(RESULT).Count);

      var firstResult = scenarioContext.Get<IList<BusinessObjectResult>>(RESULT)[0];
      var secondResult = scenarioContext.Get<IList<BusinessObjectResult>>(RESULT)[1];
      var thirdResult = scenarioContext.Get<IList<BusinessObjectResult>>(RESULT)[2];

      Assert.IsNotNull(firstResult);
      Assert.AreEqual(1, firstResult.Types.Count);

      Assert.IsTrue(firstResult.BusinessObject.AllowWebHooks);
      Assert.IsTrue(firstResult.BusinessObject.AllowUserDefinedFields);
      Assert.AreEqual("dbo.DummyTable", firstResult.BusinessObject.TableName);

      Assert.AreEqual(typeof(DummyEntity), firstResult.Types.Single());

      Assert.IsNotNull(secondResult);
      Assert.AreEqual(1, secondResult.Types.Count);

      Assert.IsFalse(secondResult.BusinessObject.AllowWebHooks);
      Assert.IsTrue(secondResult.BusinessObject.AllowUserDefinedFields);
      Assert.AreEqual("dbo.AnotherDummyTable", secondResult.BusinessObject.TableName);

      Assert.AreEqual(typeof(AnotherDummyEntity), secondResult.Types.Single());

      Assert.IsNotNull(thirdResult);
      Assert.AreEqual(1, thirdResult.Types.Count);

      Assert.IsFalse(thirdResult.BusinessObject.AllowWebHooks);
      Assert.IsTrue(thirdResult.BusinessObject.AllowUserDefinedFields);
      Assert.AreEqual("dbo.dummyEntityCaseInsensitive", thirdResult.BusinessObject.TableName);

      Assert.AreEqual(typeof(DummyEntityCaseInsensitiveEntity), thirdResult.Types.Single());
    }

    [Then(@"correct results are returned")]
    public void ThenCorrectResultsAreReturned()
    {
      Assert.IsNotNull(scenarioContext.Get<BusinessObjectResult>(RESULT));
      Assert.AreEqual(1, scenarioContext.Get<BusinessObjectResult>(RESULT).Types.Count);

      Assert.IsTrue(scenarioContext.Get<BusinessObjectResult>(RESULT).BusinessObject.AllowWebHooks);
      Assert.IsTrue(scenarioContext.Get<BusinessObjectResult>(RESULT).BusinessObject.AllowUserDefinedFields);
      Assert.AreEqual("dbo.DummyTable", scenarioContext.Get<BusinessObjectResult>(RESULT).BusinessObject.TableName);

      Assert.AreEqual(typeof(DummyEntity), scenarioContext.Get<BusinessObjectResult>(RESULT).Types.Single());
    }
  }
}
