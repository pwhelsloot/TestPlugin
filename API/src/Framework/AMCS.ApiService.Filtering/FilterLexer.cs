using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  internal class FilterLexer
  {
    private static readonly Dictionary<string, FilterTokenKind> Identifiers = BuildIdentifiers();

    private static Dictionary<string, FilterTokenKind> BuildIdentifiers()
    {
      return new Dictionary<string, FilterTokenKind>(StringComparer.OrdinalIgnoreCase)
      {
        { "eq", FilterTokenKind.Eq },
        { "ne", FilterTokenKind.Ne },
        { "gt", FilterTokenKind.Gt },
        { "gte", FilterTokenKind.Gte },
        { "lt", FilterTokenKind.Lt },
        { "lte", FilterTokenKind.Lte },
        { "startswith", FilterTokenKind.StartsWith },
        { "endswith", FilterTokenKind.EndsWith },
        { "contains", FilterTokenKind.Contains },
        { "in", FilterTokenKind.In },
        { "from", FilterTokenKind.From },
        { "to", FilterTokenKind.To },
        { "and", FilterTokenKind.And },
        { "true", FilterTokenKind.True },
        { "false", FilterTokenKind.False },
        { "null", FilterTokenKind.Null },
      };
    }

    public static List<FilterToken> Parse(string filter)
    {
      var lexer = new FilterLexer(filter);
      var result = new List<FilterToken>();

      while (true)
      {
        var token = lexer.Next();
        if (token.Kind == FilterTokenKind.None)
          break;

        result.Add(token);
      }

      return result;
    }

    private readonly string filter;
    private int offset;

    private bool AtEnd => offset >= filter.Length;

    private FilterLexer(string filter)
    {
      this.filter = filter;
    }

    private FilterToken Next()
    {
      SkipWs();

      if (AtEnd)
        return new FilterToken();

      int start = offset;
      char c = Peek();

      switch (c)
      {
        case '(':
          Take();
          return CreateToken(FilterTokenKind.ParenOpen, start);
        case ')':
          Take();
          return CreateToken(FilterTokenKind.ParenClose, start);
        case ',':
          Take();
          return CreateToken(FilterTokenKind.Comma, start);
        case '\'':
        case '"':
          return ParseString();
        default:
          if (c == '-' || c == '+' || c >= '0' && c <= '9')
            return ParseNumber();

          if (IsIdentifierStart(c))
            return ParseIdentifier();

          throw new FilterException("Unexpected token");
      }
    }

    private FilterToken ParseIdentifier()
    {
      var token = TakeIdentifier();

      if (Identifiers.TryGetValue(token.Text, out var kind))
        return new FilterToken(kind, token.Text);

      return token;
    }

    private FilterToken TakeIdentifier()
    {
      int start = offset;

      while (true)
      {
        char c = Peek();
        if (!IsIdentifier(c))
          break;

        Take();
      }

      return CreateToken(FilterTokenKind.Identifier, start);
    }

    private bool IsIdentifierStart(char c)
    {
      return c == '_' || char.IsLetter(c);
    }

    private bool IsIdentifier(char c)
    {
      return IsIdentifierStart(c) || c == '-' || c >= '0' && c <= '9' || c == '.';
    }

    private FilterToken ParseNumber()
    {
      bool hadDecimal = false;

      int start = offset;

      Take();

      while (true)
      {
        char c = Peek();

        if (!hadDecimal && c == '.')
          hadDecimal = true;
        else if (!(c >= '0' && c <= '9'))
          break;

        Take();
      }

      return CreateToken(hadDecimal ? FilterTokenKind.Decimal : FilterTokenKind.Integer, start);
    }

    private FilterToken ParseString()
    {
      int start = offset;
      char terminator = Take();

      while (!AtEnd)
      {
        if (Take() == terminator)
        {
          // Double quotes are converted into a single quote; otherwise this is
          // the end of the string.

          if (Peek() == terminator)
            Take();
          else
            return CreateToken(terminator == '\'' ? FilterTokenKind.String : FilterTokenKind.QuotedIdentifier, start);
        }
      }

      throw new FilterException("Unterminated string");
    }

    private FilterToken CreateToken(FilterTokenKind kind, int start)
    {
      return new FilterToken(kind, filter.Substring(start, offset - start));
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

      return filter[offset];
    }

    private char Take()
    {
      return filter[offset++];
    }
  }
}
