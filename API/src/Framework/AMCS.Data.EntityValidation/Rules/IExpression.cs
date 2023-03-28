using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal interface IExpression
  {
    Type Type { get; }
    object GetValue(object target);
  }
}
