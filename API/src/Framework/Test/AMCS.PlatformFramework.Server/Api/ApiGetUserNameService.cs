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
  [ServiceRoute("getUserName")]
  public class ApiGetUserNameService : IEntityObjectMessageService<UserEntity, ApiGetUserNameService.Request, ApiGetUserNameService.Response>
  {
    public Response Perform(ISessionToken userId, UserEntity entity, Request request, IDataSession dataSession)
    {
      return new Response { UserName = entity.UserName };
    }

    public class Request
    {
    }

    public class Response
    {
      public string UserName { get; set; }
    }
  }
}
