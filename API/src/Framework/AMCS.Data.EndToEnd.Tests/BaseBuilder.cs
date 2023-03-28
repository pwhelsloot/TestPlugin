using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests
{
  public abstract class BaseBuilder
  {
    public abstract EntityObject Build(ISessionToken userId, IDataSession session);

    public static void ValidateEntityIsNotGenerated(EntityObject entity)
    {
      if (entity != null)
        throw new InvalidOperationException("The entity has already been generated");
    }

    public Func<ISessionToken, IDataSession, int> GetIntValue(EntityObject entity, int? value)
    {
      ValidateEntityIsNotGenerated(entity);

      if (value.HasValue)
      {
        return (userId, session) => value.Value;
      }
      else
      {
        return null;
      }
    }

    public int? GetBuildIntValue(Func<ISessionToken, IDataSession, int> value, ISessionToken userId, IDataSession session)
    {
      if (value != null)
      {
        return value(userId, session);
      }
      else
      {
        return null;
      }
    }

    public bool GetBuildBoolValue(bool? value)
    {
      if (value != null)
      {
        return value.Value;
      }
      else
      {
        return false;
      }
    }
  }
}
