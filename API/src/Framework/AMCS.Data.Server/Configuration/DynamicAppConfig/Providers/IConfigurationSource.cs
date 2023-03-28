namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers
{
  /// <summary>
  /// A configuration source can read config settings from a given source.
  /// </summary>
  public interface IConfigurationSource : IBaseConfigurationSource
  {
    void Register<T>(IConfigurationProvider provider, IConfigurationValueUpdater updater);
  }
}
