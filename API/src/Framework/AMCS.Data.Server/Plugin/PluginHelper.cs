namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Linq;
  using AMCS.Data.Entity.Plugin;

  public static class PluginHelper
  {
    private static readonly string CorePluginName = $"{PluginConstants.VendorId.AMCS}/{PluginConstants.PluginId.Core}";

    public static string GetCorePluginFullName() => CorePluginName;
    
    public static string GetCoreAppCredentials() => GetPluginAppCredentials(CorePluginName);
    
    public static string GetPluginAppCredentials(string pluginName) => $"app:{pluginName}";

    public static bool IsCore(string pluginName)
    {
      return string.Equals(CorePluginName, pluginName, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string MergePluginObjectName(string objectName) => $"{DataServices.Resolve<IPluginSystem>().FullyQualifiedName}:{objectName}";
    
    public static string MergePluginObjectName(string pluginName, string objectName) => $"{pluginName}:{objectName}";
    
    public static string GetCoreObjectName(string objectName) => $"{CorePluginName}:{objectName}";

    public static string GetPluginVendor(string pluginName)
    {
      return IsValidPluginName(pluginName)
        ? pluginName.Split('/').First().Replace("app:", string.Empty)
        : string.Empty;
    }

    public static string GetPluginId(string pluginName)
    {
      return IsValidPluginName(pluginName)
        ? pluginName.Split('/').Last()
        : string.Empty;
    }

    public static string GetPluginIdentity(string userIdentity)
    {
      var split = userIdentity.Split(new [] {"app:"}, StringSplitOptions.None);

      // Not a valid app registration if less or greater than 2
      return split.Length != 2 ? null : split.Last();
    }

    public static string GetPluginFullName(string vendorId, string pluginId)
    {
      return $"{vendorId}/{pluginId}";
    }

    public static bool IsValidPluginName(string pluginName)
    {
      var sectors = pluginName.Split('/');
      return sectors.Length == 2;
    }
  }
}