using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetValue
  {
    public static DataSetValue Create(object value)
    {
      switch (value)
      {
        case null:
          return new DataSetValue(DataSetValueKind.Null, null);
        case int intValue:
          return new DataSetValue(DataSetValueKind.Integer, intValue);
        case long longValue:
          return new DataSetValue(DataSetValueKind.Integer, longValue);
        case double doubleValue:
          return new DataSetValue(DataSetValueKind.Decimal, doubleValue);
        case decimal decimalValue:
          return new DataSetValue(DataSetValueKind.Decimal, decimalValue);
        case bool boolValue:
          return new DataSetValue(DataSetValueKind.Boolean, boolValue);
        case string stringValue:
          return new DataSetValue(DataSetValueKind.String, stringValue);
        default:
          throw new InvalidOperationException($"Unsupported filter value type '{value.GetType()}'");
      }
    }

    public DataSetValueKind Kind { get; }

    public object Value { get; }

    public DataSetValue(DataSetValueKind kind, object value)
    {
      Kind = kind;
      Value = value;
    }
  }
}
