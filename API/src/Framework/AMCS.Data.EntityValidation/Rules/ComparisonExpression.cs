using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal abstract partial class ComparisonExpression : IExpression
  {
    private readonly IComparer comparer;
    private readonly IExpression left;
    private readonly IExpression right;

    public Type Type => typeof(bool);

    private ComparisonExpression(IComparer comparer, IExpression left, IExpression right)
    {
      this.comparer = comparer;
      this.left = left;
      this.right = right;
    }

    public object GetValue(object target)
    {
      object leftValue = left.GetValue(target);
      object rightValue = right.GetValue(target);

      int comparison = comparer.Compare(leftValue, rightValue);

      return Compare(comparison);
    }

    protected abstract bool Compare(int comparison);
  }
}
