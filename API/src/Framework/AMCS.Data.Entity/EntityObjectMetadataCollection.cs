using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.Entity
{
  public class EntityObjectMetadataCollection : IEnumerable<EntityObjectMetadata>
  {
    private readonly List<EntityObjectMetadata> list;
    private readonly Dictionary<Type, EntityObjectMetadata> typeMap;
    private readonly Dictionary<string, EntityObjectMetadata> typeNameMap;

    public EntityObjectMetadata this[int index] => list[index];
    public EntityObjectMetadata this[Type entityType] => typeMap[entityType];
    public EntityObjectMetadata this[string entityType] => typeNameMap[entityType];

    public int Count => list.Count;

    internal EntityObjectMetadataCollection(List<EntityObjectMetadata> list)
    {
      this.list = list;
      typeMap = list.ToDictionary(p => p.Type, p => p);
      typeNameMap = list.ToDictionary(p => p.Type.FullName, p => p);
    }

    public IEnumerator<EntityObjectMetadata> GetEnumerator()
    {
      return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public EntityObjectMetadata FindByType(Type entityType)
    {
      typeMap.TryGetValue(entityType, out var metadata);
      return metadata;
    }

    public EntityObjectMetadata FindByTypeName(string typeName)
    {
      typeNameMap.TryGetValue(typeName, out var metadata);
      return metadata;
    }
  }
}
