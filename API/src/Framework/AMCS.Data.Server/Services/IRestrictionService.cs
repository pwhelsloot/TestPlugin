using System;
using AMCS.Data.Entity.Interfaces;

namespace AMCS.Data.Server.Services
{
  public interface IRestrictionService
  {
    IEntityRestrictionsEntity GetEntityRestriction(ISessionToken userId, Type entityType, IDataSession session);
  }
}
