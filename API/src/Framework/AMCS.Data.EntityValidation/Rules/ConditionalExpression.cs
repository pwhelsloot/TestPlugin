using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.EntityValidation.Syntax;

namespace AMCS.Data.EntityValidation.Rules
{
  internal abstract class ConditionalExpression : IExpression
  {
    public static ConditionalExpression FromExpressionType(BinaryExpressionType type, IExpression left, IExpression right)
    {
      if (left == null)
        throw new ArgumentNullException(nameof(left));
      if (right == null)
        throw new ArgumentNullException(nameof(right));

      switch (type)
      {
        case BinaryExpressionType.LogicalAnd:
          return new AndConditionalExpression(left, right);
        case BinaryExpressionType.LogicalOr:
          return new OrConditionalExpression(left, right);
        default:
          throw new ArgumentOutOfRangeException(nameof(type));
      }
    }

    public Type Type => typeof(bool);

    private ConditionalExpression()
    {
    }

    public object GetValue(object target)
    {
      return Compare(target);
    }

    protected abstract bool Compare(object target);

    private class AndConditionalExpression : ConditionalExpression
    {
      private readonly IExpression left;
      private readonly IExpression right;

      public AndConditionalExpression(IExpression left, IExpression right)
      {
        this.left = left;
        this.right = right;
      }

      protected override bool Compare(object target)
      {
        bool leftValue = (bool)left.GetValue(target);
        if (!leftValue)
          return false;

        return (bool)right.GetValue(target);
      }
    }

    private class OrConditionalExpression : ConditionalExpression
    {
      private readonly IExpression left;
      private readonly IExpression right;

      public OrConditionalExpression(IExpression left, IExpression right)
      {
        this.left = left;
        this.right = right;
      }

      protected override bool Compare(object target)
      {
        bool leftValue = (bool)left.GetValue(target);
        if (leftValue)
          return true;

        return (bool)right.GetValue(target);
      }
    }
  }
}
