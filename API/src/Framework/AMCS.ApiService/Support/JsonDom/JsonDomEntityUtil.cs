using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.ApiService.Elemos;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Support;
using Newtonsoft.Json;
using NodaTime;

namespace AMCS.ApiService.Support.JsonDom
{
  internal static class JsonDomEntityUtil
  {
    public static void WriteEntity(JsonWriter json, EntityObject entity, List<(string PropertyName, IJsonDomElement Element)> extraItems)
    {
      WriteEntity(json, entity, extraItems, EntityObjectAccessor.ForType(entity.GetType()));
    }

    private static void WriteEntity(JsonWriter json, EntityObject entity,
      List<(string PropertyName, IJsonDomElement Element)> extraItems, EntityObjectAccessor accessor)
    {
      json.WriteStartObject();

      foreach (var property in accessor.Properties)
      {
        if (property.Column?.CanWrite == true)
        {
          json.WritePropertyName(property.Column.ColumnName);

          var value = property.GetValue(entity);
          if (property.ApiConfiguration.CollapseEmptyObject)
          {
            var propertyAccessor = EntityObjectAccessor.ForType(property.Type);
            if (EmptyObjectUtils.IsEmptyObject(value, propertyAccessor))
              value = null;
          }

          WriteValue(json, value);
        }
      }

      foreach (var item in extraItems)
      {
        json.WritePropertyName(item.PropertyName);
        if (item.Element == null)
          json.WriteNull();
        else
          item.Element.Write(json);
      }

      json.WriteEndObject();
    }
    
    public static void WriteEntities(JsonWriter json, IEnumerable<EntityObject> entities, Dictionary<Guid, List<(string PropertyName, IJsonDomElement Element)>> extraItems)
    {
      json.WriteStartArray();

      if (entities != null)
      {
        EntityObjectAccessor accessor = null;

        foreach (var entity in entities)
        {
          if (accessor == null || accessor.GetType() != entity.GetType())
            accessor = EntityObjectAccessor.ForType(entity.GetType());

          if (entity.GUID.HasValue && extraItems.TryGetValue(entity.GUID.Value, out var items))
          {
            WriteEntity(json, entity, items, accessor);
          }
          else
          {
            WriteEntity(json, entity, new List<(string PropertyName, IJsonDomElement Element)>(), accessor);
          }
        }
      }

      json.WriteEndArray();
    }

    private static void WriteValue(JsonWriter json, object value)
    {
      if (value is EntityObject entity)
      {
        WriteEntity(json, entity, new List<(string PropertyName, IJsonDomElement Element)>());
      }
      else if (value is IEnumerable<EntityObject> entities)
      {
        WriteEntities(json, entities, new Dictionary<Guid, List<(string PropertyName, IJsonDomElement Element)>>());
      }
      else if (!(value is string || value is byte[]) && value is IEnumerable list)
      {
        json.WriteStartArray();

        foreach (var item in list)
        {
          WriteValue(json, item);
        }

        json.WriteEndArray();
      }
      else if (value is ZonedDateTime zonedDateTime)
      {
        JsonUtil.PrintZonedDateTime(json, zonedDateTime);
      }
      else
      {
        json.WriteValue(JsonUtil.Print(value));
      }
    }
  }
}