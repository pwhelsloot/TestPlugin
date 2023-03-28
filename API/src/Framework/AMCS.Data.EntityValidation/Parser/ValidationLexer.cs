using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Parser
{
  internal class ValidationLexer
  {
    public static List<Token> Parse(string filter)
    {
      var lexer = new ValidationLexer(filter);
      var result = new List<Token>();

      while (true)
      {
        var token = lexer.Next();
        if (token.Kind == TokenKind.None)
          break;

        result.Add(token);
      }

      return result;
    }

    private readonly string expression;
    private int offset;

    private bool AtEnd => offset >= expression.Length;

    private ValidationLexer(string expression)
    {
      this.expression = expression;
    }

    private Token Next()
    {
      SkipWs();

      if (AtEnd)
        return new Token();

      int start = offset;
      char c = Peek();

      switch (c)
      {
        case '(':
          Take();
          return CreateToken(TokenKind.ParenOpen, start);
        case ')':
          Take();
          return CreateToken(TokenKind.ParenClose, start);
        case ',':
          Take();
          return CreateToken(TokenKind.Comma, start);
        case '+':
          Take();
          return CreateToken(TokenKind.Plus, start);
        case '-':
          Take();
          return CreateToken(TokenKind.Minus, start);
        case '!':
          Take();
          if (TryTake('='))
            return CreateToken(TokenKind.NotEquals, start);
          return CreateToken(TokenKind.LogicalNot, start);
        case '&':
          Take();
          Take('&');
          return CreateToken(TokenKind.LogicalAnd, start);
        case '|':
          Take();
          Take('|');
          return CreateToken(TokenKind.LogicalOr, start);
        case '<':
          Take();
          if (TryTake('='))
            return CreateToken(TokenKind.LessOrEquals, start);
          return CreateToken(TokenKind.Less, start);
        case '>':
          Take();
          if (TryTake('='))
            return CreateToken(TokenKind.GreaterOrEquals, start);
          return CreateToken(TokenKind.Greater, start);
        case '=':
          Take();
          Take('=');
          return CreateToken(TokenKind.Equals, start);
        case '\'':
        case '"':
          return ParseString();
        default:
          if (IsDigit(c) || c == '.')
            return ParseNumber();

          if (IsIdentifierStart(c))
            return ParseIdentifier();

          throw new ParseException("Unexpected token");
      }
    }

    private Token ParseIdentifier()
    {
      var token = TakeIdentifier();

      switch (token.Text)
      {
        case "true":
          return new Token(TokenKind.True, token.Text);
        case "false":
          return new Token(TokenKind.False, token.Text);
        case "null":
          return new Token(TokenKind.Null, token.Text);
      }

      return token;
    }

    private Token TakeIdentifier()
    {
      int start = offset;

      while (true)
      {
        char c = Peek();
        if (!IsIdentifier(c))
          break;

        Take();
      }

      return CreateToken(TokenKind.Identifier, start);
    }

    private bool IsIdentifierStart(char c)
    {
      return c == '_' || char.IsLetter(c);
    }

    private bool IsIdentifier(char c)
    {
      return IsIdentifierStart(c) || IsDigit(c);
    }

    private static bool IsDigit(char c)
    {
      return c >= '0' && c <= '9';
    }

    private Token ParseNumber()
    {
      int start = offset;
      bool hadDecimal = Take() == '.';

      if (hadDecimal && !IsDigit(Peek()))
        return CreateToken(TokenKind.Dot, start);

      while (!AtEnd)
      {
        char c = Peek();

        if (!hadDecimal && c == '.')
          hadDecimal = true;
        else if (!IsDigit(c))
          break;

        Take();
      }

      bool hadExponent = TakeExponent();

      if (hadDecimal || hadExponent)
      {
        TakeFloatSuffix();
        return CreateToken(TokenKind.FloatingPointLiteral, start);
      }

      TakeNumericSuffix();
      return CreateToken(TokenKind.DecimalLiteral, start);
    }

    private bool TakeExponent()
    {
      switch (Peek())
      {
        case 'e':
        case 'E':
          Take();
          break;
        default:
          return false;
      }

      switch (Peek())
      {
        case '+':
        case '-':
          Take();
          break;
      }

      bool hadDigit = false;

      while (!AtEnd)
      {
        if (IsDigit(Peek()))
        {
          Take();
          hadDigit = true;
        }
        else
        {
          break;
        }
      }

      if (!hadDigit)
        throw new ParseException("Malformed exponent");

      return true;
    }

    private void TakeNumericSuffix()
    {
      switch (Peek())
      {
        case 'u':
        case 'U':
          Take();
          switch (Peek())
          {
            case 'l':
            case 'L':
              Take();
              break;
          }
          break;
        case 'l':
        case 'L':
          Take();
          switch (Peek())
          {
            case 'u':
            case 'U':
              Take();
              break;
          }
          break;
        default:
          TakeFloatSuffix();
          break;
      }
    }

    private void TakeFloatSuffix()
    {
      switch (Peek())
      {
        case 'f':
        case 'F':
        case 'd':
        case 'D':
        case 'm':
        case 'M':
          Take();
          break;
      }
    }

    private Token ParseString()
    {
      int start = offset;
      char terminator = Take();

      while (!AtEnd)
      {
        char c = Take();

        if (c == terminator)
          return CreateToken(terminator == '"' ? TokenKind.StringLiteral : TokenKind.CharacterLiteral, start);

        if (c == '\\')
        {
          // Skip over whatever comes after a \. If it's a double quote,
          // we won't match the end of a string. For something else,
          // we'll just find the next ".
          Take();
        }
      }

      throw new ParseException(terminator == '"' ? "Unterminated string" : "Unterminated character");
    }

    private Token CreateToken(TokenKind kind, int start)
    {
      return new Token(kind, expression.Substring(start, offset - start));
    }

    private void SkipWs()
    {
      while (char.IsWhiteSpace(Peek()))
      {
        Take();
      }
    }

    private char Peek()
    {
      if (AtEnd)
        return (char)0;

      return expression[offset];
    }

    private char Take()
    {
      return expression[offset++];
    }

    private void Take(char c)
    {
      if (Take() != c)
        throw new ParseException($"Expected '{c}'");
    }

    private bool TryTake(char c)
    {
      if (Peek() == c)
      {
        Take();
        return true;
      }
      return false;
    }
  }
}