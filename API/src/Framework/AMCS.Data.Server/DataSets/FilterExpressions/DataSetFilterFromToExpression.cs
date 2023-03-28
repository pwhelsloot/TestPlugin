using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public class DataSetFilterFromToExpression : DataSetFilterExpression
  {
    public override DataSetFilterExpressionKind Kind => DataSetFilterExpressionKind.FromTo;

    public DataSetValue From { get; }

    public DataSetValue To { get; }

    public DataSetFilterFromToExpression(DataSetColumn column, DataSetValue from, DataSetValue to)
      : base(column)
    {
      From = from;
      To = to;
    }

    public override T Accept<T>(IDataSetFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitFromToExpression(this);
    }
  }
}
