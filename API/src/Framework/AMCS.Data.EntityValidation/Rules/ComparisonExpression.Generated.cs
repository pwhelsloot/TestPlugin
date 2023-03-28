using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.EntityValidation.Syntax;

namespace AMCS.Data.EntityValidation.Rules
{
  partial class ComparisonExpression
  {
    public static ComparisonExpression FromExpressionType(BinaryExpressionType type, IComparer comparer, IExpression left, IExpression right)
    {
      if (comparer == null)
        throw new ArgumentNullException(nameof(comparer));
      if (left == null)
        throw new ArgumentNullException(nameof(left));
      if (right == null)
        throw new ArgumentNullException(nameof(right));

      switch (type)
      {
        case BinaryExpressionType.Equals:
          return new EqualsComparisonExpression(comparer, left, right);
        case BinaryExpressionType.NotEquals:
          return new NotEqualsComparisonExpression(comparer, left, right);
        case BinaryExpressionType.Greater:
          return new GreaterComparisonExpression(comparer, left, right);
        case BinaryExpressionType.GreaterOrEquals:
          return new GreaterOrEqualsComparisonExpression(comparer, left, right);
        case BinaryExpressionType.Less:
          return new LessComparisonExpression(comparer, left, right);
        case BinaryExpressionType.LessOrEquals:
          return new LessOrEqualsComparisonExpression(comparer, left, right);
        default:
          throw new ArgumentOutOfRangeException(nameof(type));
      }
    }

    private class EqualsComparisonExpression : ComparisonExpression
    {
      public EqualsComparisonExpression(IComparer comparer, IExpression left, IExpression right)
        : base(comparer, left, right)
      {
      }

      protected override bool Compare(int comparison)
      {
        return comparison == 0;
      }
    }

    private class NotEqualsComparisonExpression : ComparisonExpression
    {
      public NotEqualsComparisonExpression(IComparer comparer, IExpression left, IExpression right)
        : base(comparer, left, right)
      {
      }

      protected override bool Compare(int comparison)
      {
        return comparison != 0;
      }
    }

    private class GreaterComparisonExpression : ComparisonExpression
    {
      public GreaterComparisonExpression(IComparer comparer, IExpression left, IExpression right)
        : base(comparer, left, right)
      {
      }

      protected override bool Compare(int comparison)
      {
        return comparison > 0;
      }
    }

    private class GreaterOrEqualsComparisonExpression : ComparisonExpression
    {
      public GreaterOrEqualsComparisonExpression(IComparer comparer, IExpression left, IExpression right)
        : base(comparer, left, right)
      {
      }

      protected override bool Compare(int comparison)
      {
        return comparison >= 0;
      }
    }

    private class LessComparisonExpression : ComparisonExpression
    {
      public LessComparisonExpression(IComparer comparer, IExpression left, IExpression right)
        : base(comparer, left, right)
      {
      }

      protected override bool Compare(int comparison)
      {
        return comparison < 0;
      }
    }

    private class LessOrEqualsComparisonExpression : ComparisonExpression
    {
      public LessOrEqualsComparisonExpression(IComparer comparer, IExpression left, IExpression right)
        : base(comparer, left, right)
      {
      }

      protected override bool Compare(int comparison)
      {
        return comparison <= 0;
      }
    }
  }
}
