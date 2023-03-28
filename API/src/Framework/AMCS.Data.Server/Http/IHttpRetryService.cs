namespace AMCS.Data.Server.Http
{
  using System;
  using System.Net.Http;
  using System.Threading.Tasks;

  public interface IHttpRetryService
  {
    Task<HttpResponseMessage> ExecuteHttpWithRetry(Func<Task<HttpResponseMessage>> callback);
  }
}