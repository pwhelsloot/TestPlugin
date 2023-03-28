using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetQueryResultConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var queryResult = (DataSetQueryResult)value;

      writer.WriteStartObject();

      writer.WritePropertyName("DataSet");
      writer.WriteValue(queryResult.Table.DataSet.Name);

      writer.WritePropertyName("Rows");
      DataSetJsonUtil.WriteDataSetTable(writer, queryResult.Table);

      writer.WritePropertyName("Cursor");
      serializer.Serialize(writer, queryResult.Cursor);

      writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSetQueryResult);
    }
  }
}
