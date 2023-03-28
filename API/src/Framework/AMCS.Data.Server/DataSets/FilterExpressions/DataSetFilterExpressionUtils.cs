using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Filtering;

namespace AMCS.Data.Server.DataSets.FilterExpressions
{
  public static class DataSetFilterExpressionUtils
  {
    public static IFilterExpression GetFilterExpression(DataSetFilterExpression expression, Func<string, string> propertyMapper = null)
    {
      return expression.Accept(new FilterVisitor(propertyMapper));
    }

    public static Filter GetFilter(IList<DataSetFilterExpression> expressions, Func<string, string> propertyMapper = null)
    {
      var result = new List<IFilterExpression>(expressions.Count);
      var visitor = new FilterVisitor(propertyMapper);

      foreach (var expression in expressions)
      {
        result.Add(expression.Accept(visitor));
      }

      return new Filter(result);
    }

    private class FilterVisitor : IDataSetFilterExpressionVisitor<IFilterExpression>
    {
      private readonly Func<string, string> propertyMapper;

      public FilterVisitor(Func<string, string> propertyMapper)
      {
        this.propertyMapper = propertyMapper;
      }

      public IFilterExpression VisitBinaryExpression(DataSetFilterBinaryExpression expression)
      {
        return new FilterBinaryExpression(
          GetBinaryOperator(expression.Operator),
          GetPropertyName(expression),
          GetFilterValue(expression.Value));
      }

      private string GetPropertyName(DataSetFilterExpression expression)
      {
        string name = expression.Column.Property.Name;
        if (propertyMapper != null)
          return propertyMapper(name);
        return name;
      }

      private FilterBinaryOperator GetBinaryOperator(DataSetFilterBinaryOperator @operator)
      {
        switch (@operator)
        {
          case DataSetFilterBinaryOperator.Eq: return FilterBinaryOperator.Eq;
          case DataSetFilterBinaryOperator.Ne: return FilterBinaryOperator.Ne;
          case DataSetFilterBinaryOperator.Gt: return FilterBinaryOperator.Gt;
          case DataSetFilterBinaryOperator.Gte: return FilterBinaryOperator.Gte;
          case DataSetFilterBinaryOperator.Lt: return FilterBinaryOperator.Lt;
          case DataSetFilterBinaryOperator.Lte: return FilterBinaryOperator.Lte;
          default:
            throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
        }
      }

      public IFilterExpression VisitFromToExpression(DataSetFilterFromToExpression expression)
      {
        return new FilterFromToExpression(
          GetPropertyName(expression),
          GetFilterValue(expression.From),
          GetFilterValue(expression.To));
      }

      public IFilterExpression VisitLikeExpression(DataSetFilterLikeExpression expression)
      {
        return new FilterLikeExpression(
          GetFilterOperator(expression.Operator),
          GetPropertyName(expression),
          expression.Value);
      }

      private FilterLikeOperator GetFilterOperator(DataSetFilterLikeOperator @operator)
      {
        switch (@operator)
        {
          case DataSetFilterLikeOperator.StartsWith: return FilterLikeOperator.StartsWith;
          case DataSetFilterLikeOperator.EndsWith: return FilterLikeOperator.EndsWith;
          case DataSetFilterLikeOperator.Contains: return FilterLikeOperator.Contains;
          default:
            throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
        }
      }

      public IFilterExpression VisitInExpression(DataSetFilterInExpression expression)
      {
        return new FilterInExpression(
          GetPropertyName(expression),
          GetFilterValueList(expression.Values));
      }

      private FilterValue GetFilterValue(DataSetValue value)
      {
        return new FilterValue(
          GetFilterValueKind(value.Kind),
          value.Value);
      }

      private FilterValueKind GetFilterValueKind(DataSets.DataSetValueKind kind)
      {
        switch (kind)
        {
          case DataSets.DataSetValueKind.Integer: return FilterValueKind.Integer;
          case DataSets.DataSetValueKind.Decimal: return FilterValueKind.Decimal;
          case DataSets.DataSetValueKind.String: return FilterValueKind.String;
          case DataSets.DataSetValueKind.Boolean: return FilterValueKind.Boolean;
          case DataSets.DataSetValueKind.Null: return FilterValueKind.Null;
          default:
            throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
      }

      private IList<FilterValue> GetFilterValueList(DataSetValueList values)
      {
        var result = new List<FilterValue>(values.Items.Count);

        foreach (var value in values.Items)
        {
          result.Add(GetFilterValue(value));
        }

        return result;
      }
    }
  }
}
