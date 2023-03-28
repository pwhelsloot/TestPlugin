using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  internal class FieldExpression : IFieldExpression
  {
    public string Field { get; }

    public FieldComparison Comparison { get; }

    public object Value { get; }

    public FieldExpression(string field, FieldComparison comparison, object value)
    {
      Field = field;
      Comparison = comparison;
      Value = value;
    }
  }
}