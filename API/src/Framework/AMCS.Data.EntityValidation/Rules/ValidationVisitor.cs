using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.EntityValidation.Syntax;
using FastMember;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class ValidationVisitor : ISyntaxVisitor<IExpression>
  {
    private readonly Type targetType;
    private readonly TypeAccessor typeAccessor;

    public ValidationVisitor(Type targetType)
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof(targetType));

      this.targetType = targetType;
      typeAccessor = TypeAccessor.Create(targetType);
    }

    public IExpression VisitArgumentList(ArgumentListSyntax node)
    {
      throw new InvalidOperationException();
    }

    public IExpression VisitBinaryExpression(BinaryExpressionSyntax node)
    {
      var left = node.Left.Accept(this);
      var right = node.Right.Accept(this);

      switch (node.Type)
      {
        case BinaryExpressionType.LogicalAnd:
        case BinaryExpressionType.LogicalOr:
          if (left.Type != typeof(bool) || right.Type != typeof(bool))
            throw new ParseException($"Cannot parse conditional expression on type {left.Type} and {right.Type}");

          return ConditionalExpression.FromExpressionType(node.Type, left, right);
      }

      var comparer = ValueComparison.GetComparer(left.Type, right.Type);
      if (comparer == null)
        throw new ParseException($"Cannot compare expression of type {left.Type} and {right.Type}");

      return ComparisonExpression.FromExpressionType(node.Type, comparer, left, right);
    }

    public IExpression VisitLiteralExpression(LiteralExpressionSyntax node)
    {
      switch (node.Value)
      {
        case null:
          return Constant.Null;
        case bool boolValue:
          return boolValue ? Constant.True : Constant.False;
        case string stringValue:
          return new Constant(stringValue);
        case UnparsedNumber number:
          return new Constant(number.Parse());
        default:
          throw new InvalidOperationException("Unknown constant value");
      }
    }

    public IExpression VisitIdentifierName(IdentifierNameSyntax node)
    {
      var member = typeAccessor.GetMembers().SingleOrDefault(p => p.Name == node.Name);
      if (member == null)
        throw new ParseException($"Cannot resolve member {node.Name} on type {targetType}");

      var expression = new IdentifierExpression(typeAccessor, node.Name, member.Type);

      var type = Nullable.GetUnderlyingType(expression.Type) ?? expression.Type;
      if (type.IsEnum)
      {
        var underlyingType = Enum.GetUnderlyingType(type);
        var cast = Cast.GetCast(underlyingType);
        if (cast == null)
          throw new InvalidOperationException($"Cannot unwrap enum type {underlyingType}");

        return new CastExpression(cast, expression);
      }

      return expression;
    }

    public IExpression VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
      var expression = node.Expression.Accept(this);

      var memberAccess = MemberAccess.GetMemberAccess(expression.Type, node.Name.Name);

      if (memberAccess == null)
        throw new ParseException($"Cannot resolve member access of member {node.Name.Name} on type {expression.Type}");

      return new MemberAccessExpression(memberAccess, expression);
    }

    public IExpression VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      string name = PrintName(node.Expression);

      var arguments = new List<IExpression>();
      var argumentTypes = new List<Type>();

      foreach (var argument in node.Arguments.Arguments)
      {
        var expression = argument.Accept(this);

        arguments.Add(expression);
        argumentTypes.Add(expression.Type);
      }

      var invocable = Invocable.GetInvocable(name, argumentTypes);
      if (invocable == null)
        throw new ParseException($"Unknown method {name}");

      return new InvocationExpression(invocable, arguments);
    }

    private string PrintName(SyntaxNode node)
    {
      switch (node)
      {
        case IdentifierNameSyntax identifierName:
          return identifierName.Name;
        case MemberAccessExpressionSyntax memberAccess:
          return PrintName(memberAccess.Expression) + "." + PrintName(memberAccess.Name);
        default:
          throw new ParseException($"Cannot invoke expression of type {node.GetType().Name}");
      }
    }

    public IExpression VisitUnaryExpression(UnaryExpressionSyntax node)
    {
      // The SimplificationRewriter takes care of the rest.
      if (node.Type != UnaryExpressionType.LogicalNot)
        throw new InvalidOperationException($"Unexpected unary expression type {node.Type}");

      return new LogicalNotExpression(node.Expression.Accept(this));
    }
  }
}
