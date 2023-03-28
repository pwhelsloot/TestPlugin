namespace AMCS.ApiService.Tests.Configuration.DynamicAppConfig
{
  using Data.Configuration;

  public class ValidTestConfigurationResolver : IDelayedStartup
  {
    public bool FeatureFlag1 { get; private set; }

    public string FeatureFlag2 { get; private set; }

    public void Start()
    {
      FeatureFlag1 = ValidTestConfiguration.FeatureFlag1.Value;
      FeatureFlag2 = ValidTestConfiguration.FeatureFlag2.Value;
    }
  }
}