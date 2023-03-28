namespace AMCS.Data.Server
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Entity;
  using AMCS.Data.Schema.Discovery.Wcf.Service;
  using AMCS.Data.Server.SQL;

  public static class IsDeletedPropertyChecker
  {
    public static Tuple<IList<string>, IList<string>> GetEntitiesWithoutIsDeletedProp(string schemaName)
    {
      var entities = new List<string>();
      var entitiesWithAccessClass = new List<string>();
      // Getting All Registered Entities
      var allEntities = DataServices.Resolve<EntityObjectManager>().Entities;

      // Getting all the tables and filter out tables which are having IsDeleted column
      var allTables = new SchemaDiscoveryService().DiscoverTables(schemaName);
      var tablesWithIsDeletedColumn = new HashSet<string>(allTables.Where(t => t.Columns.Any(c => c.Name == "IsDeleted")).Select(a => a.Name), StringComparer.OrdinalIgnoreCase);

      foreach (var entity in allEntities)
      {
        if (entity.Type.CanConstruct())
        {
          if (string.IsNullOrEmpty(entity.KeyName))
            continue;

          if (string.IsNullOrEmpty(entity.TableName) || string.Equals(entity.TableName, "NA", StringComparison.OrdinalIgnoreCase))
            continue;

          if (tablesWithIsDeletedColumn.Contains(entity.TableNameWithSchema))
          {
            if (entity.FindByPropertyName("IsDeleted") == null)
            {
              var access = DataAccessManager.GetAccessForEntity(entity.Type);
              // Separating the entities which are having custom SQLEntityObjectAccess implementation.
              if (access.GetType().Name != typeof(SQLEntityObjectAccess<>).Name)
              {
                entitiesWithAccessClass.Add(entity.Type.FullName);
              }
              else
              {
                entities.Add(entity.Type.FullName);
              }
            }
          }
        }
      }

      return new Tuple<IList<string>, IList<string>>(entities, entitiesWithAccessClass);
    }
  }
}
