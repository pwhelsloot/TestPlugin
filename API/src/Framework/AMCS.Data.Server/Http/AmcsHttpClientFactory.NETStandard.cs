#if !NETFRAMEWORK
namespace AMCS.Data.Server.Http
{
  using System.Net.Http;
  using Entity.Tenants;
  using Microsoft.Extensions.DependencyInjection;

  public class AmcsHttpClientFactory : IAmcsHttpClientFactory
  {
    private readonly IHttpClientFactory httpClientFactory;

    public const string PlatformClient = "PlatformClient";
    public const string PlatformCredentialsClient = "PlatformCredentialsClient";
    
    public AmcsHttpClientFactory()
    {
      // cannot use the IAppSetupService RegisterConfigureServices here as it's too slow
      // web hooks etc can run before it completes
      var serviceCollection = new ServiceCollection();

      serviceCollection
        .AddHttpClient(PlatformCredentialsClient);

      serviceCollection
        .AddHttpClient(PlatformClient)
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false });
      
      httpClientFactory = serviceCollection
        .BuildServiceProvider()
        .GetService<IHttpClientFactory>();
    }

    public HttpClient CreateClient(string name, string tenantId)
    {
      return httpClientFactory.CreateClient(name);
    }

    public HttpClient CreatePlatformClient() => CreateClient(PlatformClient, null);
    
    public HttpClient CreatePlatformCredentialsClient(string tenantId) => CreateClient(PlatformCredentialsClient, tenantId);
    public HttpClient CreatePlatformCredentialsClient(ITenant tenant) => CreateClient(PlatformCredentialsClient, tenant.TenantId);
  }
}
#endif