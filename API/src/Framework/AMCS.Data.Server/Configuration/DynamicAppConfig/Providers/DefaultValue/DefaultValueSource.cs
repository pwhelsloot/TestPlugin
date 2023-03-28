namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.DefaultValue
{
  using System;

  public class DefaultValueSource : IConfigurationSource
  {
    public Type ConfigurationProviderType => typeof(DefaultValueProvider);
    public int Order => 0;

    public void Register<T>(IConfigurationProvider provider, IConfigurationValueUpdater updater)
    {
      updater.SetValue(((DefaultValueProvider)provider).DefaultValue);
    }
  }
}
