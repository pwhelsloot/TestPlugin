namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class TestDataOptionsValidationFixture
  {
    [Test]
    public void UserTestDataOptionsWillPassValidation()
    {
      Assert.DoesNotThrow(() => TestDataValidator.ValidateOptionType(typeof(UserTestDataOptions)));
    }

    [Test]
    public void SystemConfigurationTestDataOptionsWillPassValidation()
    {
      Assert.DoesNotThrow(() => TestDataValidator.ValidateOptionType(typeof(SystemConfigurationTestDataOptions)));
    }
  }
}
