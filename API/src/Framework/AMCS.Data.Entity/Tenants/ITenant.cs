namespace AMCS.Data.Entity.Tenants
{
  public interface ITenant
  {
    string TenantId { get; }
    string CoreAppServiceRoot { get; }
  }
}