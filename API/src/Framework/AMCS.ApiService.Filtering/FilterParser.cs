using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterParser
  {
    public static Filter Parse(string filter)
    {
      var parser = new FilterParser(FilterLexer.Parse(filter));

      return parser.Parse();
    }

    public static bool TryParse(string filter, out Filter result)
    {
      try
      {
        result = Parse(filter);
        return true;
      }
      catch
      {
        result = null;
        return false;
      }
    }

    private readonly List<FilterToken> tokens;
    private int offset;
    private readonly StringBuilder sb = new StringBuilder();

    private bool AtEnd => offset >= tokens.Count;

    private FilterParser(List<FilterToken> tokens)
    {
      this.tokens = tokens;
    }

    private Filter Parse()
    {
      var expressions = new List<IFilterExpression>();

      while (true)
      {
        var identifier = TakeIdentifier();
        var op = Take();

        switch (op.Kind)
        {
          case FilterTokenKind.Eq:
          case FilterTokenKind.Ne:
          case FilterTokenKind.Gt:
          case FilterTokenKind.Gte:
          case FilterTokenKind.Lt:
          case FilterTokenKind.Lte:
            expressions.Add(new FilterBinaryExpression(GetFilterBinaryOperator(op.Kind), identifier, TakeValue()));
            break;
          case FilterTokenKind.StartsWith:
          case FilterTokenKind.EndsWith:
          case FilterTokenKind.Contains:
            expressions.Add(new FilterLikeExpression(GetFilterLikeOperator(op.Kind), identifier, TakeString()));
            break;
          case FilterTokenKind.In:
            expressions.Add(new FilterInExpression(identifier, TakeValueList()));
            break;
          case FilterTokenKind.From:
            var fromValue = TakeValue();
            Take(FilterTokenKind.To);
            var toValue = TakeValue();
            expressions.Add(new FilterFromToExpression(identifier, fromValue, toValue));
            break;
          default:
            throw new FilterException("Expected operator");
        }

        if (AtEnd)
          break;

        Take(FilterTokenKind.And);
      }

      return new Filter(new ReadOnlyCollection<IFilterExpression>(expressions));
    }

    private FilterLikeOperator GetFilterLikeOperator(FilterTokenKind op)
    {
      switch (op)
      {
        case FilterTokenKind.StartsWith:
          return FilterLikeOperator.StartsWith;
        case FilterTokenKind.EndsWith:
          return FilterLikeOperator.EndsWith;
        case FilterTokenKind.Contains:
          return FilterLikeOperator.Contains;
        default:
          throw new ArgumentOutOfRangeException(nameof(op), op, null);
      }
    }

    private FilterBinaryOperator GetFilterBinaryOperator(FilterTokenKind op)
    {
      switch (op)
      {
        case FilterTokenKind.Eq:
          return FilterBinaryOperator.Eq;
        case FilterTokenKind.Ne:
          return FilterBinaryOperator.Ne;
        case FilterTokenKind.Gt:
          return FilterBinaryOperator.Gt;
        case FilterTokenKind.Gte:
          return FilterBinaryOperator.Gte;
        case FilterTokenKind.Lt:
          return FilterBinaryOperator.Lt;
        case FilterTokenKind.Lte:
          return FilterBinaryOperator.Lte;
        default:
          throw new ArgumentOutOfRangeException(nameof(op), op, null);
      }
    }

    private IList<FilterValue> TakeValueList()
    {
      var result = new List<FilterValue>();

      Take(FilterTokenKind.ParenOpen);

      while (true)
      {
        result.Add(TakeValue());

        if (TryTake(FilterTokenKind.ParenClose))
          break;

        Take(FilterTokenKind.Comma);
      }

      return new ReadOnlyCollection<FilterValue>(result);
    }

    private FilterValue TakeValue()
    {
      var token = Take();

      switch (token.Kind)
      {
        case FilterTokenKind.Integer:
          return new FilterValue(FilterValueKind.Integer, int.Parse(token.Text, CultureInfo.InvariantCulture));
        case FilterTokenKind.Decimal:
          return new FilterValue(FilterValueKind.Decimal, decimal.Parse(token.Text, CultureInfo.InvariantCulture));
        case FilterTokenKind.String:
          return new FilterValue(FilterValueKind.String, ParseString(token.Text));
        case FilterTokenKind.True:
          return new FilterValue(FilterValueKind.Boolean, true);
        case FilterTokenKind.False:
          return new FilterValue(FilterValueKind.Boolean, false);
        case FilterTokenKind.Null:
          return new FilterValue(FilterValueKind.Null, null);
        default:
          throw new FilterException("Expected a value");
      }
    }

    private string TakeString()
    {
      return ParseString(Take(FilterTokenKind.String).Text);
    }

    private string TakeIdentifier()
    {
      var token = Take();
      if (token.Kind == FilterTokenKind.Identifier)
        return token.Text;
      if (token.Kind == FilterTokenKind.QuotedIdentifier)
        return ParseString(token.Text);
      throw new FilterException("Expected an identifier");
    }

    private string ParseString(string text)
    {
      sb.Clear();

      char terminator = text[0];

      for (int i = 1; i < text.Length - 1; i++)
      {
        char c = text[i];
        if (c == terminator)
          i++;
        sb.Append(c);
      }

      return sb.ToString();
    }

    private FilterToken Peek()
    {
      if (offset >= tokens.Count)
        return new FilterToken();

      return tokens[offset];
    }

    private FilterToken Take()
    {
      if (AtEnd)
        throw new FilterException("Unexpected end");

      return tokens[offset++];
    }

    private FilterToken Take(FilterTokenKind kind)
    {
      var token = Take();
      if (token.Kind != kind)
        throw new FilterException($"Expected token of kind {kind}, got {token.Kind}");

      return token;
    }

    private bool TryTake(FilterTokenKind kind)
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
