using System;
using System.Collections.Generic;
using AMCS.Data.Configuration;

namespace AMCS.Data.Entity
{
  public class EntityObjectManager
  {
    public EntityObjectMetadataCollection Entities { get; }

    public EntityObjectManager(ITypeManager entityTypes)
    {
      var entities = new List<EntityObjectMetadata>();

      foreach (var type in entityTypes.GetTypes())
      {
        if (!type.IsAbstract && typeof(EntityObject).IsAssignableFrom(type))
        {
          try
          {
            entities.Add(new EntityObjectMetadata(type));
          }
          catch (ArgumentException ex)
          {
            throw new ArgumentException($"EntityObjectManager failed to add EntityObjectMetadata for type {type.FullName}", ex);
          }
        }
      }

      Entities = new EntityObjectMetadataCollection(entities);
    }
  }
}
