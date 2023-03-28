using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AMCS.Data.Entity
{
  [DebuggerDisplay("TableName = {TableName}, Type = {Type}")]
  public class EntityObjectMetadata
  {
    private static readonly HashSet<Type> SupportedChildReferenceTypes = new HashSet<Type>(new Type[] { typeof(IList<>), typeof(List<>), typeof(ISet<>), typeof(HashSet<>) });

    private readonly Dictionary<string, EntityObjectColumn> propertyMap = new Dictionary<string, EntityObjectColumn>();
    private readonly Dictionary<string, EntityObjectColumn> columnMap = new Dictionary<string, EntityObjectColumn>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, IEntityObjectReferenceMetadata> referenceByPropertyMap;

    public Type Type { get; }

    public string TableName { get; }

    public string SchemaName { get; }

    public string TableNameWithSchema { get; }

    public string KeyName { get; }

    public string NiceName { get; }

    public IList<EntityObjectColumn> Columns { get; }

    internal EntityObjectMetadata(Type type)
    {
      var entity = (EntityObject)Activator.CreateInstance(type);

      Type = type;
      TableName = entity.GetTableName();
      SchemaName = entity.GetSchemaName();
      TableNameWithSchema = entity.GetTableNameWithSchema();
      KeyName = entity.GetKeyName();
      NiceName = entity.GetNiceName();

      var columns = GetColumns();

      Columns = new ReadOnlyCollection<EntityObjectColumn>(columns);

      foreach (var column in columns)
      {
        if (propertyMap.TryGetValue(column.PropertyName, out var duplicateProp))
        {
          throw new ArgumentException($"Type {type.FullName} has a column with ColumnName='{column.ColumnName}' that references duplicate property name '{column.PropertyName}' same as column with ColumnName='{duplicateProp.ColumnName}'");
        }
        propertyMap.Add(column.PropertyName, column);

        if (column.ColumnName != null)
        {
          if (columnMap.TryGetValue(column.ColumnName, out var duplicateColumn))
          {
            throw new ArgumentException($"Type {type.FullName} has columns with duplicate ColumnName='{column.ColumnName}', property names: '{column.PropertyName}' && '{duplicateColumn.PropertyName}'");
          }
          columnMap.Add(column.ColumnName, column);
        }
      }

      referenceByPropertyMap = GetReferences();
    }

    public EntityObjectColumn FindByColumnName(string columnName)
    {
      columnMap.TryGetValue(columnName, out var column);
      return column;
    }

    public EntityObjectColumn FindByPropertyName(string propertyName)
    {
      propertyMap.TryGetValue(propertyName, out var column);
      return column;
    }

    internal IEntityObjectReferenceMetadata FindByReferencePropertyName(string propertyName)
    {
      referenceByPropertyMap.TryGetValue(propertyName, out var reference);
      return reference;
    }

    private List<EntityObjectColumn> GetColumns()
    {
      if (string.IsNullOrEmpty(TableName))
        return new List<EntityObjectColumn>();

      var ignoreInheritedPropertiesAttribute = (DataOperationIgnoreInheritedProperties)Type.GetCustomAttribute(typeof(DataOperationIgnoreInheritedProperties), false);
      bool ignoreInheritedProperties = ignoreInheritedPropertiesAttribute != null;

      var columns = new List<EntityObjectColumn>();

      foreach (var property in Type.GetProperties())
      {
        bool canRead = true;
        bool canWrite = true;

        if (ignoreInheritedProperties && property.DeclaringType != Type)
        {
          if (!ignoreInheritedPropertiesAttribute.Except.Contains(property.Name))
          {
            if (ignoreInheritedPropertiesAttribute.IgnoreForDataOperationType.HasFlag(DataOperationIgnoreInheritedProperties.DataOperationType.Read))
              canRead = false;
            if (ignoreInheritedPropertiesAttribute.IgnoreForDataOperationType.HasFlag(DataOperationIgnoreInheritedProperties.DataOperationType.Write))
              canWrite = false;
          }
        }

        if (EntityMetadataReader.IsMember(property))
        {
          var dateStorage = EntityMetadataReader.GetDateStorage(property);
          string timeZoneMember = EntityMetadataReader.GetTimeZoneMember(property);

          if (dateStorage == DateStorage.Zoned && timeZoneMember == null)
            throw new DateConversionException("Zoned date storage requires a time zone member");
          if (dateStorage != DateStorage.Zoned && timeZoneMember != null)
            throw new DateConversionException("Non zoned date storage cannot have a time zone member");

          IDateStorageConverter dateStorageConverter = null;
          if (dateStorage != DateStorage.None)
            dateStorageConverter = DateStorageConverterFactory.CreateConverter(property.PropertyType, dateStorage);

          columns.Add(new EntityObjectColumn(
            property.Name,
            EntityMetadataReader.GetMemberColumnName(property),
            canRead,
            canWrite,
            dateStorage,
            dateStorageConverter,
            timeZoneMember));
        }
      }

      return columns;
    }

    private Dictionary<string, IEntityObjectReferenceMetadata> GetReferences()
    {
      var references = new Dictionary<string, IEntityObjectReferenceMetadata>();

      if (!typeof(EntityObject).IsAssignableFrom(Type))
        return references;

      foreach (var property in Type.GetProperties())
      {
        var referenceMetadata = GetParentReferenceMetadata(property)
          ?? GetChildReferenceMetadata(property);
        if (referenceMetadata != null)
        {
          references.Add(property.Name, referenceMetadata);
        }
      }

      return references;
    }

    private IEntityObjectReferenceMetadata GetParentReferenceMetadata(PropertyInfo property)
    {
      var attribute = property.GetCustomAttribute<EntityParentAttribute>();
      if (attribute != null)
      {
        if (!typeof(EntityObject).IsAssignableFrom(property.PropertyType))
          throw new ArgumentException($"Property {property.Name} on {Type} is marked as a parent reference, so it must be an {typeof(EntityObject).Name}");
        if (!property.CanWrite)
          throw new ArgumentException($"Property {property.Name} on {Type} is marked as a parent reference, so it cannot be readonly");
        return new EntityObjectReferenceMetadataParent(attribute);
      }
      return null;
    }

    private IEntityObjectReferenceMetadata GetChildReferenceMetadata(PropertyInfo property)
    { 
      var attribute = property.GetCustomAttribute<EntityChildAttribute>();
      if (attribute != null)
      {
        if (!IsLegalChildReferenceType(property.PropertyType))
        {
          string collectionTypes = string.Join(",", SupportedChildReferenceTypes.Select(type => type.Name));
          throw new ArgumentException($"Property {property.Name} on {Type} is marked as a child reference, so it must be a collection of "
            + $"{typeof(EntityObject).Name}, and the collection type must be one of the following types: {collectionTypes}");
        }
        return new EntityObjectReferenceMetadataChild(attribute);
      }
      return null;
    }

    private bool IsLegalChildReferenceType(Type type)
    {
      if (!type.IsGenericType)
        return false;
      if (type.GenericTypeArguments.Length != 1)
        return false;
      if (!typeof(EntityObject).IsAssignableFrom(type.GenericTypeArguments[0]))
        return false;
      if (!SupportedChildReferenceTypes.Contains(type.GetGenericTypeDefinition()))
        return false;
      return true;
    }
  }
}
