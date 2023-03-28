namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Data.Configuration;
  using Entity.Tenants;
  using PluginData.Data.MetadataRegistry.Plugins;

  public interface IPluginRegistryService : IDelayedStartup
  {
    PluginRegistry Get(ITenant tenant);
    
    Task Update();
  }
}