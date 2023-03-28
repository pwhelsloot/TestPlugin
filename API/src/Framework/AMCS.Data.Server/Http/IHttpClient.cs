namespace AMCS.Data.Server.Http
{
  using System.Net.Http;
  using System.Threading.Tasks;
  using Entity.Tenants;

  public interface IHttpClient
  {
    Task<HttpResponseMessage> GetAsync(string requestUri);
    Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage);
    Task<HttpResponseMessage> SendAsyncWithCoreCredentials(HttpRequestMessage requestMessage, ITenant tenant);
    Task<HttpResponseMessage> SendAsyncWithCoreCredentials(HttpRequestMessage requestMessage, string tenantId);
  }
}