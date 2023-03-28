using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterInExpression : IFilterExpression
  {
    public FilterExpressionKind Kind => FilterExpressionKind.In;

    public string Identifier { get; }

    public IList<FilterValue> Values { get; }

    public FilterInExpression(string identifier, IList<FilterValue> values)
    {
      Identifier = identifier;
      Values = values;
    }

    public void Accept(IFilterExpressionVisitor visitor)
    {
      visitor.VisitInExpression(this);
    }

    public T Accept<T>(IFilterExpressionVisitor<T> visitor)
    {
      return visitor.VisitInExpression(this);
    }
  }
}
