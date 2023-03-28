using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterFromToExpression : IFilterExpression
  {
    public FilterExpressionKind Kind => FilterExpressionKind.FromTo;

    public string Identifier { get; }

    public FilterValue From { get; }

    public FilterValue To { get; }

    public FilterFromToExpression(string identifier, FilterValue from, FilterValue to)
    {
      Identifier = identifier;
      From = from;
      To = to;
    }

    public void Accept(IFilterExpressionVisitor visitor)
    {
      visitor.VisitFromToExpression(this);
    }

    public T Accept<T>(IFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitFromToExpression(this);
    }
  }
}
