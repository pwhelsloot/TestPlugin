using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetTableSetConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteStartObject();

      foreach (var table in ((DataSetTableSet)value).Tables)
      {
        writer.WritePropertyName(table.DataSet.Name);

        DataSetJsonUtil.WriteDataSetTable(writer, table);
      }

      writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var dataSetManager = DataServices.Resolve<IDataSetService>();
      var tableSet = new DataSetTableSet();

      JsonUtil.ExpectTokenType(reader, JsonToken.StartObject);

      while (true)
      {
        JsonUtil.Read(reader);
        if (reader.TokenType == JsonToken.EndObject)
          break;

        // Key is the data set name.

        JsonUtil.ExpectTokenType(reader, JsonToken.PropertyName);
        var dataSet = dataSetManager.GetDataSet((string)reader.Value);

        tableSet.Tables.Add(DataSetJsonUtil.ReadDataSetTable(reader, dataSet));
      }

      return tableSet;
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSetTableSet);
    }
  }
}
