namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Threading.Tasks;
  using System.Collections.Generic;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;

  public interface IPluginMetadataService
  {
    void Register(Action<string, PluginMetadata, IList<PluginDependency>> callback);

    PluginMetadata GetMetadata(string tenantId, PluginConfiguration configuration);

    Task ForceMex();
  }
}