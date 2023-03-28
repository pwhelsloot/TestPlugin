using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AMCS.ApiService.CommsServer
{
  public class JsonMessageSerializer
  {
    private readonly JsonMessageSerializerConfiguration configuration;
    private readonly JsonSerializer serializer = new JsonSerializer();

    public JsonMessageSerializer(JsonMessageSerializerConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public void Serialize<T>(IEnumerable<T> messages, Stream stream)
    {
      using (var writer = new StreamWriter(stream))
      using (var json = new JsonTextWriter(writer))
      {
        json.WriteStartArray();

        foreach (var message in messages)
        {
          json.WriteValue(message.GetType().Name);
          serializer.Serialize(json, message);
        }

        json.WriteEndArray();
      }
    }

    public List<T> Deserialize<T>(Stream stream)
    {
      var result = new List<T>();

      using (var reader = new StreamReader(stream))
      using (var json = new JsonTextReader(reader))
      {
        result.AddRange(DeserializeMessages<T>(json));
      }

      return result;
    }

    private IEnumerable<T> DeserializeMessages<T>(JsonTextReader json)
    {
      var serializer = new JsonSerializer();

      json.Read();
      if (json.TokenType != JsonToken.StartArray)
        throw new Exception();

      while (true)
      {
        json.Read();
        if (json.TokenType == JsonToken.EndArray)
          yield break;

        var messageType = (string)json.Value;
        var type = configuration.GetType(messageType);
        if (type == null)
          throw new InvalidOperationException($"Unknown message type '{messageType}'");

        json.Read();
        yield return (T)serializer.Deserialize(json, type);
      }
    }
  }
}
