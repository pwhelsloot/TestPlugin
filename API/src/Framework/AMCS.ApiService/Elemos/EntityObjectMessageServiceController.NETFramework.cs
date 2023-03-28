#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos
{
  [Authenticated]
  public class EntityObjectMessageServiceController<TService, TEntity, TRequest, TResponse> : MessageControllerBase<TResponse>
    where TService : IEntityObjectMessageService<TEntity, TRequest, TResponse>, new()
    where TEntity : EntityObject
  {
    [HttpPost]
    public ActionResult Perform(int id)
    {
      var user = HttpContext.GetAuthenticatedUser();

      ActionResult result;
      TRequest message;

      Request.InputStream.Position = 0;

      using (var streamReader = new StreamReader(Request.InputStream))
      using (var jsonTextReader = new JsonTextReader(streamReader))
      {
        JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
        message = serializer.Deserialize<TRequest>(jsonTextReader);
      }

      using (var dataSession = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = dataSession.CreateTransaction())
      {
        TEntity entity = null;

        if(id != 0)
           entity = dataSession.GetById<TEntity>(user, id);

        var response = new TService().Perform(user, entity, message, dataSession);

        result = GetResponse(response);

        transaction.Commit();
      }

      return result;
    }
  }
}

#endif
