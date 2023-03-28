using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  public interface IParallelTaskCollectionBuilder<TResult>
  {
    IParallelTask Collect(Action<TResult> action);
  }
}
