namespace AMCS.Data.Entity.Tenants
{
  public class Tenant : ITenant
  {
    public string TenantId { get; }

    public string CoreAppServiceRoot { get; }

    public Tenant(string tenantId, string coreAppServiceRoot)
    {
      TenantId = tenantId;
      CoreAppServiceRoot = coreAppServiceRoot;
    }
  }
}