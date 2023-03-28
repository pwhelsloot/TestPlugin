namespace AMCS.Data.Server.TestData
{
  public interface ITestDataGenerator<TConfig, TOptions>
  {
    void Generate(TestDataBuilder builder, TConfig configuration, TOptions options, ISessionToken userId, IDataSession dataSession);
  }
}
