using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class Constant : IExpression
  {
    public static readonly Constant Null = new Constant(null);
    public static readonly Constant True = new Constant(true);
    public static readonly Constant False = new Constant(false);

    private readonly object value;

    public Type Type => value?.GetType();

    public Constant(object value)
    {
      this.value = value;
    }

    public object GetValue(object target)
    {
      return value;
    }
  }
}
