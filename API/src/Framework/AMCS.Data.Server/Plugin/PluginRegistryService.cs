namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using AMCS.PluginData.Data.MetadataRegistry.Plugins;
  using Core;
  using Data.Configuration;
  using Entity.Tenants;
  using PluginData.Data.Metadata.MetadataRegistries;
  using Services;
  using Support;
  using WebHook;

  internal class PluginRegistryService : IPluginRegistryService
  {
    private readonly ConcurrentDictionary<ITenant, PluginRegistry> cachedRegistries =
      new ConcurrentDictionary<ITenant, PluginRegistry>();

    private readonly ICorePluginHttpService httpService;
    private readonly ITenantManager tenantManager;

    public PluginRegistryService(ICorePluginHttpService httpService, ITenantManager tenantManager)
    {
      this.httpService = httpService;
      this.tenantManager = tenantManager;
    }

    public void Start()
    {
      // Don't want it to block startup, so run in background thread
      TaskUtils.RunBackground(Update);

      if (DataServices.TryResolve<IMexUpdatedService>(out var mexUpdatedService))
      {
        mexUpdatedService.MexUpdated += async (eventArgs) =>
        {
          await Update();
        };
      }
    }

    public PluginRegistry Get(ITenant tenant)
    {
      return cachedRegistries[tenant];
    }

    public async Task Update()
    {
      var currentTenants = tenantManager.GetAllTenants();
      var tasks = currentTenants.Select(tenant => Task.Run(async () =>
        {
          var registry = await httpService.GetMetadataRegistry<PluginRegistry>(tenant, MetadataRegistryType.Plugins);
          return (tenant, registry);
        }))
        .ToList();

      var results = await Task.WhenAll(tasks);

      foreach (var result in results)
      {
        cachedRegistries[result.tenant] = result.registry;
      }
    }
  }
}