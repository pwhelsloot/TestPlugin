﻿#if NETFRAMEWORK

using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Support;
using AMCS.Data.Entity;
using AMCS.Data.Server;

namespace AMCS.ApiService.Elemos
{
  [Authenticated]
  public class EntityObjectAsyncMessageServiceController<TService, TEntity, TRequest, TResponse> : MessageControllerBase<TResponse>
    where TService : IEntityObjectAsyncMessageService<TEntity, TRequest, TResponse>, new()
    where TEntity : EntityObject
  {
    [HttpPost]
    public async Task<ActionResult> Perform(int id, TRequest message)
    {
      var user = HttpContext.GetAuthenticatedUser();

      ActionResult result;

      using (var dataSession = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = dataSession.CreateTransaction())
      {
        var entity = dataSession.GetById<TEntity>(user, id);

        var response = await new TService().Perform(user, entity, message, dataSession);

        result = GetResponse(response);

        transaction.Commit();
      }

      return result;
    }
  }
}

#endif