namespace AMCS.PlatformFramework.Server.Configuration
{
  using Data.Server.Configuration.DynamicAppConfig;
  using Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration;
  using Data.Server.Configuration.DynamicAppConfig.Providers.DefaultValue;

  public static class PlatformFrameworkConfigurations
  {
    public static readonly TenantConfiguration<string> MyTenantConfiguration = ConfigurationService.CreateTenant(
      "MyTenantConfiguration",
      ValueCombiners<string>.MostPrivileged,
      ConfigurationVisibility.Public,
      new AzureAppConfigurationTenantProvider("MyTenantConfiguration"),
      new DefaultValueProvider("default"));

    public static readonly Configuration<string> MyGlobalConfiguration = ConfigurationService.Create(
      "MyGlobalConfiguration",
      ValueCombiners<string>.MostPrivileged,
      ConfigurationVisibility.Public,
      new AzureAppConfigurationGlobalProvider("MyGlobalConfiguration"),
      new DefaultValueProvider("default"));

    public static readonly TenantConfiguration<bool> MyTenantFeatureFlag = ConfigurationService.CreateTenant(
      "MyTenantFeatureFlag",
      ValueCombinersBool.Or,
      ConfigurationVisibility.Public,
      new AzureAppConfigurationTenantProvider("MyTenantFeatureFlag"),
      new DefaultValueProvider(false));

    public static readonly Configuration<bool> MyGlobalFeatureFlag = ConfigurationService.Create(
      "MyGlobalFeatureFlag",
      ValueCombinersBool.Or,
      ConfigurationVisibility.Public,
      new AzureAppConfigurationGlobalProvider("MyGlobalFeatureFlag"),
      new DefaultValueProvider(false));

    public static readonly TenantConfiguration<bool> MyTenantFeatureFlag2 = ConfigurationService.CreateTenant(
      "MyTenantFeatureFlag2",
      ValueCombinersBool.Or,
      ConfigurationVisibility.Public,
      new AzureAppConfigurationTenantProvider("MyTenantFeatureFlag2"),
      new DefaultValueProvider(false));
  }
}