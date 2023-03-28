using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public interface IOrder
  {
    string Field { get; }

    OrderDirection Direction { get; }
  }
}
