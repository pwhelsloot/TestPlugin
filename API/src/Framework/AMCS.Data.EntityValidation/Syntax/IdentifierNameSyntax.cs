using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class IdentifierNameSyntax : SyntaxNode
  {
    public string Name { get; }

    public IdentifierNameSyntax(string name)
    {
      Name = name;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitIdentifierName(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitIdentifierName(this);
    }
  }
}
