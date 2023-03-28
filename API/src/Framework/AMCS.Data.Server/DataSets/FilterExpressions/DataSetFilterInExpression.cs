using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public class DataSetFilterInExpression : DataSetFilterExpression
  {
    public override DataSetFilterExpressionKind Kind => DataSetFilterExpressionKind.In;

    public DataSetValueList Values { get; }

    public DataSetFilterInExpression(DataSetColumn column, DataSetValueList values)
      : base(column)
    {
      Values = values;
    }

    public override T Accept<T>(IDataSetFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitInExpression(this);
    }
  }
}
