using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.FilterExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetQueryConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var dto = serializer.Deserialize<Dto>(reader);

      var dataSet = DataServices.Resolve<IDataSetService>().GetDataSet(dto.DataSet);

      var columns = new List<DataSetColumn>();

      foreach (string column in dto.Columns)
      {
        columns.Add(dataSet.GetColumn(column));
      }

      var expressions = new List<DataSetFilterExpression>();

      foreach (var expression in dto.Expressions)
      {
        expressions.Add(ParseExpression(expression, dataSet));
      }

      return new DataSetQuery(dataSet, columns, expressions);
    }

    private DataSetFilterExpression ParseExpression(JObject expression, DataSet dataSet)
    {
      var type = (string)expression["Type"];

      switch (type)
      {
        case "Binary":
          return ParseBinaryExpression(expression, dataSet);
        case "FromTo":
          return ParseFromToExpression(expression, dataSet);
        case "In":
          return ParseInExpression(expression, dataSet);
        case "Like":
          return ParseLikeExpression(expression, dataSet);
        default:
          throw new InvalidOperationException($"Unexpected expression type '{type}'");
      }
    }

    private DataSetFilterExpression ParseBinaryExpression(JObject expression, DataSet dataSet)
    {
      return new DataSetFilterBinaryExpression(
        dataSet.GetColumn((string)expression["Column"]),
        (DataSetFilterBinaryOperator)Enum.Parse(typeof(DataSetFilterBinaryOperator), (string)expression["Operator"]),
        ParseFilterValue(expression["Value"]));
    }

    private DataSetFilterExpression ParseFromToExpression(JObject expression, DataSet dataSet)
    {
      return new DataSetFilterFromToExpression(
        dataSet.GetColumn((string)expression["Column"]),
        ParseFilterValue(expression["From"]),
        ParseFilterValue(expression["To"]));
    }

    private DataSetFilterExpression ParseInExpression(JObject expression, DataSet dataSet)
    {
      var values = new List<DataSetValue>();

      foreach (var value in (JArray)expression["Values"])
      {
        values.Add(ParseFilterValue(value));
      }

      return new DataSetFilterInExpression(
        dataSet.GetColumn((string)expression["Column"]),
        new DataSetValueList(values));
    }

    private DataSetFilterExpression ParseLikeExpression(JObject expression, DataSet dataSet)
    {
      return new DataSetFilterLikeExpression(
        dataSet.GetColumn((string)expression["Column"]),
        (DataSetFilterLikeOperator)Enum.Parse(typeof(DataSetFilterLikeOperator), (string)expression["Operator"]),
        (string)expression["Value"]);
    }

    private DataSetValue ParseFilterValue(JToken token)
    {
      var value = ((JValue)token).Value;

      return DataSetValue.Create(value);
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSetQuery);
    }

    private class Dto
    {
      [JsonProperty(Required = Required.Always)]
      public string DataSet { get; set; }

      [JsonProperty(Required = Required.Always)]
      public List<string> Columns { get; } = new List<string>();

      public List<JObject> Expressions { get; } = new List<JObject>();
    }
  }
}
