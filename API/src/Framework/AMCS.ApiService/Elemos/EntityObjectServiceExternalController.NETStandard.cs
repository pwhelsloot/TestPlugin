#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Support;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  public class EntityObjectServiceExternalController<TEntity> : EntityObjectServiceController<TEntity, Guid>
    where TEntity : EntityObject, new()
  {
    internal override EntityObjectReader<Guid> GetReader()
    {
      ValidateEntityRoleAccess(typeof(TEntity));
      return new EntityObjectExternalReader(typeof(TEntity), HttpContext.GetAuthenticatedUser());
    }
  }
}

#endif
