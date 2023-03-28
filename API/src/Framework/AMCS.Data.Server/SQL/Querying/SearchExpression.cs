using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  internal class SearchExpression : ISearchExpression
  {
    public string Search { get; }

    public SearchExpression(string search)
    {
      Search = search;
    }
  }
}