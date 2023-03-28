using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public static class FieldUtils 
  {
    public static void Validate(IFieldExpression expression, Type valueType, FieldComparison op, string filterName)
    {
      if (expression == null)
      {
        throw new ArgumentException($"{filterName} criteria must be supplied.");
      }

      if (expression.Comparison != op)
      {
        throw new ArgumentException($"{filterName} criteria must use {op} operator.");
      }

      if (expression.Value.GetType() != valueType)
      {
        throw new ArgumentException($"{filterName} criteria must have value of type {valueType.Name}");
      }
    }

    public static void Validate(ICriteria criteria, Type valueType, FieldComparison op, string filterName)
    {
      IFieldExpression expression = criteria.Expressions.OfType<IFieldExpression>().FirstOrDefault(p => p.Field.Equals(filterName) && p.Comparison == op);
      Validate(expression, valueType, op, filterName);
    }

    public static IFieldExpression ExtractFromCriteria(ICriteria criteria, string fieldName, FieldComparison fieldComparison)
    {
      return criteria.Expressions.OfType<IFieldExpression>().FirstOrDefault(p => p.Field.Equals(fieldName) && p.Comparison == fieldComparison);
    }

    public static void RemoveFromCriteria(ICriteria criteria, string fieldName, FieldComparison fieldComparison)
    {
      criteria.Expressions.Remove(ExtractFromCriteria(criteria, fieldName, fieldComparison));
    }
  }
}
