using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.FilterExpressions;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Filters
{
  public abstract class DataSetFilter
  {
    public DataSetColumn Column { get; }

    protected DataSetFilter(DataSetColumn column)
    {
      Column = column;
    }

    internal void ToJson(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WriteStartObject();

      writer.WritePropertyName("Type");
      writer.WriteValue(GetTypeName());

      writer.WritePropertyName("Column");
      writer.WriteValue(Column.Property.Name);

      WriteJsonProperties(writer, serializer);

      writer.WriteEndObject();
    }

    private string GetTypeName()
    {
      return DataSetUtil.GetNameFromTypeName(GetType(), "DataSet", "Filter");
    }

    internal virtual void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
    }

    public abstract bool IsMatch(DataSetFilterExpression expression);
  }
}
