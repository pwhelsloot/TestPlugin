namespace AMCS.Data.Entity.Plugin
{
  public class PluginSystem : IPluginSystem
  {
    public string VendorId { get; }

    public string PluginId { get; }

    public string FullyQualifiedName { get; }

    public string CurrentVersion { get; }
    
    public PluginSystem(string vendorId, string pluginId, string currentVersion)
    {
      VendorId = vendorId;
      PluginId = pluginId;
      CurrentVersion = currentVersion;
      FullyQualifiedName = $"{vendorId}/{pluginId}";
    }
  }
}
