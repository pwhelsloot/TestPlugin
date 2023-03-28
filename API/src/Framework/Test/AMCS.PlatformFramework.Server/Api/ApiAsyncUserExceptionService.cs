using System.Threading.Tasks;
using AMCS.ApiService.Abstractions;
using AMCS.Data;

namespace AMCS.PlatformFramework.Server.Api
{
  [ServiceRoute("api/async/response")]
  public class ApiAsyncResponseService : IAsyncMessageService<ApiAsyncResponseService.AsyncRequest, ApiAsyncResponseService.AsyncResponse>
  {
    public async Task<AsyncResponse> Perform(ISessionToken userId, AsyncRequest message)
    {
      return await Task.Run(() => new AsyncResponse { ResponseString = "Some async response" });
    }

    public class AsyncRequest
    {
    }

    public class AsyncResponse
    {
      public string ResponseString { get; set; }
    }
  }
}
