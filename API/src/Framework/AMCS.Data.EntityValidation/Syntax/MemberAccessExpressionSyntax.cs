using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class MemberAccessExpressionSyntax : SyntaxNode
  {
    public SyntaxNode Expression { get; }
    public IdentifierNameSyntax Name { get; }

    public MemberAccessExpressionSyntax(SyntaxNode expression, IdentifierNameSyntax name)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));
      if (name == null)
        throw new ArgumentNullException(nameof(name));

      Expression = expression;
      Name = name;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitMemberAccessExpression(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitMemberAccessExpression(this);
    }
  }
}
