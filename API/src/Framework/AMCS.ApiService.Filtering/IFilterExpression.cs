using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public interface IFilterExpression
  {
    FilterExpressionKind Kind { get; }

    void Accept(IFilterExpressionVisitor visitor);

    T Accept<T>(IFilterExpressionVisitor<T> visitor);
  }
}
