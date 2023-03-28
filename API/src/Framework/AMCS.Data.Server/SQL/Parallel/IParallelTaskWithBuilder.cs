using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  public interface IParallelTaskWithBuilder<TWith>
  {
    IParallelTask Run(Action<IDataSession, TWith> action);

    IParallelTaskCollectionBuilder<TResult> Run<TResult>(Func<IDataSession, TWith, TResult> action);
  }
}
