namespace AMCS.PlatformFramework.Server.TestData
{
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using Newtonsoft.Json;

  [TestDataOptions(DisplayName: "User", DataSetRecordType: typeof(UserRecord))]
  public class UserTestDataOptions
  {
    private const string EmailDomainDefaultValue = "amcstest.com";
    public int Number { get; }
    public string EmailDomain { get; }

    [JsonConstructor]
    public UserTestDataOptions(int number, string emailDomain)
    {
      Number = number;
      EmailDomain = !string.IsNullOrEmpty(emailDomain) ? emailDomain : EmailDomainDefaultValue;
    }

    public UserTestDataOptions(int number)
        : this(number, EmailDomainDefaultValue)
    {
    }

    public UserTestDataOptions()
        : this(0, EmailDomainDefaultValue)
    {
    }
  }
}
