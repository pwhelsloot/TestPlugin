using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  internal class FieldOrder : IOrder
  {
    public string Field { get; }

    public OrderDirection Direction { get; }

    public FieldOrder(string field, OrderDirection direction)
    {
      Field = field;
      Direction = direction;
    }
  }
}
