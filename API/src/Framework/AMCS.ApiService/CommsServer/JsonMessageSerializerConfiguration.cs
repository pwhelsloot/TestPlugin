using System;
using System.Collections.Generic;

namespace AMCS.ApiService.CommsServer
{
  public class JsonMessageSerializerConfiguration
  {
    public static JsonMessageSerializerConfiguration FromMessages(IEnumerable<Type> types)
    {
      var typesByName = new Dictionary<string, Type>();

      foreach (var type in types)
      {
        if (typesByName.ContainsKey(type.Name))
          throw new InvalidOperationException($"Message type '{type.Name}' appears multiple times");
        typesByName.Add(type.Name, type);
      }

      return new JsonMessageSerializerConfiguration(typesByName);
    }

    private readonly Dictionary<string, Type> typesByName;

    private JsonMessageSerializerConfiguration(Dictionary<string, Type> typesByName)
    {
      this.typesByName = typesByName;
    }

    public Type GetType(string type)
    {
      typesByName.TryGetValue(type, out var result);
      return result;
    }
  }
}
