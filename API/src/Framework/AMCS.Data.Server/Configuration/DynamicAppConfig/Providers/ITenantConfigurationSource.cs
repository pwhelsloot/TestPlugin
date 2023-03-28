namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers
{
  public interface ITenantConfigurationSource : IBaseConfigurationSource
  {
    void Register<T>(IConfigurationProvider provider, ITenantConfigurationValueUpdater updater);
  }
}
