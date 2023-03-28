using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public abstract class DataSetRestriction
  {
    public DataSetColumn Column { get; }

    protected DataSetRestriction(DataSetColumn column)
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
      return DataSetUtil.GetNameFromTypeName(GetType(), "DataSet", "Restriction");
    }

    internal virtual void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
    }

    public abstract void Accept(IDataSetRestrictionVisitor visitor);
  }
}
