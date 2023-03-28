namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  public interface ITenantConfigurationValueUpdater : IConfigurationValueUpdater
  {
    void SetValue(string tenantId, object value);
    void ClearValue(string tenantId);
  }
}
