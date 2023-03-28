namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  
  public interface IMetadataRegistryMetadataProcessor
  {
    void Process(
      IList<PluginData.Data.Metadata.MetadataRegistries.MetadataRegistry> metadataRegistries,
      string fullyQualifiedPluginName,
      ISessionToken sessionToken, IDataSession dataSession);
  }
}