using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity.Interfaces;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;

namespace AMCS.PlatformFramework.Server.Services
{
  public class RestrictionService : IRestrictionService
  {
    public void ClearCachedEntityRestrictions(ISessionToken userId, int accessGroupId, IDataSession dataSession)
    {
    }

    public IEntityRestrictionsEntity GetEntityRestriction(ISessionToken userId, Type entityType, IDataSession session)
    {
      return null;
    }
  }
}
