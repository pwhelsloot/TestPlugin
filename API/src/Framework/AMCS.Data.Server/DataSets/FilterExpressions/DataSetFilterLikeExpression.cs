using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public class DataSetFilterLikeExpression : DataSetFilterExpression
  {
    public override DataSetFilterExpressionKind Kind => DataSetFilterExpressionKind.Like;

    public DataSetFilterLikeOperator Operator { get; }

    public string Value { get; }

    public DataSetFilterLikeExpression(DataSetColumn column, DataSetFilterLikeOperator @operator, string value)
      : base(column)
    {
      Operator = @operator;
      Value = value;
    }

    public override T Accept<T>(IDataSetFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitLikeExpression(this);
    }
  }
}
