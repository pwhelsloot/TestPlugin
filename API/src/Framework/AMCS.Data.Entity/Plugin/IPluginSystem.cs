namespace AMCS.Data.Entity.Plugin
{
  public interface IPluginSystem
  {
    string VendorId { get; }

    string PluginId { get; }
    
    string FullyQualifiedName { get; }

    string CurrentVersion { get; }
  }
}