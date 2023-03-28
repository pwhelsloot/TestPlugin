namespace AMCS.Data.Server.Api
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.ApiService.Filtering;
  using AMCS.Data.Entity;
  using AMCS.Data.Support;
  using Newtonsoft.Json.Linq;

  public static class FilterMatch
  {
    public static bool IsMatch(string filter, EntityObject entity, out Exception exception)
    {
      try
      {
        var result = IsMatch(filter, entity);
        exception = null;

        return result;
      }
      catch (Exception ex)
      {
        exception = ex;
        return false;
      }
    }

    public static bool IsMatch(string filter, EntityObject entity)
    {
      if (string.IsNullOrWhiteSpace(filter))
        return true;

      if (!FilterParser.TryParse(filter, out var resultFilter))
        throw new InvalidOperationException($"Invalid filter \"{filter}\"");

      var jsonEntity = JObject.FromObject(entity);
      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      var matchingCriteria = new List<bool>();
      var visitor = new FilterMatchVisitor(matchingCriteria, accessor, jsonEntity);

      foreach (var expression in resultFilter.Expressions)
      {
        expression.Accept(visitor);
      }

      return matchingCriteria.All(item => item);
    }
    
    private class FilterMatchVisitor : IFilterExpressionVisitor
    {
      private readonly List<bool> matchingCriteria;
      private readonly EntityObjectAccessor accessor;
      private readonly JObject jsonEntity;

      public FilterMatchVisitor(List<bool> matchingCriteria, EntityObjectAccessor accessor,
        JObject jsonEntity)
      {
        this.matchingCriteria = matchingCriteria;
        this.accessor = accessor;
        this.jsonEntity = jsonEntity;
      }

      public void VisitBinaryExpression(FilterBinaryExpression expression)
      {
        var (property, type, isNullable) = GetProperty(expression.Identifier);
        var expectedValue = ParseValue(expression.Value, property, type);

        var actualValue = GetJTokenValue(property.Name);

        if (isNullable
            && expression.Value.Kind == FilterValueKind.Null
            && (expression.Operator == FilterBinaryOperator.Eq || expression.Operator == FilterBinaryOperator.Ne))
        {
          var result = expression.Operator == FilterBinaryOperator.Eq
            ? actualValue.Type == JTokenType.Null
            : actualValue.Type != JTokenType.Null;
          matchingCriteria.Add(result);

          return;
        }

        if (type == typeof(bool))
        {
          switch (expression.Operator)
          {
            case FilterBinaryOperator.Eq:
              matchingCriteria.Add(expectedValue.Equals((bool)actualValue));
              return;
            case FilterBinaryOperator.Ne:
              matchingCriteria.Add(!expectedValue.Equals((bool)actualValue));
              return;
            default:
              throw new FilterException($"Invalid operation {expression.Operator} on boolean field");
          }
        }

        var typedActualValue = GetTypedValue(property.Name, type);

        switch (expression.Operator)
        {
          case FilterBinaryOperator.Eq:
            matchingCriteria.Add(expectedValue.Equals(typedActualValue));
            return;
          case FilterBinaryOperator.Ne:
            matchingCriteria.Add(!expectedValue.Equals(typedActualValue));
            return;
          case FilterBinaryOperator.Gt:
            matchingCriteria.Add(Compare(typedActualValue, expectedValue) > 0);
            return;
          case FilterBinaryOperator.Gte:
            matchingCriteria.Add(Compare(typedActualValue, expectedValue) >= 0);
            return;
          case FilterBinaryOperator.Lt:
            matchingCriteria.Add(Compare(typedActualValue, expectedValue) < 0);
            return;
          case FilterBinaryOperator.Lte:
            matchingCriteria.Add(Compare(typedActualValue, expectedValue) <= 0);
            return;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      public void VisitFromToExpression(FilterFromToExpression expression)
      {
        var (property, type, _) = GetProperty(expression.Identifier);

        var fromValue = ParseValue(expression.From, property, type);
        var toValue = ParseValue(expression.To, property, type);

        var actualValue = GetTypedValue(property.Name, type);

        var fromResult = Compare(actualValue, fromValue) >= 0;
        var toResult = Compare(actualValue, toValue) <= 0;

        matchingCriteria.Add(fromResult && toResult);
      }

      public void VisitInExpression(FilterInExpression expression)
      {
        var (property, type, _) = GetProperty(expression.Identifier);

        var actualValue = GetTypedValue(property.Name, type);

        if (expression.Values.Select(value => ParseValue(value, property, type)).Contains(actualValue))
        {
          matchingCriteria.Add(true);
          return;
        }

        matchingCriteria.Add(false);
      }

      public void VisitLikeExpression(FilterLikeExpression expression)
      {
        var (property, type, _) = GetProperty(expression.Identifier);

        if (type != typeof(string))
          throw new FilterException($"Like operator cannot be applied to property '{expression.Identifier}' of type '{type}'");

        var actualValue = (string)GetTypedValue(property.Name, type);

        switch (expression.Operator)
        {
          case FilterLikeOperator.StartsWith:
            matchingCriteria.Add(actualValue.StartsWith(expression.Value));
            return;
          case FilterLikeOperator.EndsWith:
            matchingCriteria.Add(actualValue.EndsWith(expression.Value));
            return;
          case FilterLikeOperator.Contains:
            matchingCriteria.Add(actualValue.Contains(expression.Value));
            return;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      private static object ParseValue(FilterValue value, EntityObjectProperty property, Type type)
      {
        var result = value.Value;
        if (result == null)
          return null;

        if (value.Kind == FilterValueKind.String)
          result = JsonUtil.Parse((string)value.Value, type);

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

      private static int Compare(object actualValue, object expectedValue) =>
        Comparer.DefaultInvariant.Compare(actualValue, expectedValue);

      private (EntityObjectProperty Property, Type Type, bool IsNullable) GetProperty(string identifier)
      {
        var property = accessor.GetPropertyByColumnName(identifier);
        if (property == null)
          throw new FilterException($"Unknown field name {identifier}");

        var nullableType = Nullable.GetUnderlyingType(property.Type);
        var propertyType = nullableType ?? property.Type;

        return (property, propertyType, nullableType != null);
      }

      private JToken GetJTokenValue(string propertyName)
      {
        var jToken = jsonEntity[propertyName];

        if (jToken == null)
          throw new FilterException($"Could not find property {propertyName} while applying filter");

        return jToken;
      }

      private object GetTypedValue(string propertyName, Type type)
      {
        var actualValue = GetJTokenValue(propertyName);
        var typedValue = actualValue.ToObject(type);

        return typedValue;
      }

    }
  }
}