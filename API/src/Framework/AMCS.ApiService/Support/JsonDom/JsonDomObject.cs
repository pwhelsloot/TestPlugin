using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support.JsonDom
{
  public class JsonDomObject : IJsonDomElement
  {
    private readonly List<(string PropertyName, IJsonDomElement Element)> items = new List<(string PropertyName, IJsonDomElement Element)>();

    public void Add(string propertyName, IJsonDomElement element)
    {
      items.Add((propertyName, element));
    }

    public void Write(JsonWriter writer)
    {
      writer.WriteStartObject();

      foreach (var item in items)
      {
        writer.WritePropertyName(item.PropertyName);
        if (item.Element == null)
          writer.WriteNull();
        else
          item.Element.Write(writer);
      }

      writer.WriteEndObject();
    }
  }
}
