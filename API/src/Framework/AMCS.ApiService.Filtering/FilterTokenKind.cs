using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  internal enum FilterTokenKind
  {
    None,

    // Generic.

    Identifier,
    QuotedIdentifier,
    Integer,
    Decimal,
    String,
    True,
    False,
    Null,

    // Syntax.

    ParenOpen,
    ParenClose,
    Comma,

    // Identifiers.

    Eq,
    Ne,
    Gt,
    Gte,
    Lt,
    Lte,
    StartsWith,
    EndsWith,
    Contains,
    In,
    From,
    To,
    And
  }
}
