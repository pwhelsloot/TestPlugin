namespace AMCS.PlatformFramework.Server.TestData
{
  using System.Linq;
  using AMCS.Data;
  using AMCS.Data.Server;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.DataSets.SystemConfiguration;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using FizzWare.NBuilder;

  public class SystemConfigurationTestDataGenerator : IPlatformFrameworkTestDataGenerator<SystemConfigurationTestDataOptions>
  {
    public void Generate(TestDataBuilder builder, PlatformFrameworkTestDataConfiguration configuration, SystemConfigurationTestDataOptions options, ISessionToken userId, IDataSession dataSession)
    {
      var items = FizzWare.NBuilder.Builder<SystemConfigurationRecord>.CreateListOfSize(options.Number)
        .All()
        .Do((config, index) => config.Id = (index + 1) * -1)
        .With(config => config.Name = configuration.Currency + builder.NextRandomString() + builder.GetDependantRecord<UserRecord>().UserName)
        .Build()
        .ToList<IDataSetRecord>();
      builder.AddRecords(items);
    }
  }
}
