namespace AMCS.Data.Server.Services
{
  using System.Collections.Generic;
  using AMCS.Data.Entity.Tenants;
  
  public interface ITenantManager
  {
    ITenant GetTenant(string tenantId);

    IList<ITenant> GetAllTenants();

    void AddTenant(string tenantId, string coreServiceRootUrl);

    event TenantsChangedEventHandler TenantsChanged;
  }
}