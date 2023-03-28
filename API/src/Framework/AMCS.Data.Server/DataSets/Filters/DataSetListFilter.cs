using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.FilterExpressions;
using AMCS.Data.Support;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Filters
{
  public class DataSetListFilter : DataSetFilter
  {
    public DataSetNamedValueList List { get; }

    public DataSetListFilter(DataSetColumn column, DataSetNamedValueList list)
      : base(column)
    {
      List = list;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("List");
      serializer.Serialize(writer, List);
    }

    public override bool IsMatch(DataSetFilterExpression expression)
    {
      switch (expression)
      {
        case DataSetFilterBinaryExpression binaryExpression:
          if (binaryExpression.Operator != DataSetFilterBinaryOperator.Eq && binaryExpression.Operator != DataSetFilterBinaryOperator.Ne)
            return false;

          return
            binaryExpression.Value.Kind == DataSetValueKind.Null ||
            List.Find(binaryExpression.Value.Value) != null;

        case DataSetFilterInExpression inExpression:
          foreach (var value in inExpression.Values.Items)
          {
            if (List.Find(value.Value) == null)
              return false;
          }

          return true;

        default:
          return false;
      }
    }
  }
}
