using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface IEntityObjectChangesFilter
  {
    byte[] RowVersionStart { get; }

    byte[] RowVersionEnd { get; }

    int? IdStart { get; }

    int Count { get; }
  }
}
