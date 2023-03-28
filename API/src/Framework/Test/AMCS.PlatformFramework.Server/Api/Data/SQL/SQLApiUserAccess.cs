using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;
using AMCS.PlatformFramework.Entity.Api;

namespace AMCS.PlatformFramework.Server.Api.Data.SQL
{
  public class SQLApiUserAccess : SQLEntityObjectAccess<ApiUserEntity>
  {
    public static byte[] GetHighestRowVersion(IDataSession dataSession)
    {
      return dataSession.Query("select max(RowVersion) from [User]")
        .Execute()
        .SingleOrDefaultScalar<byte[]>();
    }

    public static IList<EntityObject> GetChanges(IDataSession session, IEntityObjectChangesFilter filter)
    {
      var sb = new StringBuilder();

      sb
        .Append("SELECT TOP ").Append(filter.Count).Append(" * ")
        .Append("FROM [User] ")
        .Append("WHERE ");

      if (filter.IdStart.HasValue)
        sb.Append("[UserId] > @IdStart AND ");

      sb
        .Append("[RowVersion] > @RowVersionStart AND [RowVersion] <= @RowVersionEnd ")
        .Append("ORDER BY [UserId]");

      return session.Query(sb.ToString())
        .Set("@IdStart", filter.IdStart)
        .Set("@RowVersionStart", filter.RowVersionStart)
        .Set("@RowVersionEnd", filter.RowVersionEnd)
        .Execute()
        .List<EntityObject>(typeof(ApiUserEntity));
    }
  }
}
