using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public enum FilterExpressionKind
  {
    Binary,
    FromTo,
    In,
    Like
  }
}
