namespace AMCS.Data.Server.Http
{
  using System.Net.Http;
  using System.Threading.Tasks;
  using Entity.Tenants;
  using PlatformCredentials;
  using Plugin;

  internal class AmcsHttpClient : IHttpClient
  {
    private readonly IAmcsHttpClientFactory httpClientFactory;

    public AmcsHttpClient(IAmcsHttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory;
    }
    
    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
      var httpClient = httpClientFactory.CreatePlatformClient();

      var responseMessage = await httpClient.GetAsync(requestUri);
      return responseMessage;
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
      var httpClient = httpClientFactory.CreatePlatformClient();
     
      var responseMessage = await httpClient.PostAsync(requestUri, content);
      return responseMessage;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
    {
      var httpClient = httpClientFactory.CreatePlatformClient();
     
      var responseMessage = await httpClient.SendAsync(requestMessage);
      return responseMessage;
    }

    public Task<HttpResponseMessage> SendAsyncWithCoreCredentials(
      HttpRequestMessage requestMessage,
      ITenant tenant) => SendAsyncWithCoreCredentials(requestMessage, tenant.TenantId);
    
    public async Task<HttpResponseMessage> SendAsyncWithCoreCredentials(HttpRequestMessage requestMessage, string tenantId)
    {
      var httpClient = httpClientFactory.CreatePlatformCredentialsClient(tenantId);
      
      var platformCredentials = new PlatformCredentials(PluginHelper.GetCoreAppCredentials(), tenantId);
      var token = DataServices.Resolve<IPlatformCredentialsTokenManager>().Serialize(platformCredentials);
      requestMessage.Headers.Add("Cookie", $"{PlatformCredentials.CookieName}={token}");

      var responseMessage = await httpClient.SendAsync(requestMessage);
      return responseMessage;
    }
  }
}