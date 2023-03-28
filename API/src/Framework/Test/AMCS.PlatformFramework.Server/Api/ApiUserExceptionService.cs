using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.Api
{
  [ServiceRoute("api/users/exception")]
  public class ApiUserExceptionService : IMessageService<ApiUserExceptionService.Request, ApiUserExceptionService.Response>
  {
    public Response Perform(ISessionToken userId, Request message)
    {
      throw BslUserExceptionFactory<BslUserException>.CreateCodeException("Some error", ErrorCode.SomeUserError);
    }

    public class Request
    {
    }

    public class Response
    {
    }
  }
}
