using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AMCS.Data.Entity.SQL;
using log4net;

namespace AMCS.Data.Entity
{
  [DebuggerDisplay("Entity = {Type}, Table = {TableName}")]
  public class EntityObjectAccessor
  {
    // Concurrent dictionary for primary access.
    private static readonly ConcurrentDictionary<Type, EntityObjectAccessor> Accessors = new ConcurrentDictionary<Type, EntityObjectAccessor>();
    // Synchronized dictionary to ensure no duplicates are built.
    private static readonly Dictionary<Type, EntityObjectAccessor> BuiltAccessors = new Dictionary<Type, EntityObjectAccessor>();
    private static readonly object SyncRoot = new object();

    private static readonly Func<Type, EntityObjectAccessor> BuildEntityObjectAccessorDelegate = BuildEntityObjectAccessor;
    private static readonly ILog Logger = LogManager.GetLogger(typeof(EntityObjectAccessor));

    public static EntityObjectAccessor ForType(Type entityType)
    {
      return Accessors.GetOrAdd(entityType, BuildEntityObjectAccessorDelegate);
    }

    private static EntityObjectAccessor BuildEntityObjectAccessor(Type entityType)
    {
      // ConcurrentDictionary`2.GetOrAdd does not synchronize the callbacks. However, we can only
      // ever instantiate an EntityObjectAccessor once because of the proxy types it create.
      // So, we synchronize the builders here.

      lock (SyncRoot)
      {
        if (!BuiltAccessors.TryGetValue(entityType, out var accessor))
        {
          using (SQLPerformanceLogger.Create(Logger, "BuildEntityObjectAccessor"))
          {
            var metadata = DataServices.Resolve<EntityObjectManager>().Entities.FindByType(entityType);

            accessor = new EntityObjectAccessor(entityType, metadata);
          }

          BuiltAccessors.Add(entityType, accessor);

          accessor.InitReferences();
        }

        return accessor;
      }
    }

    private readonly Dictionary<string, EntityObjectProperty> properties = new Dictionary<string, EntityObjectProperty>();
    private readonly Dictionary<string, EntityObjectProperty> propertiesByColumnName = new Dictionary<string, EntityObjectProperty>(StringComparer.OrdinalIgnoreCase);

    private Dictionary<string, IEntityObjectReference> references;

    public Type Type { get; }

    public string TableName { get; }

    public string SchemaName { get; }

    public string TableNameWithSchema { get; }

    public string KeyName { get; }

    public string NiceName { get; }

    public bool CanUndelete { get; }

    public IdentityInsertMode IdentityInsertMode { get; }

    public bool InsertOnNullId { get; }
    
    public bool HasBlobProperties { get; }

    public bool TrackInserts { get; }
    
    public bool TrackUpdates { get; }
    
    public bool TrackDeletes { get; }

    public IList<EntityObjectProperty> Properties { get; }

    public bool SupportsTracking => TrackInserts || TrackUpdates || TrackDeletes;

    internal EntityObjectAccessor(Type type, EntityObjectMetadata metadata)
    {
      // Types strictly don't have to be an EntityObject, at least not in DataRowToObjectConverter.
      // If we didn't get an EntityObject, we're not populating a bunch of data. However,
      // a lot still will be available and usable, e.g. the TypeAccessor access to the properties.

      if (!type.IsClass || type.IsAbstract || type.IsNested)
        throw new ArgumentException("System Error: The type " + type.FullName + " cannot be populated by the ORM");

      Type = type;

      if (Activator.CreateInstance(type) is EntityObject entity)
      {
        TableName = entity.GetTableName();
        SchemaName = entity.GetSchemaName();
        TableNameWithSchema = entity.GetTableNameWithSchema();
        KeyName = entity.GetKeyName();
        NiceName = entity.GetNiceName();
        TrackInserts = entity.GetTrackInserts();
        TrackUpdates = entity.GetTrackUpdates();
        TrackDeletes = entity.GetTrackDeletes();
      }

      Properties = BuildProperties(metadata);
      HasBlobProperties = Properties.Any(p => p.Type == typeof(EntityBlob));

      foreach (var property in Properties)
      {
        properties.Add(property.Name, property);
        if (property.Column != null)
          propertiesByColumnName.Add(property.Column.ColumnName, property);
      }

      CanUndelete = GetCanUndelete(metadata);
      IdentityInsertMode = EntityMetadataReader.GetIdentityInsertMode(type);
      InsertOnNullId = EntityMetadataReader.GetInsertOnNullId(type);
    }

    private bool GetCanUndelete(EntityObjectMetadata metadata)
    {
      var isDeletedProp = GetProperty("IsDeleted");

      if (isDeletedProp != null)
      {
        // if this property is dynamic column ignore as it's not been linked to a db field
        if (isDeletedProp.IsDynamicColumn)
        {
          isDeletedProp = null;
        }
        else
        {
          // ensure that this field should actually be used.  For example, it may have been inherited from a subclass and this type has said to ignore it
          // with the DataOperationIgnoreInheritedProperties attribute
          if (metadata != null && metadata.FindByColumnName("IsDeleted") == null)
            isDeletedProp = null;
        }
      }

      return isDeletedProp != null;
    }

    private IList<EntityObjectProperty> BuildProperties(EntityObjectMetadata metadata)
    {
      var properties = new List<EntityObjectProperty>();

      foreach (var property in Type.GetProperties())
      {
        var reference = metadata?.FindByReferencePropertyName(property.Name);
        var column = metadata?.FindByPropertyName(property.Name);

        properties.Add(
          new EntityObjectProperty(
            Type,
            property.Name,
            property.PropertyType,
            column,
            reference,
            property.Name == KeyName,
            EntityMetadataReader.IsMemberDynamic(property),
            EntityMetadataReader.IsMemberOverridable(property),
            EntityMetadataReader.GetMemberDynamicName(property),
            EntityMetadataReader.GetApiPropertyConfiguration(property)));
      }

      return new ReadOnlyCollection<EntityObjectProperty>(properties);
    }

    private void InitReferences()
    {
      Debug.Assert(Monitor.IsEntered(SyncRoot));

      this.references = properties.Values
        .Where(property => property.Reference != null)
        .ToDictionary(property => property.Name, property => property.Reference.CreateReference(property, this));
    }

    public EntityObjectProperty GetProperty(string propertyName)
    {
      properties.TryGetValue(propertyName, out var property);
      return property;
    }

    public EntityObjectProperty GetPropertyByColumnName(string columnName)
    {
      propertiesByColumnName.TryGetValue(columnName, out var property);
      return property;
    }

    public object CreateClone(object source)
    {
      return DataServices.Resolve<IEntityObjectMapper>().Map(source, source.GetType());
    }

    internal IEntityObjectReference GetReference(string propertyName)
    {
      references.TryGetValue(propertyName, out var reference);
      return reference;
    }

    internal void ResetReferences(EntityObject target)
    {
      foreach (var reference in references.Values)
      {
        reference.Reset(target);
      }
    }
  }
}
