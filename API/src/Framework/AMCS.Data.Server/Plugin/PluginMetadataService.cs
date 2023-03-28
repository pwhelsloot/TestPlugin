namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Immutable;
  using System.Linq;
  using System.Threading.Tasks;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Server.Plugin.UpdateNotification;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;

  public class PluginMetadataService : IPluginMetadataService
  {
    private readonly object syncRoot = new object();

    private readonly List<Action<string, PluginMetadata, IList<PluginDependency>>> registrations =
      new List<Action<string, PluginMetadata, IList<PluginDependency>>>();

    private readonly IPluginSystem pluginSystem;

    public PluginMetadataService(IPluginSystem pluginSystem)
    {
      this.pluginSystem = pluginSystem;
    }

    public void Register(Action<string, PluginMetadata, IList<PluginDependency>> callback)
    {
      lock (syncRoot)
      {
        registrations.Add(callback);
      }
    }

    public PluginMetadata GetMetadata(string tenantId, PluginConfiguration configuration)
    {
      List<Action<string, PluginMetadata, IList<PluginDependency>>> registrationCopies;

      lock (syncRoot)
      {
        registrationCopies = registrations.ToList();
      }

      var pluginMetadata = new PluginMetadata
      {
        Plugin = pluginSystem.FullyQualifiedName,
        Version = pluginSystem.CurrentVersion
      };

      registrationCopies.ForEach(registration =>
      {
        registration(tenantId, pluginMetadata, ImmutableList.Create(configuration.PluginDependencies.ToArray()));
      });

      return pluginMetadata;
    }

    public async Task ForceMex()
    {
      DataServices.TryResolve<IPluginUpdateNotificationService>(out var notificationService);

      if (notificationService == null)
        throw new InvalidOperationException("PluginUpdateNotificationService is not configured");

      var tasks = notificationService.Update(true);
      await Task.WhenAll(tasks);
    }
  }
}