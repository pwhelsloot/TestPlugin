namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.DefaultValue
{
  public class DefaultValueProvider : IConfigurationProvider
  {
    public object DefaultValue { get; }

    public DefaultValueProvider(object defaultValue)
    {
      DefaultValue = defaultValue;
    }
  }
}
