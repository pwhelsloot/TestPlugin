using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class UnaryExpressionSyntax : SyntaxNode
  {
    public SyntaxNode Expression { get; }
    public UnaryExpressionType Type { get; }

    public UnaryExpressionSyntax(SyntaxNode expression, UnaryExpressionType type)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));

      Expression = expression;
      Type = type;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitUnaryExpression(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitUnaryExpression(this);
    }
  }
}
