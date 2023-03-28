using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class Filter
  {
    public IList<IFilterExpression> Expressions { get; }

    public Filter(IList<IFilterExpression> expressions)
    {
      Expressions = expressions;
    }
  }
}
