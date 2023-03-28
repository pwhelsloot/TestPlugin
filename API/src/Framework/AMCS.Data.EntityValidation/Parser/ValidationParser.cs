using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.EntityValidation.Syntax;

namespace AMCS.Data.EntityValidation.Parser
{
  internal class ValidationParser
  {
    public static SyntaxNode Parse(string expression)
    {
      var parser = new ValidationParser(ValidationLexer.Parse(expression));

      return parser.Parse();
    }

    private readonly List<Token> tokens;
    private int offset;

    private bool AtEnd => offset >= tokens.Count;

    private ValidationParser(List<Token> tokens)
    {
      this.tokens = tokens;
    }

    private SyntaxNode Parse()
    {
      var expression = ParseExpression();
      if (!AtEnd)
        throw new ParseException($"Did not expect token '{Take().Kind}'");
      return expression;
    }

    private SyntaxNode ParseExpression()
    {
      return ParseLogicalOrExpression();
    }

    private SyntaxNode ParseLogicalOrExpression()
    {
      var expression = ParseLogicalAndExpression();

      while (TryTake(TokenKind.LogicalOr))
      {
        expression = new BinaryExpressionSyntax(
          expression,
          ParseLogicalAndExpression(),
          BinaryExpressionType.LogicalOr);
      }

      return expression;
    }

    private SyntaxNode ParseLogicalAndExpression()
    {
      var expression = ParseUnaryNotExpression();

      while (TryTake(TokenKind.LogicalAnd))
      {
        expression = new BinaryExpressionSyntax(
          expression,
          ParseUnaryNotExpression(),
          BinaryExpressionType.LogicalAnd);
      }

      return expression;
    }

    private SyntaxNode ParseUnaryNotExpression()
    {
      if (TryTake(TokenKind.LogicalNot))
      {
        return new UnaryExpressionSyntax(
          ParseUnaryNotExpression(),
          UnaryExpressionType.LogicalNot);
      }

      return ParseEqualityExpression();
    }

    private SyntaxNode ParseEqualityExpression()
    {
      var expression = ParseRelationalExpression();

      while (true)
      {
        if (TryTake(TokenKind.Equals))
        {
          expression = new BinaryExpressionSyntax(
            expression,
            ParseRelationalExpression(),
            BinaryExpressionType.Equals);
        }
        else if (TryTake(TokenKind.NotEquals))
        {
          expression = new BinaryExpressionSyntax(
            expression,
            ParseRelationalExpression(),
            BinaryExpressionType.NotEquals);
        }
        else
        {
          break;
        }
      }

      return expression;
    }

    private SyntaxNode ParseRelationalExpression()
    {
      var expression = ParseUnaryExpression();

      while (true)
      {
        if (TryTake(TokenKind.Less))
        {
          expression = new BinaryExpressionSyntax(
            expression,
            ParseUnaryExpression(),
            BinaryExpressionType.Less);
        }
        else if (TryTake(TokenKind.LessOrEquals))
        {
          expression = new BinaryExpressionSyntax(
            expression,
            ParseUnaryExpression(),
            BinaryExpressionType.LessOrEquals);
        }
        else if (TryTake(TokenKind.Greater))
        {
          expression = new BinaryExpressionSyntax(
            expression,
            ParseUnaryExpression(),
            BinaryExpressionType.Greater);
        }
        else if (TryTake(TokenKind.GreaterOrEquals))
        {
          expression = new BinaryExpressionSyntax(
            expression,
            ParseUnaryExpression(),
            BinaryExpressionType.GreaterOrEquals);
        }
        else
        {
          break;
        }
      }

      return expression;
    }

    private SyntaxNode ParseUnaryExpression()
    {
      if (TryTake(TokenKind.Plus))
      {
        return new UnaryExpressionSyntax(
          ParseUnaryExpression(),
          UnaryExpressionType.Plus);
      }
      if (TryTake(TokenKind.Minus))
      {
        return new UnaryExpressionSyntax(
          ParseUnaryExpression(),
          UnaryExpressionType.Minus);
      }
      return ParsePostfixExpression();
    }

    private SyntaxNode ParsePostfixExpression()
    {
      var expression = ParsePrimaryExpression();

      while (true)
      {
        if (TryTake(TokenKind.ParenOpen))
        {
          if (TryTake(TokenKind.ParenClose))
          {
            expression = new InvocationExpressionSyntax(
              expression,
              ArgumentListSyntax.Empty);
          }
          else
          {
            expression = new InvocationExpressionSyntax(
              expression,
              ParseArgumentExpressionList());
            Take(TokenKind.ParenClose);
          }
        }
        else if (TryTake(TokenKind.Dot))
        {
          var identifier = Take(TokenKind.Identifier);

          expression = new MemberAccessExpressionSyntax(
            expression,
            CreateIdentifier(identifier.Text));
        }
        else
        {
          break;
        }
      }

      return expression;
    }

    private ArgumentListSyntax ParseArgumentExpressionList()
    {
      var arguments = new List<SyntaxNode> { ParseExpression() };

      while (TryTake(TokenKind.Comma))
      {
        arguments.Add(ParseExpression());
      }

      return new ArgumentListSyntax(new ReadOnlyCollection<SyntaxNode>(arguments));
    }

    private SyntaxNode ParsePrimaryExpression()
    {
      if (TryTake(TokenKind.ParenOpen))
      {
        var expression = ParseExpression();
        Take(TokenKind.ParenClose);
        return new UnaryExpressionSyntax(
          expression,
          UnaryExpressionType.Group);
      }

      var token = Take();
      switch (token.Kind)
      {
        case TokenKind.Identifier:
          return CreateIdentifier(token.Text);
        case TokenKind.True:
          return LiteralExpressionSyntax.True;
        case TokenKind.False:
          return LiteralExpressionSyntax.False;
        case TokenKind.Null:
          return LiteralExpressionSyntax.Null;
        case TokenKind.DecimalLiteral:
          return ParseDecimal(token.Text);
        case TokenKind.CharacterLiteral:
          return ParseCharacter(token.Text);
        case TokenKind.StringLiteral:
          return ParseString(token.Text);
        case TokenKind.FloatingPointLiteral:
          return ParseFloatingPoint(token.Text);
        default:
          throw new ParseException($"Unexpected token '{token.Kind}'");
      }
    }

    private LiteralExpressionSyntax ParseDecimal(string text)
    {
      if (
          text.EndsWith("ul", StringComparison.OrdinalIgnoreCase) ||
          text.EndsWith("lu", StringComparison.OrdinalIgnoreCase)
      )
        return new LiteralExpressionSyntax(new UnparsedNumber(text.Substring(0, text.Length - 2), typeof(ulong), NumberStyles.None));
      if (text.EndsWith("l", StringComparison.OrdinalIgnoreCase))
        return new LiteralExpressionSyntax(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(long), NumberStyles.None));
      if (text.EndsWith("u", StringComparison.OrdinalIgnoreCase))
        return new LiteralExpressionSyntax(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(uint), NumberStyles.None));
      if (
        text.EndsWith("f", StringComparison.OrdinalIgnoreCase) ||
        text.EndsWith("d", StringComparison.OrdinalIgnoreCase) ||
        text.EndsWith("m", StringComparison.OrdinalIgnoreCase)
      )
        return ParseFloatingPoint(text);

      return new LiteralExpressionSyntax(new UnparsedNumber(text, typeof(int), NumberStyles.None));
    }

    private LiteralExpressionSyntax ParseFloatingPoint(string text)
    {
      char suffix = char.ToLowerInvariant(text[text.Length - 1]);

      switch (suffix)
      {
        case 'f': return new LiteralExpressionSyntax(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(float), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
        case 'd': return new LiteralExpressionSyntax(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(double), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
        case 'm': return new LiteralExpressionSyntax(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(decimal), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
        default:
          Debug.Assert(char.IsDigit(suffix) || suffix == '.');

          return new LiteralExpressionSyntax(new UnparsedNumber(text, typeof(double), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
      }
    }

    private LiteralExpressionSyntax ParseCharacter(string text)
    {
      Debug.Assert(text.Substring(0, 1) == "'");
      Debug.Assert(text.Substring(text.Length - 1) == "'");

      text = ParseEscapes(text.Substring(1, text.Length - 2));
      if (text.Length != 1)
        throw new ParseException("Too many characters in literal");

      return new LiteralExpressionSyntax(text[0]);
    }

    private LiteralExpressionSyntax ParseString(string text)
    {
      Debug.Assert(text.Substring(0, 1) == "\"");
      Debug.Assert(text.Substring(text.Length - 1) == "\"");

      text = ParseEscapes(text.Substring(1, text.Length - 2));

      return new LiteralExpressionSyntax(text);
    }

    private string ParseEscapes(string text)
    {
      var sb = new StringBuilder(text.Length);

      bool hadEscape = false;

      for (int i = 0; i < text.Length; i++)
      {
        char c = text[i];

        if (hadEscape)
        {
          hadEscape = false;

          switch (c)
          {
            case 'B':
            case 'b':
              sb.Append('\b');
              break;

            case 'T':
            case 't':
              sb.Append('\t');
              break;

            case 'N':
            case 'n':
              sb.Append('\n');
              break;

            case 'R':
            case 'r':
              sb.Append('\r');
              break;

            case 'F':
            case 'f':
              sb.Append('\f');
              break;

            case '"':
              sb.Append('"');
              break;

            case '\'':
              sb.Append('\'');
              break;

            case '\\':
              sb.Append('\\');
              break;

            case 'u':
            case 'U':
              Debug.Assert(i < text.Length - 4);

              sb.Append((char)uint.Parse(text.Substring(i + 1, 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));

              i += 4;
              break;

            default:
              throw new ArgumentOutOfRangeException(text);
          }
        }
        else
        {
          if (c == '\\')
            hadEscape = true;
          else
            sb.Append(c);
        }
      }

      return sb.ToString();
    }

    private IdentifierNameSyntax CreateIdentifier(string text)
    {
      return new IdentifierNameSyntax(text);
    }

    private Token Peek()
    {
      if (offset >= tokens.Count)
        return new Token();

      return tokens[offset];
    }

    private Token Take()
    {
      if (AtEnd)
        throw new ParseException("Unexpected end");

      return tokens[offset++];
    }

    private Token Take(TokenKind kind)
    {
      var token = Take();
      if (token.Kind != kind)
        throw new ParseException($"Expected token of kind {kind}, got {token.Kind}");

      return token;
    }

    private bool TryTake(TokenKind kind)
    {
      if (Peek().Kind == kind)
      {
        Take();
        return true;
      }

      return false;
    }
  }
}
