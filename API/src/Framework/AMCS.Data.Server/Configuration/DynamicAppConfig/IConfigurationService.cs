namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;
  using System.Collections.Generic;
  using Data;

  public interface IConfigurationService
  {
    List<ConfigValue> GetPublicConfigValues(ISessionToken userId);

    Type GetAzureAppConfigurationType(string configId);
  }
}