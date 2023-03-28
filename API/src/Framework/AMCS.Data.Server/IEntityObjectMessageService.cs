using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public interface IEntityObjectMessageService<in TEntity, in TRequest, out TResponse>
    where TEntity : EntityObject
  {
    TResponse Perform(ISessionToken userId, TEntity entity, TRequest request, IDataSession dataSession);
  }
}
