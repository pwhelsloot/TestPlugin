namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  /// <summary>
  /// Base class used by the TenantConfiguration. 
  /// Its main purpose is to ensure the TenantConfigurationValueUpdater only 
  /// has access to RecalculateTenantValue because the ValueUpdater should only Update/Recalculate values and nothing else.
  /// </summary>
  public abstract class TenantConfigurationBase : Configuration
  {
    internal abstract void RecalculateTenantValue(string tenantId);
  }
}
