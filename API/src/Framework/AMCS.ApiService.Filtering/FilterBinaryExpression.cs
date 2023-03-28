using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterBinaryExpression : IFilterExpression
  {
    public FilterExpressionKind Kind => FilterExpressionKind.Binary;

    public FilterBinaryOperator Operator { get; }

    public string Identifier { get; }

    public FilterValue Value { get; }

    public FilterBinaryExpression(FilterBinaryOperator @operator, string identifier, FilterValue value)
    {
      Operator = @operator;
      Identifier = identifier;
      Value = value;
    }

    public void Accept(IFilterExpressionVisitor visitor)
    {
      visitor.VisitBinaryExpression(this);
    }

    public T Accept<T>(IFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitBinaryExpression(this);
    }
  }
}
