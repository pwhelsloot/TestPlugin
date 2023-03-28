namespace AMCS.TestPlugin.Server.Plugin.ReverseProxyRules
{
  using System.Collections.Generic;
  using AMCS.Data.Server.Plugin;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.ReverseProxyRules;

  public class PluginReverseProxyRuleMetadataService
  {
    public PluginReverseProxyRuleMetadataService(IPluginMetadataService pluginMetadataService)
    {
      pluginMetadataService.Register(AddProxyRules);
    }

    private static void AddProxyRules(
      string tenantId, 
      PluginMetadata pluginMetadata,
      IList<PluginDependency> pluginDependencies)
    {
      pluginMetadata.ReverseProxyRules ??= new List<ReverseProxyRule>();

      pluginMetadata.ReverseProxyRules.Add(new ReverseProxyRule
      {
        Prefix = "/testplugin/api/",
        Target = "http://localhost:16932/"
      });
    }
  }
}