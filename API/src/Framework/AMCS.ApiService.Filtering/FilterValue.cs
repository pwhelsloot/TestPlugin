using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterValue
  {
    public FilterValueKind Kind { get; }
    
    public object Value { get; }

    public FilterValue(FilterValueKind kind, object value)
    {
      Kind = kind;
      Value = value;
    }
  }
}
