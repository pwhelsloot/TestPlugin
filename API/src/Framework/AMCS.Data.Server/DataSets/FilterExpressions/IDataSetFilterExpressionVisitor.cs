using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public interface IDataSetFilterExpressionVisitor<out T>
  {
    T VisitBinaryExpression(DataSetFilterBinaryExpression expression);

    T VisitFromToExpression(DataSetFilterFromToExpression expression);

    T VisitLikeExpression(DataSetFilterLikeExpression expression);

    T VisitInExpression(DataSetFilterInExpression expression);
  }
}
