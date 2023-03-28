using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal enum BinaryExpressionType
  {
    Equals,
    Greater,
    GreaterOrEquals,
    Less,
    LessOrEquals,
    LogicalAnd,
    LogicalOr,
    NotEquals
  }
}
