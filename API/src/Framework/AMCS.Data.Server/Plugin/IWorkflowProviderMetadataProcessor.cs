namespace AMCS.Data.Server.Plugin
{
  using PluginData.Data.Metadata;

  public interface IWorkflowProviderMetadataProcessor
  {
    void Process(
      PluginMetadata pluginMetadata,
      ISessionToken sessionToken, 
      IDataSession dataSession);
  }
}

