using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support.JsonDom
{
  public class JsonDomEntity : IJsonDomElement
  {
    private readonly List<(string PropertyName, IJsonDomElement Element)> items = new List<(string PropertyName, IJsonDomElement Element)>();
    private readonly EntityObject entity;

    public JsonDomEntity(EntityObject entity)
    {
      this.entity = entity;
    }

    public void Write(JsonWriter writer)
    {
      JsonDomEntityUtil.WriteEntity(writer, entity, items);
    }

    public void Add(string propertyName, IJsonDomElement element)
    {
      items.Add((propertyName, element));
    }
  }
}
