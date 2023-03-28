using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public abstract class DataSetFilterExpression
  {
    public DataSetColumn Column { get; }

    public abstract DataSetFilterExpressionKind Kind { get; }

    protected DataSetFilterExpression(DataSetColumn column)
    {
      Column = column;
    }

    public abstract T Accept<T>(IDataSetFilterExpressionVisitor<T> visitor);
  }
}
