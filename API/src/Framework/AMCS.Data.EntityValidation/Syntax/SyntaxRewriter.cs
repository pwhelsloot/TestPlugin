using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class SyntaxRewriter : ISyntaxVisitor<SyntaxNode>
  {
    public virtual SyntaxNode VisitArgumentList(ArgumentListSyntax node)
    {
      var arguments = new List<SyntaxNode>();
      bool changed = false;

      foreach (var argument in node.Arguments)
      {
        var newArgument = argument.Accept(this);
        arguments.Add(newArgument);
        changed = changed || argument != newArgument;
      }

      if (changed)
        return new ArgumentListSyntax(new ReadOnlyCollection<SyntaxNode>(arguments));

      return node;
    }

    public virtual SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
    {
      var left = node.Left.Accept(this);
      var right = node.Right.Accept(this);

      if (left == node.Left && right == node.Right)
        return node;

      return new BinaryExpressionSyntax(left, right, node.Type);
    }

    public virtual SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
    {
      return node;
    }

    public virtual SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
      return node;
    }

    public virtual SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
      var expression = node.Expression.Accept(this);
      var name = (IdentifierNameSyntax)node.Name.Accept(this);

      if (expression == node.Expression && name == node.Name)
        return node;

      return new MemberAccessExpressionSyntax(expression, name);
    }

    public virtual SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      var expression = node.Expression.Accept(this);
      var arguments = (ArgumentListSyntax)node.Arguments.Accept(this);

      if (expression == node.Expression && arguments == node.Arguments)
        return node;

      return new InvocationExpressionSyntax(expression, arguments);
    }

    public virtual SyntaxNode VisitUnaryExpression(UnaryExpressionSyntax node)
    {
      var expression = node.Expression.Accept(this);

      if (expression == node.Expression)
        return node;

      return new UnaryExpressionSyntax(expression, node.Type);
    }
  }
}
