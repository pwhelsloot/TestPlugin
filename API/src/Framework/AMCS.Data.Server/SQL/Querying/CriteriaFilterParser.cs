using System;
using System.Collections;
using AMCS.ApiService.Filtering;
using AMCS.Data.Entity;
using AMCS.Data.Support;

namespace AMCS.Data.Server.SQL.Querying
{
  public static class CriteriaFilterParser
  {
    public static ICriteria Parse(string filter, Type entityType)
    {
      var accessor = EntityObjectAccessor.ForType(entityType);

      return Parse(filter, accessor);
    }

    public static ICriteria Parse(string filter, EntityObjectAccessor accessor)
    {
      return Parse(FilterParser.Parse(filter), accessor);
    }

    public static ICriteria Parse(Filter filter, Type entityType)
    {
      var accessor = EntityObjectAccessor.ForType(entityType);

      return Parse(filter, accessor);
    }

    private static ICriteria Parse(Filter parsed, EntityObjectAccessor accessor)
    {
      var criteria = Criteria.For(accessor.Type);
      var visitor = new Visitor(accessor, criteria);

      foreach (var expression in parsed.Expressions)
      {
        expression.Accept(visitor);
      }

      return criteria;
    }

    private class Visitor : IFilterExpressionVisitor
    {
      private readonly EntityObjectAccessor accessor;
      private readonly ICriteria criteria;

      public Visitor(EntityObjectAccessor accessor, ICriteria criteria)
      {
        this.accessor = accessor;
        this.criteria = criteria;
      }

      private (EntityObjectProperty Property, Type Type) GetProperty(string identifier)
      {
        var property = accessor.GetPropertyByColumnName(identifier);
        if (property == null)
          throw new FilterException($"Unknown field name {identifier}");

        var propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

        return (property, propertyType);
      }

      private object ParseValue(FilterValue value, EntityObjectProperty property, Type type)
      {
        object result = value.Value;
        if (result == null)
          return null;

        if (value.Kind == FilterValueKind.String)
        {
          result = JsonUtil.Parse((string)value.Value, type);
        }

        if (result != null && result.GetType() != type)
        {
          if (!ValueCoercion.TryCoerce(result, type, out var converted))
            throw new FilterException($"Expected value of type {type.Name} instead of {result.GetType().Name}");
          result = converted;
        }

        var converter = property.Column?.DateStorageConverter;
        if (converter != null)
          result = converter.ToStorage(result, null);

        return result;
      }

      public void VisitBinaryExpression(FilterBinaryExpression expression)
      {
        var property = GetProperty(expression.Identifier);
        if (property.Type == typeof(bool))
        {
          switch (expression.Operator)
          {
            case FilterBinaryOperator.Eq:
            case FilterBinaryOperator.Ne:
              break;
            default:
              throw new FilterException($"Invalid operation {expression.Operator} on boolean field");
          }
        }

        object value = ParseValue(expression.Value, property.Property, property.Type);

        switch (expression.Operator)
        {
          case FilterBinaryOperator.Eq:
            if (value == null)
              criteria.Add(Expression.Null(property.Property.Name));
            else
              criteria.Add(Expression.Eq(property.Property.Name, value));
            break;
          case FilterBinaryOperator.Ne:
            if (value == null)
              criteria.Add(Expression.NotNull(property.Property.Name));
            else
              criteria.Add(Expression.Ne(property.Property.Name, value));
            break;
          case FilterBinaryOperator.Gt:
            criteria.Add(Expression.Gt(property.Property.Name, value));
            break;
          case FilterBinaryOperator.Gte:
            criteria.Add(Expression.Ge(property.Property.Name, value));
            break;
          case FilterBinaryOperator.Lt:
            criteria.Add(Expression.Lt(property.Property.Name, value));
            break;
          case FilterBinaryOperator.Lte:
            criteria.Add(Expression.Le(property.Property.Name, value));
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      public void VisitFromToExpression(FilterFromToExpression expression)
      {
        var property = GetProperty(expression.Identifier);

        criteria.Add(Expression.Ge(property.Property.Name, ParseValue(expression.From, property.Property, property.Type)));
        criteria.Add(Expression.Le(property.Property.Name, ParseValue(expression.To, property.Property, property.Type)));
      }

      public void VisitInExpression(FilterInExpression expression)
      {
        var property = GetProperty(expression.Identifier);

        if (property.Type == typeof(bool))
          throw new FilterException($"In operator cannot be used with type {property.Type}");

        var values = new ArrayList();
        foreach (var value in expression.Values)
        {
          values.Add(ParseValue(value, property.Property, property.Type));
        }

        criteria.Add(Expression.In(property.Property.Name, values));
      }

      public void VisitLikeExpression(FilterLikeExpression expression)
      {
        var property = GetProperty(expression.Identifier);
        if (property.Type != typeof(string))
          throw new FilterException($"Like operator cannot be applied to property '{expression.Identifier}' of type '{property.Type}'");

        switch (expression.Operator)
        {
          case FilterLikeOperator.StartsWith:
            criteria.Add(Expression.Like(property.Property.Name, Like.StartsWith(expression.Value)));
            break;
          case FilterLikeOperator.EndsWith:
            criteria.Add(Expression.Like(property.Property.Name, Like.EndsWith(expression.Value)));
            break;
          case FilterLikeOperator.Contains:
            criteria.Add(Expression.Like(property.Property.Name, Like.Contains(expression.Value)));
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
  }
}
