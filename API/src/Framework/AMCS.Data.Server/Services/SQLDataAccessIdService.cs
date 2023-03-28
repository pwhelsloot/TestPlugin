using System;
using AMCS.Data.Entity;
using AMCS.Data.Server.SQL;

namespace AMCS.Data.Server.Services
{
  public class SQLDataAccessIdService : IDataAccessIdService
  {
    public Guid? GetGuidById(IDataSession session, Type entityType, int id)
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

    public int? GetIdByGuid(IDataSession session, Type entityType, Guid guid)
    {
      var accessor = EntityObjectAccessor.ForType(entityType);

      var tb = new SQLTextBuilder()
        .Text("SELECT ").Name(accessor.KeyName).Text(" ")
        .Text("FROM ").TableName(accessor).Text(" ")
        .Text("WHERE ").Name("GUID").Text(" = @Guid");

      return session.Query(tb.ToString())
        .Set("@Guid", guid)
        .Execute()
        .SingleOrDefaultScalar<int?>();
    }
  }
}
