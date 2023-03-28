using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.Data.Server.SQL
{
  internal class SQLEntityHistoryHelper
  {
    public static string DetectChanges(ISessionToken userToken, EntityObject entityObject, bool updateOverridableDynamicColumns, IList<string> specialFields, bool ignoreSpecialFields, IList<string> restrictToFields, IDataSession dataSession)
    {
      var changes = new List<SQLEntityHistoryChange>();
      EntityObject oldVersion = DataAccessManager.GetAccessForEntity(entityObject.GetType()).GetById(dataSession, userToken, entityObject.Id32);
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());

      foreach (var property in accessor.Properties)
      {
        if (property.Column?.CanWrite != true)
          continue;

        // ignore dynamic properties that aren't overriden
        if (property.IsDynamicColumn && (!updateOverridableDynamicColumns || !property.IsDynamicColumnOverridable))
          continue;

        // Ignore the key column.
        if (property.IsKey || property.Name == "Id")
          continue;

        // Ignore GUID column for all updates - should never be changed (following chat with Tony) - Seb
        if (property.Name == "GUID")
          continue;

        if (specialFields != null && specialFields.Count > 0)
        {
          bool thisIsSpecialField = specialFields.Contains(property.Name);

          if (ignoreSpecialFields && thisIsSpecialField)
            continue;

          // we don't want to ignore special fields (which means we want to add them) this field is not special so don't carry on with it
          if (!ignoreSpecialFields && !thisIsSpecialField)
            continue;
        }

        // got this far, so want to update this property in the database
        if ((restrictToFields == null) || restrictToFields.Contains(property.Name))
        {
          string propertyName = property.Name;
          object newValue = property.GetValue(entityObject);
          object oldValue = property.GetValue(oldVersion);

          if (!Equals(oldValue, newValue))
          {
            changes.Add(new SQLEntityHistoryChange
            {
              PropertyKey = propertyName,
              OldValue = oldValue,
              NewValue = newValue
            });
          }
        }
      }

      if (changes.Count > 0)
        return JsonConvert.SerializeObject(changes);

      return null;
    }
    
    public static string GetChangesForDelete(EntityObject entityObject, bool isUndelete)
    {      
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());
      var deletedProperty = accessor.GetProperty("IsDeleted");
      var changes = new List<SQLEntityHistoryChange>();

      if (deletedProperty != null)
      {  
        changes.Add(new SQLEntityHistoryChange
        {
          PropertyKey = deletedProperty.Name,
          OldValue = isUndelete,
          NewValue = !isUndelete
        });
      }

      return JsonConvert.SerializeObject(changes);
    }

    public static string InitialInsert(EntityObject entityObject, bool insertOverridableDynamicColumns, IList<string> restrictToFields)
    {
      var changes = new List<SQLEntityHistoryChange>();

      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());

      foreach (var property in accessor.Properties)
      {
        if (property.Column?.CanWrite != true)
          continue;

        // ignore dynamic properties that aren't overriden
        if (property.IsDynamicColumn && (!insertOverridableDynamicColumns || !property.IsDynamicColumnOverridable))
          continue;

        // Ignore the key column.
        if (property.IsKey || property.Name == "Id")
          continue;

        // Ignore GUID column for all updates - should never be changed (following chat with Tony) - Seb
        if (property.Name == "GUID")
          continue;

        // got this far, so want to update this property in the database
        if ((restrictToFields == null) || restrictToFields.Contains(property.Name))
        {
          string propertyName = property.Name;
          object newValue = property.GetValue(entityObject);

          if (newValue != null)
          {
            changes.Add(new SQLEntityHistoryChange
            {
              PropertyKey = propertyName,
              OldValue = null,
              NewValue = newValue
            });
          }
        }
      }

      if (changes.Count > 0)
        return JsonConvert.SerializeObject(changes);

      return null;
    }
  }
}
