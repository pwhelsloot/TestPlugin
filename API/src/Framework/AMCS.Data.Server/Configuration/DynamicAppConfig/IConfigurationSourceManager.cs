namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using Providers;

  public interface IConfigurationSourceManager
  {
    int? GetOrder(IConfigurationProvider provider);
    void Register<T>(IConfigurationProvider provider, IConfigurationValueUpdater updater);
    void RegisterTenant<T>(IConfigurationProvider provider, ITenantConfigurationValueUpdater updater);
  }
}
