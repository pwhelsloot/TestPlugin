using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class ArgumentListSyntax : SyntaxNode
  {
    public static ArgumentListSyntax Empty = new ArgumentListSyntax(new SyntaxNode[0]);

    public IList<SyntaxNode> Arguments { get; }

    public ArgumentListSyntax(IList<SyntaxNode> arguments)
    {
      if (arguments == null)
        throw new ArgumentNullException(nameof(arguments));

      Arguments = arguments;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitArgumentList(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitArgumentList(this);
    }
  }
}
