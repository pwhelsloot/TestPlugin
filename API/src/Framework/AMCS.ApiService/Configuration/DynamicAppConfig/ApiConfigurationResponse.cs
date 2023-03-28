using System.Collections.Generic;

namespace AMCS.ApiService.Configuration.DynamicAppConfig
{
  /// <summary>
  /// Response for configuration/dynamicAppConfiguration
  /// </summary>
  public class ApiConfigurationResponse
  {
    /// <summary>
    /// List of all configurationValues for a Tenant
    /// </summary>
    public IEnumerable<ApiConfigValue> ConfigurationValues { get; set; }
  }
}
