namespace AMCS.Data.Server.Plugin
{
  using Data.Configuration;
  using PluginData.Data.Configuration;

  public interface IMexPostProcessingService : IDelayedStartup
  {
    string ExecuteGetMetadataPostProcessing(PluginConfiguration pluginConfiguration, string pluginMetadataXml);
    string AddAcceptsOAuthToken(PluginConfiguration pluginConfiguration, string pluginMetadataXml);
  }
}
