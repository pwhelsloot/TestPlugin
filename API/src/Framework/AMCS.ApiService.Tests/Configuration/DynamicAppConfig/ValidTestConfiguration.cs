namespace AMCS.ApiService.Tests.Configuration.DynamicAppConfig
{
  using Data.Server.Configuration.DynamicAppConfig;
  using Data.Server.Configuration.DynamicAppConfig.Providers.DefaultValue;

  public class ValidTestConfigurationKeys
  {
    public static string FeatureFlag1ConfigId = "FeatureFlag1";
    public static bool FeatureFlag1Value = false;

    public static string FeatureFlag2ConfigId = "FeatureFlag2";
    public static string FeatureFlag2Value = "im a string value";
  }

  public class ValidTestConfiguration
  {
    public static readonly Configuration<bool> FeatureFlag1 = ConfigurationService.Create<bool>(
      "FeatureFlag1",
      ValueCombinersBool.And,
      ConfigurationVisibility.Public,
      new DefaultValueProvider(false));

    public static readonly Configuration<string> FeatureFlag2 = ConfigurationService.Create<string>(
      "FeatureFlag2",
      ValueCombiners<string>.MostPrivileged,
      ConfigurationVisibility.Public,
      new DefaultValueProvider("im a string value"));
  }
}
