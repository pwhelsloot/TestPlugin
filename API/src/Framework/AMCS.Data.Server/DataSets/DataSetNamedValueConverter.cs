using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetNamedValueConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var namedValue = (DataSetNamedValue)value;

      writer.WriteStartObject();
      writer.WritePropertyName("Name");
      writer.WriteValue(namedValue.Name);
      writer.WritePropertyName("Value");
      writer.WriteValue(namedValue.Value.Value);
      writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSetNamedValue);
    }
  }
}
