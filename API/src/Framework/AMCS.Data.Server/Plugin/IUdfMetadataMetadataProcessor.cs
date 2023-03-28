namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public interface IUdfMetadataMetadataProcessor
  {
    void Process(IList<UdfMetadata> udfMetadata, string fullyQualifiedPluginName,
      ISessionToken sessionToken, IDataSession dataSession);
  }
}