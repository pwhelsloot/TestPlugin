using AMCS.PluginData.Data.Metadata;

namespace AMCS.Data.Server.Plugin
{
  public interface IMetadataProcessor
  {
    void ProcessMetadata(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession);
  }
}