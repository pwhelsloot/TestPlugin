using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public enum FieldComparison
  {
    Eq,
    Ne,
    Gt,
    Ge,
    Lt,
    Le,
    Null,
    NotNull,
    Like,
    In,
    InSQL
  }
}
