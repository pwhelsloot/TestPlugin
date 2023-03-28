using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Parser
{
  internal struct Token
  {
    public TokenKind Kind { get; }

    public string Text { get; }

    public Token(TokenKind kind, string text)
    {
      Kind = kind;
      Text = text;
    }
  }
}
