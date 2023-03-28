namespace AMCS.PlatformFramework.Server.TestData
{
  using AMCS.Data.Server.TestData;
  using Newtonsoft.Json;

  [TestDataConfiguration("Configuration")]
  public class PlatformFrameworkTestDataConfiguration
  {
    private const string CurrencyDefaultValue = "EUR";
    public string Currency { get; }

    [JsonConstructor]
    public PlatformFrameworkTestDataConfiguration(string currency)
    {
      Currency = !string.IsNullOrEmpty(currency) ? currency : CurrencyDefaultValue;
    }

    public PlatformFrameworkTestDataConfiguration()
      : this(CurrencyDefaultValue)
    {
    }
  }
}
