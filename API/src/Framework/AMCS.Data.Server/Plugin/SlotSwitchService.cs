namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;

  public class SlotSwitchService : ISlotSwitchService
  {
    private readonly object syncRoot = new object();

    private readonly List<Func<PluginMetadata, PluginConfiguration, bool>> registrations =
      new List<Func<PluginMetadata, PluginConfiguration, bool>>();

    public void Register(Func<PluginMetadata, PluginConfiguration, bool> callback)
    {
      lock (syncRoot)
      {
        registrations.Add(callback);
      }
    }

    public bool IsSlotSwitching(PluginMetadata pluginMetadata, PluginConfiguration pluginConfiguration)
    {
      List<Func<PluginMetadata, PluginConfiguration, bool>> registrationCopies;

      lock (syncRoot)
      {
        registrationCopies = registrations.ToList();
      }

      var isSlotSwitching = registrationCopies
        .Any(registration => registration(pluginMetadata, pluginConfiguration));

      return isSlotSwitching;
    }
  }
}