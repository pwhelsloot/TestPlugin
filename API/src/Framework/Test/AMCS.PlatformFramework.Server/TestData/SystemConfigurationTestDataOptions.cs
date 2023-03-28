namespace AMCS.PlatformFramework.Server.TestData
{
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.DataSets.SystemConfiguration;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using Newtonsoft.Json;

  [TestDataOptions(DisplayName: "SystemConfiguration", DataSetRecordType: typeof(SystemConfigurationRecord), DependendantTypes: typeof(UserRecord))]
  public class SystemConfigurationTestDataOptions
  {
    public int Number { get; }

    [JsonConstructor]
    public SystemConfigurationTestDataOptions(int number)
    {
      Number = number;
    }

    public SystemConfigurationTestDataOptions()
      : this(0)
    {
    }
  }
}
