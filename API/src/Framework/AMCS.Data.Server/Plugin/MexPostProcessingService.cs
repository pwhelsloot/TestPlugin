namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;
  using PluginData.Data.Configuration;

  public class MexPostProcessingService : IMexPostProcessingService
  {
    private readonly List<Func<PluginConfiguration, string, string>> registrations = new List<Func<PluginConfiguration, string, string>>();
    private readonly Version version811 = new Version("8.11.0.0");
    private const string PluginMetadataXmlns = "https://schemas.amcsgroup.io/platform/plugin-metadata/2021-12";

    public void Start()
    {
      registrations.Add(AddAcceptsOAuthToken);
    }

    public string ExecuteGetMetadataPostProcessing(PluginConfiguration pluginConfiguration, string pluginMetadataXml)
    {
      foreach (var registration in registrations)
      {
        pluginMetadataXml = registration(pluginConfiguration, pluginMetadataXml);
      }

      return pluginMetadataXml;
    }

    public string AddAcceptsOAuthToken(PluginConfiguration pluginConfiguration, string pluginMetadataXml)
    {
      var corePlugin = pluginConfiguration.PluginDependencies.SingleOrDefault(plugin => PluginHelper.IsCore(plugin.Name));
      
      if (IsAfter810())
        return pluginMetadataXml;

      var xDocument = XDocument.Parse(pluginMetadataXml);

      var reverseProxyRules = xDocument
        .Elements(XName.Get("PluginMetadata", PluginMetadataXmlns))
        .Elements(XName.Get("ReverseProxyRules", PluginMetadataXmlns))
        .Elements(XName.Get("ReverseProxyRule", PluginMetadataXmlns));

      foreach (var reverseProxyRule in reverseProxyRules)
      {
        reverseProxyRule.SetAttributeValue("AcceptsOAuthToken", false);
      }
      
      return xDocument.ToString();

      bool IsAfter810()
      {
        if (corePlugin == null) 
          return true;
        
        var coreVersion = new Version(corePlugin.Version);
        return coreVersion.Major == 0 || coreVersion.CompareTo(version811) >= 0;
      }
    }
  }
}
