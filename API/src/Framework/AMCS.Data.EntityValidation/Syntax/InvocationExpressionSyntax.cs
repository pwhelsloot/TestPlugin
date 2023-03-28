using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class InvocationExpressionSyntax : SyntaxNode
  {
    public SyntaxNode Expression { get; }
    public ArgumentListSyntax Arguments { get; }

    public InvocationExpressionSyntax(SyntaxNode expression, ArgumentListSyntax arguments)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));
      if (arguments == null)
        throw new ArgumentNullException(nameof(arguments));

      Expression = expression;
      Arguments = arguments;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitInvocationExpression(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitInvocationExpression(this);
    }
  }
}
