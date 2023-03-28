using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Parser
{
  internal enum TokenKind
  {
    None,

    // Generic.

    Identifier,
    DecimalLiteral,
    FloatingPointLiteral,
    StringLiteral,
    CharacterLiteral,
    True,
    False,
    Null,

    // Syntax.

    Dot,
    ParenOpen,
    ParenClose,
    Comma,
    Plus,
    Minus,
    Equals,
    NotEquals,
    Greater,
    GreaterOrEquals,
    Less,
    LessOrEquals,
    LogicalNot,
    LogicalAnd,
    LogicalOr,
  }
}
