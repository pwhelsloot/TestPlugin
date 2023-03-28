using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public interface IFieldExpression : IExpression
  {
    string Field { get; }

    FieldComparison Comparison { get; }

    object Value { get; }
  }
}