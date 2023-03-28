using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.BslTriggers;
using AMCS.Data.Server.SQL.Fetch;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.SQL
{
  public static class DataSessionExtensions
  {
    public static ISQLQuery StoredProcedure(this IDataSession dataSession, string name, FetchInfo fetchInfo = null)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (name == null)
        throw new ArgumentNullException(nameof(name));

      return new SQLStoredProcedureCommandBuilder((SQLDataSession)dataSession, fetchInfo, name);
    }

    public static ISQLInsert Insert(this IDataSession dataSession, ISessionToken userId, EntityObject entityObject)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (entityObject == null)
        throw new ArgumentNullException(nameof(entityObject));

      return new SQLInsertCommandBuilder((SQLDataSession)dataSession, userId, entityObject);
    }

    public static ISQLBulkInsert BulkInsert(this IDataSession dataSession, ISessionToken userId, IList<EntityObject> entityObjects)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (entityObjects == null)
        throw new ArgumentNullException(nameof(entityObjects));

      return new SQLBulkInsertBuilder((SQLDataSession)dataSession, userId, entityObjects);
    }

    public static ISQLUpdate Update(this IDataSession dataSession, ISessionToken userToken, EntityObject entityObject)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (entityObject == null)
        throw new ArgumentNullException(nameof(entityObject));

      return new SQLUpdateCommandBuilder((SQLDataSession)dataSession, userToken, entityObject);
    }

    public static ISQLDelete Delete(this IDataSession dataSession, ISessionToken userId, EntityObject entityObject)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (entityObject == null)
        throw new ArgumentNullException(nameof(entityObject));

      return new SQLDeleteCommandBuilder((SQLDataSession)dataSession, userId, entityObject);
    }

    public static ISQLQuery Query(this IDataSession dataSession, string sql, FetchInfo fetchInfo = null)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (sql == null)
        throw new ArgumentNullException(nameof(sql));

      return new SQLQueryCommandBuilder((SQLDataSession)dataSession, fetchInfo, sql);
    }

    public static ISQLQuery Query(this IDataSession dataSession, SQLQueryBuilder query, FetchInfo fetchInfo = null)
    {
      if (dataSession == null)
        throw new ArgumentNullException(nameof(dataSession));
      if (query == null)
        throw new ArgumentNullException(nameof(query));

      var result = dataSession.Query(query.ToString(), fetchInfo);

      foreach (var parameter in query.Parameters)
      {
        result.SetObject(parameter.Name, parameter.Value);
      }

      return result;
    }
  }
}
