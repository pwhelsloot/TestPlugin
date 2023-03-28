#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Support;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  public class EntityObjectServiceInternalController<TEntity> : EntityObjectServiceController<TEntity, int>
    where TEntity : EntityObject, new()
  {
    internal override EntityObjectReader<int> GetReader()
    {
      ValidateEntityRoleAccess(typeof(TEntity));
      return new EntityObjectInternalReader(typeof(TEntity), HttpContext.GetAuthenticatedUser());
    }
  }
}

#endif
