#if NETFRAMEWORK

using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos
{
  [Authenticated]
  public class AsyncMessageServiceController<TService, TRequest, TResponse> : MessageControllerBase<TResponse>
    where TService : IAsyncMessageService<TRequest, TResponse>, new()
  {
    [HttpPost]
    public async Task<ActionResult> Perform()
    {
      // Deserialize request
      TRequest message;
      Request.InputStream.Position = 0;
      using (var streamReader = new StreamReader(Request.InputStream))
      using (var jsonTextReader = new JsonTextReader(streamReader))
      {
        JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
        message = serializer.Deserialize<TRequest>(jsonTextReader);
      }

      // Process request
      var user = HttpContext.GetAuthenticatedUser();
      var response = await new TService().Perform(user, message);
      return GetResponse(response);
    }
  }
}

#endif