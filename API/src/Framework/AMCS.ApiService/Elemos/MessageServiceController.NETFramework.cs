#if NETFRAMEWORK

using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AMCS.ApiService.Elemos
{
  [Authenticated]
  public class MessageServiceController<TService, TRequest, TResponse> : MessageControllerBase<TResponse>
    where TService : IMessageService<TRequest, TResponse>, new()
  {
    [HttpPost]
    public ActionResult Perform()
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
      var response = new TService().Perform(user, message);
      return GetResponse(response);
    }
  }
}

#endif
