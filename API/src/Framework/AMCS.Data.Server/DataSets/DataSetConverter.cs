using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.Filters;
using AMCS.Data.Server.DataSets.Restrictions;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var dataSet = (DataSet)value;

      writer.WriteStartObject();

      writer.WritePropertyName("Name");
      writer.WriteValue(dataSet.Name);

      writer.WritePropertyName("Label");
      writer.WriteValue(dataSet.Label);

      writer.WritePropertyName("KeyColumn");
      writer.WriteValue(dataSet.KeyColumn.Property.Name);

      writer.WritePropertyName("DisplayColumn");
      writer.WriteValue(dataSet.DisplayColumn?.Property.Name);

      writer.WritePropertyName("Columns");
      WriteColumns(writer, dataSet.Columns);

      writer.WritePropertyName("Restrictions");
      WriteRestrictions(writer, dataSet.Restrictions, serializer);

      writer.WritePropertyName("Filters");
      WriteFilters(writer, dataSet.Filters, serializer);

      writer.WriteEndObject();
    }

    private void WriteColumns(JsonWriter writer, IList<DataSetColumn> columns)
    {
      writer.WriteStartArray();

      foreach (var column in columns)
      {
        writer.WriteStartObject();

        writer.WritePropertyName("Name");
        writer.WriteValue(column.Property.Name);

        writer.WritePropertyName("Label");
        writer.WriteValue(column.Label);

        writer.WritePropertyName("Type");
        var propertyType = Nullable.GetUnderlyingType(column.Property.Type) ?? column.Property.Type;
        writer.WriteValue(DataSetJsonUtil.GetTypeName(propertyType));

        writer.WritePropertyName("IsReadOnly");
        writer.WriteValue(column.IsReadOnly);

        writer.WritePropertyName("IsDefault");
        writer.WriteValue(column.IsDefault);

        writer.WritePropertyName("IsMandatory");
        writer.WriteValue(column.IsMandatory);

        writer.WriteEndObject();
      }

      writer.WriteEndArray();
    }

    private void WriteRestrictions(JsonWriter writer, IList<DataSetRestriction> restrictions, JsonSerializer serializer)
    {
      writer.WriteStartArray();

      foreach (var restriction in restrictions)
      {
        restriction.ToJson(writer, serializer);
      }

      writer.WriteEndArray();
    }

    private void WriteFilters(JsonWriter writer, IList<DataSetFilter> filters, JsonSerializer serializer)
    {
      writer.WriteStartArray();

      foreach (var filter in filters)
      {
        filter.ToJson(writer, serializer);
      }

      writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DataSet);
    }
  }
}
