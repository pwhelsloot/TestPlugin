using System;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;

namespace AMCS.ApiService.Elemos.SQL
{
  public class SQLApiDataAccessService : IApiDataAccessService
  {
    public Guid? GetGuidById(IDataSession session, Type entityType, int? id)
    {
      var accessor = EntityObjectAccessor.ForType(entityType);

      var tb = new SQLTextBuilder()
        .Text("SELECT ").Name("GUID").Text(" ")
        .Text("FROM ").TableName(accessor).Text(" ")
        .Text("WHERE ").Name(accessor.KeyName).Text(" = @Id");

      return session.Query(tb.ToString())
        .Set("@Id", id)
        .Execute()
        .SingleOrDefaultScalar<Guid?>();
    }
  }
}