using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  partial class ParallelPipelineBuilder
  {
    private class ParallelTaskInitialBuilder : IParallelTaskInitialBuilder
    {
      public IParallelTaskWithBuilder<TWith> With<TWith>(Func<TWith> action)
      {
        return new ParallelTaskWithBuilder<TWith>(action);
      }

      public IParallelTask Run(Action<IDataSession> action)
      {
        return new ParallelTask(action);
      }

      public IParallelTaskCollectionBuilder<TResult> Run<TResult>(Func<IDataSession, TResult> action)
      {
        return new ParallelTaskCollectionBuilder<TResult>(action);
      }
    }

    private class ParallelTaskWithBuilder<TWith> : IParallelTaskWithBuilder<TWith>
    {
      private readonly Func<TWith> with;

      internal ParallelTaskWithBuilder(Func<TWith> with)
      {
        this.with = with;
      }

      public IParallelTask Run(Action<IDataSession, TWith> action)
      {
        return new ParallelTaskWithInput<TWith>(with, action);
      }

      public IParallelTaskCollectionBuilder<TResult> Run<TResult>(Func<IDataSession, TWith, TResult> action)
      {
        return new ParallelTaskCollectionBuilder<TWith, TResult>(with, action);
      }
    }

    private class ParallelTaskCollectionBuilder<TResult> : IParallelTaskCollectionBuilder<TResult>
    {
      private readonly Func<IDataSession, TResult> run;

      public ParallelTaskCollectionBuilder(Func<IDataSession, TResult> run)
      {
        this.run = run;
      }

      public IParallelTask Collect(Action<TResult> action)
      {
        return new ParallelTaskWithResult<TResult>(run, action);
      }
    }

    private class ParallelTaskCollectionBuilder<TWith, TResult> : IParallelTaskCollectionBuilder<TResult>
    {
      private readonly Func<TWith> with;
      private readonly Func<IDataSession, TWith, TResult> run;

      public ParallelTaskCollectionBuilder(Func<TWith> with, Func<IDataSession, TWith, TResult> run)
      {
        this.with = with;
        this.run = run;
      }

      public IParallelTask Collect(Action<TResult> action)
      {
        return new ParallelTaskWithInputAndResult<TWith, TResult>(with, run, action);
      }
    }
  }
}
