using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos
{
  internal class EntityObjectMetadataManager : IEntityObjectMetadataManager
  {
    private Dictionary<Type, IEntityObjectMetadata> entityMap;
    private Dictionary<string, IEntityObjectMetadata> collectionMap;
    private Dictionary<string, IOperation> operationMap;

    public void Initialize(IEnumerable<IEntityObjectMetadata> entities, IEnumerable<IOperation> operations)
    {
      entityMap = entities.ToDictionary(p => p.EntityType, p => p);
      collectionMap = entities.ToDictionary(p => p.CollectionName, p => p);
      operationMap = operations.ToDictionary(p => p.Name, p => p);
    }

    public IEntityObjectMetadata ForType(Type entityType)
    {
      var entities = entityMap;
      if (entities == null)
        throw new InvalidOperationException("Entity object metadata has not been initialized");

      if (!entities.TryGetValue(entityType, out var metadata))
        throw new ArgumentOutOfRangeException($"Unknown entity type {entityType}", nameof(entityType));

      return metadata;
    }

    public IEntityObjectMetadata ForCollection(string collectionName)
    {
      var collections = collectionMap;
      if (collections == null)
        throw new InvalidOperationException("Entity object metadata has not been initialized");

      if (!collections.TryGetValue(collectionName, out var metadata))
        throw new ArgumentOutOfRangeException($"Unknown entity type {collectionName}", nameof(collectionName));

      return metadata;
    }

    public IOperation FindOperation(string operationName)
    {
      var operations = operationMap;
      if (operations == null)
        throw new InvalidOperationException("Operation map has not been initialized");

      operations.TryGetValue(operationName, out var operation);
      return operation;
    }
  }
}
