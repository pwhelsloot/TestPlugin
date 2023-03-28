using AMCS.Data.Entity.Tenants;

namespace AMCS.PlatformFramework.UnitTests
{
    public class TenantMockEntity : ITenant
    {
        public string TenantId { get; }

        public string CoreAppServiceRoot { get; }

        public TenantMockEntity(string tenantId, string coreAppServiceRoot)
        {
            this.TenantId = tenantId;
            this.CoreAppServiceRoot = coreAppServiceRoot;
        }
    }
}