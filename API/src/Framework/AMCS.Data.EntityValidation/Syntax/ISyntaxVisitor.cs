using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal interface ISyntaxVisitor
  {
    void VisitArgumentList(ArgumentListSyntax node);
    void VisitBinaryExpression(BinaryExpressionSyntax node);
    void VisitLiteralExpression(LiteralExpressionSyntax node);
    void VisitIdentifierName(IdentifierNameSyntax node);
    void VisitMemberAccessExpression(MemberAccessExpressionSyntax node);
    void VisitInvocationExpression(InvocationExpressionSyntax node);
    void VisitUnaryExpression(UnaryExpressionSyntax node);
  }

  internal interface ISyntaxVisitor<out T>
  {
    T VisitArgumentList(ArgumentListSyntax node);
    T VisitBinaryExpression(BinaryExpressionSyntax node);
    T VisitLiteralExpression(LiteralExpressionSyntax node);
    T VisitIdentifierName(IdentifierNameSyntax node);
    T VisitMemberAccessExpression(MemberAccessExpressionSyntax node);
    T VisitInvocationExpression(InvocationExpressionSyntax node);
    T VisitUnaryExpression(UnaryExpressionSyntax node);
  }
}
