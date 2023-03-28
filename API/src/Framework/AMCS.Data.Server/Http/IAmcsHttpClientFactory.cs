namespace AMCS.Data.Server.Http
{
  using System.Net.Http;
  using Entity.Tenants;

  public interface IAmcsHttpClientFactory
  {
    HttpClient CreateClient(string name, string tenantId);

    HttpClient CreatePlatformClient();

    HttpClient CreatePlatformCredentialsClient(string tenantId);
    HttpClient CreatePlatformCredentialsClient(ITenant tenant);
  }
}