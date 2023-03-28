using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support.JsonDom
{
  public class JsonDomEntityArray : IJsonDomElement
  {
    private readonly Dictionary<Guid, List<(string PropertyName, IJsonDomElement Element)>> items =
      new Dictionary<Guid, List<(string PropertyName, IJsonDomElement Element)>>();

    private readonly IList<EntityObject> entities;

    public JsonDomEntityArray(IList<EntityObject> entities)
    {
      this.entities = entities;
    }

    public void Add(Guid entity, (string PropertyName, IJsonDomElement Element) element)
    {
      if (items.TryGetValue(entity, out var existingList))
      {
        existingList.Add(element);
      }
      else
      {
        items.Add(entity, new List<(string PropertyName, IJsonDomElement Element)> { element });
      }
    }

    public void Write(JsonWriter writer)
    {
      JsonDomEntityUtil.WriteEntities(writer, entities, items);
    }
  }
}
