namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System.Collections.Generic;
  using System.Linq;

  public class AzureAppConfigurationTenantProvider : IConfigurationProvider
  {
    private static readonly object syncRoot = new object();

    private static readonly List<string> tenantConfigurations = new List<string>();

    public string Configuration { get; }

    public AzureAppConfigurationTenantProvider(string configuration)
    {
      Configuration = configuration;

      lock (syncRoot)
        tenantConfigurations.Add(configuration);
    }

    public static List<string> GetTenantConfigurations()
    {
      lock (syncRoot)
        return tenantConfigurations.ToList();
    }
  }
}