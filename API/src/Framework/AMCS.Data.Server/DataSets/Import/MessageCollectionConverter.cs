using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class MessageCollectionConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteStartArray();

      foreach (var message in (MessageCollection)value)
      {
        writer.WriteStartObject();

        writer.WritePropertyName("Type");
        writer.WriteValue(message.Type.ToString());

        writer.WritePropertyName("Description");
        writer.WriteValue(message.Description);

        if (message.DataSet != null)
        {
          writer.WritePropertyName("DataSet");
          writer.WriteValue(message.DataSet.Name);
        }

        if (message.Record != null)
        {
          writer.WritePropertyName("Record");

          writer.WriteStartObject();

          writer.WritePropertyName("Id");
          writer.WriteValue(message.Record.GetId());

          var referenceKey = message.Record.GetReferenceKey();
          if (referenceKey.HasValue)
          {
            writer.WritePropertyName("ReferenceKey");
            writer.WriteValue(referenceKey.Value);
          }

          writer.WriteEndObject();
        }

        if (message.Exception != null)
        {
          writer.WritePropertyName("Exception");
          serializer.Serialize(writer, message.Exception);
        }

        writer.WriteEndObject();
      }

      writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(MessageCollection);
    }
  }
}
