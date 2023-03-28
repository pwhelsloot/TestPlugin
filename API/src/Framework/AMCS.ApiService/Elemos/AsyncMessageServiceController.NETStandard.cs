#if !NETFRAMEWORK

using System.IO;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos
{
  [ApiAuthorize]
  public class AsyncMessageServiceController<TService, TRequest, TResponse> : MessageControllerBase<TResponse>
    where TService : IAsyncMessageService<TRequest, TResponse>, new()
  {
    [HttpPost]
    public async Task<ActionResult> Perform()
    {
      string content;

      using (var stream = Request.Body)
      using (var reader = new StreamReader(stream))
      {
        content = await reader.ReadToEndAsync();
      }

      // Deserialize request.

      TRequest message;

      using (var reader = new StringReader(content))
      using (var json = new JsonTextReader(reader))
      {
        var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
        message = serializer.Deserialize<TRequest>(json);
      }

      // Process request

      var user = HttpContext.GetAuthenticatedUser();
      var response = await new TService().Perform(user, message);

      return GetResponse(response);
    }
  }
}

#endif