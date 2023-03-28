namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  public class AzureAppConfigurationPrefix
  {
    public string Prefix { get; }
      
    public AzureAppConfigurationPrefixType PrefixType { get; }

    public AzureAppConfigurationPrefix(string prefix, AzureAppConfigurationPrefixType prefixType)
    {
      Prefix = prefix;
      PrefixType = prefixType;
    }
  }
}