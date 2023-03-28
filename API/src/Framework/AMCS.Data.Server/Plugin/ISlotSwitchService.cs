namespace AMCS.Data.Server.Plugin
{
  using System;
  using PluginData.Data.Configuration;
  using PluginData.Data.Metadata;

  public interface ISlotSwitchService
  {
    void Register(Func<PluginMetadata, PluginConfiguration, bool> callback);
    bool IsSlotSwitching(PluginMetadata pluginMetadata, PluginConfiguration pluginConfiguration);
  }
}