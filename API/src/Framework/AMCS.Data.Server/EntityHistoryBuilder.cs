using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Server.SQL;
using AMCS.Data.Support;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace AMCS.Data.Server
{
  public class EntityHistoryBuilder
  {

    private readonly EntityObject entityObject;
    private readonly EntityObjectAccessor accessor;

    private EntityHistoryChanges changes = new EntityHistoryChanges();
    private List<EntityHistoryMetadata> metadata = new List<EntityHistoryMetadata>();
    private Type parentEntityType = null;
    private int? parentEntityId = null;
    private DataUpdateKind kind { get; set; }
    private bool forceWriteWhenNoChanges { get; set; } = false;
    public EntityHistoryBuilder(EntityObject entityObject, DataUpdateKind kind)
    {
      this.entityObject = entityObject;
      this.accessor = EntityObjectAccessor.ForType(entityObject.GetType());
      this.kind = kind;

      if (!accessor.SupportsTracking)
        throw new InvalidOperationException($"Entity of type {entityObject.GetType()} does not have entity history enabled");
    }

    #region Access properties
    public void SetKind(DataUpdateKind kind)
    {
      this.kind = kind;
    }
    #endregion

    public void ForceWriteWhenNoChanges(bool force = true)
    {
      this.forceWriteWhenNoChanges = force;
    }

    #region Get changes
    public string GetChangesForUpdate()
    {
      // First gather the internal changes
      AddUpdateChanges();

      // Allow Object Services to have their say
      AddObjectServiceChanges();

      // Return result
      return SerializeResult();
    }

    public string GetChangesForInsert()
    {
      // Gather changes
      AddInsertChanges();

      // Allow Object Services to have their say
      AddObjectServiceChanges();

      // Return result
      return SerializeResult();
    }

    public string GetChangesForDelete()
    {
      // Gather changes
      AddDeleteChanges();

      // Allow Object Services to have their say
      AddObjectServiceChanges();

      // Return result
      return SerializeResult();
    }
    #endregion

    #region Manage changes
    private void AddChange(EntityHistoryChange change)
    {
      AddChange(change.PropertyKey, change.OldValue, change.NewValue);
    }

    private void AddChange(string propertyName, object oldValue, object newValue)
    {
      changes[propertyName] = new object[] { oldValue, newValue };
    }

    public void AddCustomChange(string propertyName, object oldValue, object newValue)
    {
      AddChange(propertyName, oldValue, newValue);
      metadata.Add(new EntityHistoryMetadata
      {
        PropertyName = propertyName,
        OldValueDataType = GetMetadata(oldValue),
        NewValueDataType = GetMetadata(newValue)
      });
    }

    private string GetMetadata(object value)
    {
      string metadata = "";
      Type type = value.GetType();

      if (type.GetInterface("ICollection") != null)
      {
        metadata = "ICollection:";
        type = type.GetGenericArguments()[0];
      }
      metadata += type.Name;

      return metadata;
    }

    public bool HasChanges()
    {
      return changes.Count > 0;
    }
    #endregion

    #region Build result
    private string SerializeResult()
    {
      if (changes.Count > 0 || this.forceWriteWhenNoChanges)
      {
        // For optimal performance manually create the json
        using (var sw = new StringWriter())
        using (var writer = new JsonTextWriter(sw))
        {
          writer.WriteStartObject();
          foreach (var propertyName in changes.Keys)
          {
            writer.WritePropertyName(propertyName);
            writer.WriteStartArray();
            WriteValue(writer, changes[propertyName][0]);
            WriteValue(writer, changes[propertyName][1]);
            writer.WriteEndArray();
          }
          writer.WriteEndObject();

          // Prepend "2:" to indicate the version of the serialization
          return $"2:{sw}";
        }
      }

      return null;
    }

    private void WriteValue(JsonTextWriter writer, object value)
    {
      if (value != null)
      {
        if (value.GetType().GetInterface("ICollection") != null)
        {
          value = JsonConvert.SerializeObject(value);
        }
        else
        {
          Type type = value.GetType();
          if (
            type == typeof(Guid) ||
            type == typeof(NodaTime.ZonedDateTime) ||
            type == typeof(NodaTime.OffsetDateTime) ||
            type == typeof(NodaTime.LocalDate) ||
            type == typeof(NodaTime.LocalTime)
          )
          {
            value = JsonUtil.Print(value);
          }
          else if (
            type == typeof(GeographyPoint) ||
            type == typeof(GeographyPolygon)
          )
          {
            value = JsonConvert.SerializeObject(value);
          }
        }
      }
      writer.WriteValue(value);
    }

    private void AddUpdateChanges()
    {
      var loadedValues = entityObject.GetLoadedPropertyValues();

      if (loadedValues == null)
      {
        loadedValues = new object[accessor.Properties.Count];
      }

      foreach (var property in accessor.Properties)
      {
        if (property.Column?.CanWrite != true)
          continue;

        // ignore dynamic properties that aren't overriden
        if (property.IsDynamicColumn && !property.IsDynamicColumnOverridable)
          continue;

        // Ignore the key column.
        if (property.IsKey || property.Name == "Id")
          continue;

        // Ignore GUID column for all updates - should never be changed (following chat with Tony) - Seb
        if (property.Name == "GUID")
          continue;

        // got this far, so want to update this property in the database
        var propertyIndex = accessor.Properties.IndexOf(property);
        object newValue = property.GetValue(entityObject);
        object oldValue = loadedValues[propertyIndex];

        if (!Equals(oldValue, newValue) || this.forceWriteWhenNoChanges)
        {
          AddChange(property.Name, oldValue, newValue);
        }
      }
    }

    private void AddInsertChanges()
    {
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());

      foreach (var property in accessor.Properties)
      {
        if (property.Column?.CanWrite != true)
          continue;

        // ignore dynamic properties that aren't overriden
        if (property.IsDynamicColumn && !property.IsDynamicColumnOverridable)
          continue;

        // Ignore the key column.
        if (property.IsKey || property.Name == "Id")
          continue;

        // Ignore GUID column for all updates - should never be changed (following chat with Tony) - Seb
        if (property.Name == "GUID")
          continue;

        // got this far, so want to update this property in the database
        string propertyName = property.Name;
        object newValue = property.GetValue(entityObject);

        if (newValue != null)
        {
          AddChange(property.Name, null, newValue);
        }
      }
    }

    private void AddDeleteChanges()
    {
      var deletedProperty = accessor.GetProperty("IsDeleted");
      var hardDelete = !accessor.CanUndelete;

      if (deletedProperty != null)
      {
        bool isUndelete = (bool)deletedProperty.GetValue(entityObject);
        AddChange(deletedProperty.Name, isUndelete, !isUndelete);
      }
      else
      {
        AddChange("Id", entityObject.Id32, null);
      }
    }

    private void AddObjectServiceChanges()
    {
      var entityObjectService = BusinessServiceManager.GetService(entityObject.GetType());

      var adder = (EntityHistoryChangeAdder)Activator.CreateInstance(typeof(EntityHistoryChangeAdder<>).MakeGenericType(entityObject.GetType()));

      adder.AddEntityHistoryChanges(entityObject, this);
    }
    #endregion

    #region Parenting
    public void SetParent(Type parentEntityType, int parentEntityId)
    {
      this.parentEntityType = parentEntityType;
      this.parentEntityId = parentEntityId;
    }

    public string GetParentTable()
    {
      return GetParentTableName(this.parentEntityType);
    }

    public static string GetParentTableName(Type parentEntityType)
    {
      if (parentEntityType == null)
        return null;

      var accessor = EntityObjectAccessor.ForType(parentEntityType);

      return $"[{accessor.SchemaName}].[{accessor.TableName}]";
    }

    public int? GetParentTableId()
    {
      return this.parentEntityId;
    }
    #endregion

    public void ManualSave(IDataSession dataSession, ISessionToken sessionToken)
    {
      EntityHistoryTypeEnum type;
      string changes = "";

      switch (this.kind)
      {
        case DataUpdateKind.Insert:
          type = EntityHistoryTypeEnum.Insert;
          changes = this.GetChangesForInsert();
          break;

        case DataUpdateKind.Delete:
        case DataUpdateKind.Undelete:
          type = EntityHistoryTypeEnum.Delete;
          changes = this.GetChangesForDelete();
          break;

        default:
          type = EntityHistoryTypeEnum.Update;
          changes = this.GetChangesForUpdate();
          break;
      }
      
      if (changes != null || this.forceWriteWhenNoChanges)
      {
        string tableName = new SQLTextBuilder().TableName(entityObject.GetSchemaName(), entityObject.GetTableName()).ToString();
        SQLDataAccessHistory.InsertHistoryRecord(sessionToken, tableName, entityObject.GetKeyName(), entityObject.Id32, changes, type, this.GetParentTable(), this.GetParentTableId(), DateTime.Now, dataSession);
        UpdateLoadedValues();
      }
    }

    public void UpdateLoadedValues()
    {
      UpdateLoadedValues(entityObject);
    }

    public void ReadLoadedValuesFromDB(IDataSession dataSession, ISessionToken sessionToken)
    {
      var readFromEntity = DataAccessManager.GetAccessForEntity(entityObject.GetType()).GetById(dataSession, sessionToken, entityObject.Id32);
      UpdateLoadedValues(readFromEntity);
    }

    private void UpdateLoadedValues(EntityObject readFromEntity)
    {
      var accessor = EntityObjectAccessor.ForType(readFromEntity.GetType());
      object[] loadedPropertyValues = new object[accessor.Properties.Count];

      foreach (var property in accessor.Properties)
      {
        // got this far, so want to update this property in the database
        var propertyIndex = accessor.Properties.IndexOf(property);
        object fieldValue = property.GetValue(readFromEntity);
        if (fieldValue != null)
        {
          loadedPropertyValues[propertyIndex] = fieldValue;
        }
      }

      ((ILoadedPropertyValues)entityObject).SetLoadedPropertyValues(loadedPropertyValues);
    }
  }

  internal class EntityHistoryChanges
    : Dictionary<string, object[]>
  {
  }

  internal class EntityHistoryMetadata
  {
    public string PropertyName;
    public string OldValueDataType;
    public string NewValueDataType;
  }

  abstract class EntityHistoryChangeAdder
  {
    public abstract void AddEntityHistoryChanges(EntityObject entity, EntityHistoryBuilder builder);
  }

  class EntityHistoryChangeAdder<T> : EntityHistoryChangeAdder
    where T : EntityObject
  {
    public override void AddEntityHistoryChanges(EntityObject entity, EntityHistoryBuilder builder)
    {
      if (DataServices.Resolve <IEntityObjectService<T>>() is IEntityObjectHistoryChangeService<T> historyChangeService)
      {
        historyChangeService.AddEntityHistoryChanges((T)entity, builder);
      }
    }
  }

}
