namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Tools
{
  using System.Collections.Generic;

  public class AzureAppConfigurations
  {
    public List<object> FeatureFlags { get; set; } = new List<object>();

    public List<object> ConfigurationKeys { get; set; } = new List<object>();
  }
}