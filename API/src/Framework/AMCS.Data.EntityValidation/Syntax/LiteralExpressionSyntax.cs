using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class LiteralExpressionSyntax : SyntaxNode
  {
    public static readonly LiteralExpressionSyntax True = new LiteralExpressionSyntax(true);
    public static readonly LiteralExpressionSyntax False = new LiteralExpressionSyntax(false);
    public static readonly LiteralExpressionSyntax Null = new LiteralExpressionSyntax(null);

    public object Value { get; }

    public LiteralExpressionSyntax(object value)
    {
      Value = value;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitLiteralExpression(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitLiteralExpression(this);
    }
  }
}
