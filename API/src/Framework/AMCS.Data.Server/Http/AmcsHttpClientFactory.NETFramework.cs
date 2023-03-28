#if NETFRAMEWORK
namespace AMCS.Data.Server.Http
{
  using System;
  using System.Net.Http;
  using Entity.Tenants;

  public class AmcsHttpClientFactory : IAmcsHttpClientFactory
  {
    private readonly HttpClient httpClient = new HttpClient(new HttpClientHandler { UseCookies = false });
    
    public const string PlatformClient = "PlatformClient";
    public const string PlatformCredentialsClient = "PlatformCredentialsClient";

    public HttpClient CreateClient(string name, string tenantId)
    {
      return name?.Equals(PlatformCredentialsClient, StringComparison.InvariantCultureIgnoreCase) == true 
        ? new HttpClient(HttpCredentialsUtil.CreatePlatformCredentialsClientClientHandler(tenantId)) 
        : httpClient;
    }

    public HttpClient CreatePlatformClient() => CreateClient(PlatformClient, null);
    
    public HttpClient CreatePlatformCredentialsClient(string tenantId) => CreateClient(PlatformCredentialsClient, tenantId);
    public HttpClient CreatePlatformCredentialsClient(ITenant tenant) => CreateClient(PlatformCredentialsClient, tenant.TenantId);
  }
}
#endif