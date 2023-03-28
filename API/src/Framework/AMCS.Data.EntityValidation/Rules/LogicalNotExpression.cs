using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class LogicalNotExpression : IExpression
  {
    private readonly IExpression expression;

    public Type Type => typeof(bool);

    public LogicalNotExpression(IExpression expression)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));

      this.expression = expression;
    }

    public object GetValue(object target)
    {
      bool value = (bool)expression.GetValue(target);

      return !value;
    }
  }
}
