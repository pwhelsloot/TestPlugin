namespace AMCS.PlatformFramework.Server.TestData
{
  using AMCS.Data.Server.TestData;

  public interface IPlatformFrameworkTestDataGenerator<TOptions>
    : ITestDataGenerator<PlatformFrameworkTestDataConfiguration, TOptions>
  {
  }
}
