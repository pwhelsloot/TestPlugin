using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetValueListConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      serializer.Serialize(writer, ((DataSetValueList)value).Items);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      return new DataSetValueList(serializer.Deserialize<List<DataSetValue>>(reader));
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSetValueList);
    }
  }
}