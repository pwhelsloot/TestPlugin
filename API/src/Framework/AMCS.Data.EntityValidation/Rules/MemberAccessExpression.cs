using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class MemberAccessExpression : IExpression
  {
    private readonly MemberAccess memberAccess;
    private readonly IExpression expression;

    public Type Type => memberAccess.ReturnType;

    public MemberAccessExpression(MemberAccess memberAccess, IExpression expression)
    {
      if (memberAccess == null)
        throw new ArgumentNullException(nameof(memberAccess));
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));

      this.memberAccess = memberAccess;
      this.expression = expression;
    }

    public object GetValue(object target)
    {
      object value = expression.GetValue(target);

      return memberAccess.GetValue(value);
    }
  }
}
