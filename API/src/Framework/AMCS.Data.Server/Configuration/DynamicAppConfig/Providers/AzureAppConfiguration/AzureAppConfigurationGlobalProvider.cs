namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System.Collections.Generic;
  using System.Linq;

  public class AzureAppConfigurationGlobalProvider : IConfigurationProvider
  {
    private static readonly object syncRoot = new object();

    private static readonly List<string> globalConfigurations = new List<string>();

    public string Configuration { get; }

    public AzureAppConfigurationGlobalProvider(string configuration)
    {
      Configuration = configuration;

      lock (syncRoot)
        globalConfigurations.Add(configuration);
    }

    public static List<string> GetGlobalConfigurations()
    {
      lock (syncRoot)
        return globalConfigurations.ToList();
    }
  }
}