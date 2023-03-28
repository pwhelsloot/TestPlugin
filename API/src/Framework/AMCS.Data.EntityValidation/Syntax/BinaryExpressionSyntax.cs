using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class BinaryExpressionSyntax : SyntaxNode
  {
    public SyntaxNode Left { get; }
    public SyntaxNode Right { get; }
    public BinaryExpressionType Type { get; }

    public BinaryExpressionSyntax(SyntaxNode left, SyntaxNode right, BinaryExpressionType type)
    {
      if (left == null)
        throw new ArgumentNullException(nameof(left));
      if (right == null)
        throw new ArgumentNullException(nameof(right));

      Left = left;
      Right = right;
      Type = type;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
      visitor.VisitBinaryExpression(this);
    }

    public override T Accept<T>(ISyntaxVisitor<T> visitor)
    {
      return visitor.VisitBinaryExpression(this);
    }
  }
}
