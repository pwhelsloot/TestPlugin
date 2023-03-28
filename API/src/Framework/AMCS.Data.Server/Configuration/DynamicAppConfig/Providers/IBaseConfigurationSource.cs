namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers
{
  using System;

  public interface IBaseConfigurationSource
  {
    Type ConfigurationProviderType { get; }
    int Order { get; }
  }
}
