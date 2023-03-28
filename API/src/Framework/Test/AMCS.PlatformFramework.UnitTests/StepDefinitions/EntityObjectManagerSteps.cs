using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class EntityObjectManagerSteps
  {
    private ITypeManager? typeManager;
    private Exception exception = new Exception();
    private EntityObjectManager entityObjectManager;

    [Given(@"type manager as (.*)")]
    public void GivenTypeManagerAsNull(string typeManagerValue)
    {
      switch (typeManagerValue.ToUpperInvariant())
      {
        case "NULL":
          typeManager = null;
          break;
        case "EMPTY":
          typeManager = TypeManager.FromFiles(new List<string>());
          break;
      }
    }

    [When(@"entity object manager is initiated")]
    public void WhenEntityObjectManagerIsInitiated()
    {
      try
      {
        entityObjectManager = new EntityObjectManager(typeManager);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    [Then(@"null reference exception is thrown")]
    public void ThenNullReferenceExceptionIsThrown()
    {
      Assert.AreEqual(typeof(NullReferenceException), exception.GetType());
    }

    [Then(@"entity count is zero")]
    public void ThenEntityCountIsZero()
    {
      Assert.IsNotNull(entityObjectManager);
      Assert.That(entityObjectManager.Entities.Count == 0);
    }
  }
}