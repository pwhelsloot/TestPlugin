using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public class DataSetFilterBinaryExpression : DataSetFilterExpression
  {
    public override DataSetFilterExpressionKind Kind => DataSetFilterExpressionKind.Binary;

    public DataSetFilterBinaryOperator Operator { get; }

    public DataSetValue Value { get; }

    public DataSetFilterBinaryExpression(DataSetColumn column, DataSetFilterBinaryOperator @operator, DataSetValue value)
      : base(column)
    {
      Operator = @operator;
      Value = value;
    }

    public override T Accept<T>(IDataSetFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitBinaryExpression(this);
    }
  }
}
