using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetNamedValueListConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      serializer.Serialize(writer, ((DataSetNamedValueList)value).Items);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      return new DataSetNamedValueList(serializer.Deserialize<List<DataSetNamedValue>>(reader));
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSetNamedValueList);
    }
  }
}
