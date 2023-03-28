using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support.JsonDom
{
  public class JsonDomArray : IJsonDomElement
  {
    private readonly List<IJsonDomElement> items = new List<IJsonDomElement>();

    public void Add(IJsonDomElement element)
    {
      items.Add(element);
    }

    public void Write(JsonWriter writer)
    {
      writer.WriteStartArray();

      foreach (var item in items)
      {
        if (item == null)
          writer.WriteNull();
        else
          item.Write(writer);
      }

      writer.WriteEndArray();
    }
  }
}
