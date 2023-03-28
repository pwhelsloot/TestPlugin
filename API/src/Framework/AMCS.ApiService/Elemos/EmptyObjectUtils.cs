using System;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  internal static class EmptyObjectUtils
  {
    public static bool IsEmptyObject(object obj, EntityObjectAccessor accessor)
    {
      foreach (var property in accessor.Properties)
      {
        // Only count properties we actually serialize.

        if (property.Column?.CanWrite == true)
        {
          var value = property.GetValue(obj);
          if (value != null)
          {
            // If this property itself is a collapsible object, recurse into it.

            if (!property.ApiConfiguration.CollapseEmptyObject)
              return false;
            var propertyAccessor = EntityObjectAccessor.ForType(property.Type);
            if (!IsEmptyObject(value, propertyAccessor))
              return false;
          }
        }
      }

      return true;
    }

    public static object CreateEmptyObject(EntityObjectProperty property)
    {
      var obj = Activator.CreateInstance(property.Type);

      // Initialize all nested properties that also have collapsible set
      // with an empty object.

      foreach (var objProperty in EntityObjectAccessor.ForType(property.Type).Properties)
      {
        if (objProperty.ApiConfiguration.CollapseEmptyObject)
          objProperty.SetValue(obj, CreateEmptyObject(objProperty));
      }

      return obj;
    }
  }
}
