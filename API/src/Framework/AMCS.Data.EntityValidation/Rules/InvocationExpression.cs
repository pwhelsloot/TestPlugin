using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class InvocationExpression : IExpression
  {
    private readonly Invocable invocable;
    private readonly IList<IExpression> arguments;

    public Type Type => invocable.ReturnType;

    public InvocationExpression(Invocable invocable, IList<IExpression> arguments)
    {
      if (invocable == null)
        throw new ArgumentNullException(nameof(invocable));
      if (arguments == null)
        throw new ArgumentNullException(nameof(arguments));

      this.invocable = invocable;
      this.arguments = arguments;
    }

    public object GetValue(object target)
    {
      object[] arguments = new object[this.arguments.Count];

      for (int i = 0; i < this.arguments.Count; i++)
      {
        arguments[i] = this.arguments[i].GetValue(target);
      }

      return invocable.GetValue(arguments);
    }
  }
}
