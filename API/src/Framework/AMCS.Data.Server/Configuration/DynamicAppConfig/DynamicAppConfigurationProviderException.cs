namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;

  public class DynamicAppConfigurationProviderException : Exception
  {
    public DynamicAppConfigurationProviderException()
    {
    }

    public DynamicAppConfigurationProviderException(string message)
      : base(message)
    {
    }

    public DynamicAppConfigurationProviderException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}