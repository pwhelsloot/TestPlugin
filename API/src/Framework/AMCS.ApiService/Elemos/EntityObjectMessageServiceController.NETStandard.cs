#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Support;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos
{
  [ApiAuthorize]
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
      string content;

      using (var stream = Request.Body)
      using (var reader = new StreamReader(stream))
      {
        content = Task.Run(() => reader.ReadToEndAsync()).ConfigureAwait(false).GetAwaiter().GetResult(); 
      }

      using (var reader = new StringReader(content))
      using (var json = new JsonTextReader(reader))
      {
        var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
        message = serializer.Deserialize<TRequest>(json);
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
