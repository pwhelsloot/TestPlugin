namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using AMCS.Data;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.DataSets.Support;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.TestData;

  public static class TestDataGenerationHelper
  {
    public static void GenerateUserTestData(
      TestDataBuilder builder,
      PlatformFrameworkTestDataConfiguration configuration,
      UserTestDataOptions options,
      ISessionToken sessionToken)
    {
      DataServices
          .Resolve<IPlatformFrameworkTestDataGenerator<UserTestDataOptions>>()
          .Generate(builder, configuration, options, sessionToken, null);
    }

    public static void GenerateSystemConfigurationTestData(
      TestDataBuilder builder,
      PlatformFrameworkTestDataConfiguration configuration,
      SystemConfigurationTestDataOptions options,
      ISessionToken sessionToken)
    {
      DataServices
          .Resolve<IPlatformFrameworkTestDataGenerator<SystemConfigurationTestDataOptions>>()
          .Generate(builder, configuration, options, sessionToken, null);
    }

    public static DataSetImportResultReader Import(DataSetImport dataSetImport, ISessionToken sessionToken)
    {
      return new DataSetImportResultReader(DataServices.Resolve<IDataSetService>().Import(sessionToken, dataSetImport));
    }
  }
}
