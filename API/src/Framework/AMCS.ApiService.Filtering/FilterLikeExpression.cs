using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterLikeExpression : IFilterExpression
  {
    public FilterExpressionKind Kind => FilterExpressionKind.Like;

    public FilterLikeOperator Operator { get; }

    public string Identifier { get; }

    public string Value { get; }

    public FilterLikeExpression(FilterLikeOperator @operator, string identifier, string value)
    {
      Operator = @operator;
      Identifier = identifier;
      Value = value;
    }

    public void Accept(IFilterExpressionVisitor visitor)
    {
      visitor.VisitLikeExpression(this);
    }

    public T Accept<T>(IFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitLikeExpression(this);
    }
  }
}
