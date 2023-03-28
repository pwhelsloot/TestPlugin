using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  public interface IParallelTaskInitialBuilder
  {
    IParallelTaskWithBuilder<TWith> With<TWith>(Func<TWith> action);

    IParallelTask Run(Action<IDataSession> action);

    IParallelTaskCollectionBuilder<TResult> Run<TResult>(Func<IDataSession, TResult> action);
  }
}
