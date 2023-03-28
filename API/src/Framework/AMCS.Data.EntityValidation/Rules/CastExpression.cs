using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class CastExpression : IExpression
  {
    private readonly Cast cast;
    private readonly IExpression expression;

    public Type Type => cast.ReturnType;

    public CastExpression(Cast cast, IExpression expression)
    {
      if (cast == null)
        throw new ArgumentNullException(nameof(cast));
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));

      this.cast = cast;
      this.expression = expression;
    }

    public object GetValue(object target)
    {
      object value = expression.GetValue(target);

      return cast.GetValue(value);
    }
  }
}
