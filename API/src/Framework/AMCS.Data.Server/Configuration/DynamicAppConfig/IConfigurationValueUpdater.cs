namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  public interface IConfigurationValueUpdater
  {
    void SetValue(object value);
    void ClearValue();
  }
}
