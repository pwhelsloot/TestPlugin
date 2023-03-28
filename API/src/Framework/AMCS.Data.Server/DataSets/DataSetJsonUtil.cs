using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Support;
using Newtonsoft.Json;
using NodaTime;

namespace AMCS.Data.Server.DataSets
{
  internal static class DataSetJsonUtil
  {
    private const string ReferenceKeyName = "$ReferenceKey";

    private static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>
    {
      { typeof(string), "string" },
      { typeof(bool), "boolean" },
      { typeof(int), "int" },
      { typeof(double), "decimal" },
      { typeof(decimal), "decimal" },
      { typeof(DateTime), "datetime" },
      { typeof(DateTimeOffset), "datetime-tz" },
      { typeof(ZonedDateTime), "datetime-tz" },
      { typeof(LocalDate), "date" },
      { typeof(LocalTime), "time" }
    };

    public static void WriteDataSetTable(JsonWriter writer, DataSetTable table)
    {
      writer.WriteStartArray();

      // First row is the list of columns.

      writer.WriteStartArray();
      foreach (var column in table.Columns)
      {
        writer.WriteValue(column.Property.Name);
      }
      writer.WriteValue(ReferenceKeyName);
      writer.WriteEndArray();

      // Remainder of the rows is the data.

      var propertyMap = GetPropertyMap(table.Columns);

      foreach (var row in table.Records)
      {
        writer.WriteStartArray();
        foreach (var property in propertyMap)
        {
          writer.WriteValue(JsonUtil.Print(property.GetValue(row)));
        }
        writer.WriteValue(row.GetReferenceKey());
        writer.WriteEndArray();
      }

      writer.WriteEndArray();
    }

    public static DataSetTable ReadDataSetTable(JsonReader reader, DataSet dataSet)
    {
      JsonUtil.ReadStartArray(reader);

      // First row is the list of columns.

      var columns = new List<DataSetColumn>();
      var dataSetColumns = new List<DataSetColumn>();

      JsonUtil.ReadStartArray(reader);

      while (true)
      {
        JsonUtil.Read(reader);
        if (reader.TokenType == JsonToken.EndArray)
          break;

        JsonUtil.ExpectTokenType(reader, JsonToken.String);
        var columnName = (string)reader.Value;
        if (columnName == ReferenceKeyName)
        {
          columns.Add(null);
        }
        else
        {
          var column = dataSet.GetColumn(columnName);

          columns.Add(column);
          dataSetColumns.Add(column);
        }
      }

      // Remainder of the rows is the data.

      var table = new DataSetTable(dataSet, dataSetColumns);
      var propertyMap = GetPropertyMap(columns);
      var typeMap = GetParseTypeMap(propertyMap);

      while (true)
      {
        JsonUtil.Read(reader);
        if (reader.TokenType == JsonToken.EndArray)
          break;

        JsonUtil.ExpectTokenType(reader, JsonToken.StartArray);

        var record = (IDataSetRecord)Activator.CreateInstance(dataSet.Type);
        table.Records.Add(record);

        for (var i = 0; i < propertyMap.Length; i++)
        {
          JsonUtil.Read(reader);
          JsonUtil.RejectTokenType(reader, JsonToken.EndArray);

          var value = reader.Value;

          var property = propertyMap[i];
          var propertyType = property == null ? typeof(Guid) : typeMap[i];

          if (value != null)
            value = JsonUtil.Parse(value, propertyType);

          if (property == null)
            record.SetReferenceKey((Guid?)value);
          else
            property.SetValue(record, value);
        }

        JsonUtil.ReadEndArray(reader);
      }

      return table;
    }

    private static Type[] GetParseTypeMap(EntityObjectProperty[] propertyMap)
    {
      var result = new Type[propertyMap.Length];

      for (int i = 0; i < propertyMap.Length; i++)
      {
        if (propertyMap[i] == null)
          continue;

        result[i] = Nullable.GetUnderlyingType(propertyMap[i].Type) ?? propertyMap[i].Type;
      }

      return result;
    }

    private static EntityObjectProperty[] GetPropertyMap(IList<DataSetColumn> columns)
    {
      var result = new EntityObjectProperty[columns.Count];

      for (int i = 0; i < columns.Count; i++)
      {
        result[i] = columns[i]?.Property;
      }

      return result;
    }

    public static string GetTypeName(Type type)
    {
      if (!TypeNames.TryGetValue(type, out string name))
        throw new NotSupportedException($"Type '{type}' is not a supported DTO value type");
      return name;
    }
  }
}
