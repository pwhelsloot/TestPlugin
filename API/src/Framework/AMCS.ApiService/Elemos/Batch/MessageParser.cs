using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class MessageParser
  {
    private readonly JsonMediaTypeFormatter serializer = new JsonMediaTypeFormatter();

    public object ParseRequest(Type requestType, JObject data)
    {
      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
        using (var json = new JsonTextWriter(writer))
        {
          data.WriteTo(json);
        }

        stream.Position = 0;

        return serializer.ReadFromStream(requestType, stream, Encoding.UTF8, null);
      }
    }

    public void WriteResponse(JsonWriter json, Type responseType, object obj)
    {
      if (obj is ContentResponse content)
        WriteContentResponse(json, content);
      else if (obj is StreamResponse stream)
        WriteStreamResponse(json, stream);
      else
        WriteGenericResponse(json, responseType, obj);
    }

    private void WriteContentResponse(JsonWriter json, ContentResponse content)
    {
      json.WriteStartObject();

      json.WritePropertyName("content");
      json.WriteValue(content.Content);

      json.WritePropertyName("contentType");
      json.WriteValue(content.ContentType ?? "text/plain");

      json.WriteEndObject();
    }

    private void WriteStreamResponse(JsonWriter json, StreamResponse stream)
    {
      byte[] content;

      using (stream.Stream)
      using (var memoryStream = new MemoryStream())
      {
        stream.Stream.CopyTo(memoryStream);

        content = memoryStream.ToArray();
      }

      json.WriteStartObject();

      json.WritePropertyName("content");
      json.WriteValue(content);

      json.WritePropertyName("contentType");
      json.WriteValue(stream.ContentType ?? "application/octet-stream");

      if (stream.ContentDisposition != null)
      {
        json.WritePropertyName("contentDisposition");
        json.WriteValue(stream.ContentDisposition);
      }

      json.WriteEndObject();
    }

    private void WriteGenericResponse(JsonWriter json, Type responseType, object obj)
    {
      JToken response;

      using (var stream = new MemoryStream())
      {
        serializer.WriteToStream(responseType, obj, stream, Encoding.UTF8);

        stream.Position = 0;

        using (var reader = new StreamReader(stream))
        using (var jsonReader = new JsonTextReader(reader))
        {
          response = JToken.Load(jsonReader);
        }
      }

      response.WriteTo(json);
    }
  }
}
