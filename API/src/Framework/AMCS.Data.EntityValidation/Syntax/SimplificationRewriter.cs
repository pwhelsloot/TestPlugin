using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  /// <summary>
  /// Simplify a syntax tree by removing group unaries (i.e. parenthesized expressions)
  /// and collapsing unary + and - tokens with underlying unparsed numbers, integrating the
  /// sign into the unparsed number value itself.
  /// </summary>
  internal class SimplificationRewriter : SyntaxRewriter
  {
    public override SyntaxNode VisitUnaryExpression(UnaryExpressionSyntax node)
    {
      var expression = node.Expression.Accept(this);

      // Remove group (i.e. parenthesis) nodes.
      if (node.Type == UnaryExpressionType.Group)
        return expression;

      switch (node.Type)
      {
        case UnaryExpressionType.Minus:
        case UnaryExpressionType.Plus:
          if (
            expression is LiteralExpressionSyntax literalExpression &&
            literalExpression.Value is UnparsedNumber number
          )
          {
            // + signes are just dropped because they don't influence numeric parsing.
            if (node.Type == UnaryExpressionType.Plus)
              return expression;

            string numberText = number.Text;

            // Turn -- into a number without a sign, and a number without a sign into a number
            // with a sign:
            //
            //   -(-1) becomes 1
            //   -(1) becomes -1
            //

            if (numberText.StartsWith("-"))
              numberText = numberText.Substring(1);
            else
              numberText = "-" + numberText;

            // We always add an AllowLeadingSign. Even -- should be parsed as allowing a leading
            // sign since it was signed at some point.

            number = new UnparsedNumber(numberText, number.Type, number.NumberStyles | NumberStyles.AllowLeadingSign);

            return new LiteralExpressionSyntax(number);
          }
          break;
      }

      if (node.Expression == expression)
        return node;

      return new UnaryExpressionSyntax(expression, node.Type);
    }
  }
}
