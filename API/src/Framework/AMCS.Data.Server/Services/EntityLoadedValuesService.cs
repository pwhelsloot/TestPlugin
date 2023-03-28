using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.Services
{
  public class EntityLoadedValuesService : IEntityLoadedValuesService
  {
    public bool HasPropertyChanged(EntityObject entity, object[] loadedValues, string propertyName)
    {
      if (loadedValues == null)
        return true;

      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      Debug.Assert(accessor.Properties.Count == loadedValues.Length);

      var entityProperty = accessor.GetProperty(propertyName);
      var entityPropertyValue = entityProperty.GetValue(entity);
      var loadedPropertyValue = loadedValues[accessor.Properties.IndexOf(entityProperty)];

      return !Equals(entityPropertyValue, loadedPropertyValue);
    }

    public bool HasAnyPropertyChanged(EntityObject entity, object[] loadedValues)
    {
      if (loadedValues == null)
        return true;

      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      Debug.Assert(accessor.Properties.Count == loadedValues.Length);

      for (var i = 0; i < accessor.Properties.Count; i++)
      {
        var entityProperty = accessor.Properties[i];

        if (!entityProperty.IsDatabaseColumn || entityProperty.IsDynamicColumn)
          continue;

        if (!Equals(entityProperty.GetValue(entity), loadedValues[i]))
          return true;
      }

      return false;
    }
  }
}
