using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.Data.Server.Util;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.Entity.Api;
using AMCS.PlatformFramework.Server.Api.Data.SQL;

namespace AMCS.PlatformFramework.Server.Api
{
  public class ApiUserService : EntityObjectMapperService<ApiUserEntity, UserEntity>, IEntityObjectChangesService
  {
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Rule firing on a false positive.")]
    private SQLApiUserAccess DataAccess { get; }

    public ApiUserService(IEntityObjectAccess<ApiUserEntity> dataAccess, IEntityObjectService<UserEntity> target, IEntityObjectMapper mapper)
      : base(target, mapper)
    {
      DataAccess = (SQLApiUserAccess)dataAccess;
    }

    public override int? Save(ISessionToken userId, ApiUserEntity entity, IDataSession existingDataSession = null)
    {
      UserEntity mapped;
      if (entity.Id32 > 0)
        mapped = existingDataSession.GetById<UserEntity>(userId, entity.Id32);
      else
        mapped = new UserEntity();

      Mapper.Map(entity, mapped);

      if (!string.IsNullOrEmpty(entity.Password))
        mapped.Password = PasswordHashing.Hash(entity.User, entity.Password);

      return Target.Save(userId, mapped);
    }

    public byte[] GetHighestRowVersion(ISessionToken userId, IDataSession existingDataSession = null)
    {
      return SQLApiUserAccess.GetHighestRowVersion(existingDataSession);
    }

    public IList<EntityObject> GetChanges(ISessionToken userId, IEntityObjectChangesFilter filter, IDataSession existingDataSession = null)
    {
      return SQLApiUserAccess.GetChanges(existingDataSession, filter);
    }
  }
}
