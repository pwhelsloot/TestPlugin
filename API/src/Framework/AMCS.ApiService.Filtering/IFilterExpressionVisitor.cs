using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public interface IFilterExpressionVisitor
  {
    void VisitBinaryExpression(FilterBinaryExpression expression);

    void VisitFromToExpression(FilterFromToExpression expression);

    void VisitInExpression(FilterInExpression expression);

    void VisitLikeExpression(FilterLikeExpression expression);
  }

  public interface IFilterExpressionVisitor<out T>
  {
    T VisitBinaryExpression(FilterBinaryExpression expression);

    T VisitFromToExpression(FilterFromToExpression expression);

    T VisitInExpression(FilterInExpression expression);

    T VisitLikeExpression(FilterLikeExpression expression);
  }
}
