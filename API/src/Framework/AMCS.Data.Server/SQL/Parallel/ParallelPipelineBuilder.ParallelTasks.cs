using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  partial class ParallelPipelineBuilder
  {
    private class ParallelTask : IParallelTask
    {
      private readonly Action<IDataSession> run;

      public ParallelTask(Action<IDataSession> run)
      {
        this.run = run;
      }

      public void ExecuteWith()
      {
      }

      public void ExecuteRun(IDataSession dataSession)
      {
        run(dataSession);
      }

      public void ExecuteCollect()
      {
      }
    }

    private class ParallelTaskWithInput<TWith> : IParallelTask
    {
      private readonly Func<TWith> with;
      private readonly Action<IDataSession, TWith> run;

      private TWith input;

      public ParallelTaskWithInput(Func<TWith> with, Action<IDataSession, TWith> run)
      {
        this.with = with;
        this.run = run;
      }

      public void ExecuteWith()
      {
        input = with();
      }

      public void ExecuteRun(IDataSession dataSession)
      {
        run(dataSession, input);
      }

      public void ExecuteCollect()
      {
      }
    }

    private class ParallelTaskWithResult<TResult> : IParallelTask
    {
      private readonly Func<IDataSession, TResult> run;
      private readonly Action<TResult> collect;

      private TResult result;

      public ParallelTaskWithResult(Func<IDataSession, TResult> run, Action<TResult> collect)
      {
        this.run = run;
        this.collect = collect;
      }

      public void ExecuteWith()
      {
      }

      public void ExecuteRun(IDataSession dataSession)
      {
        result = run(dataSession);
      }

      public void ExecuteCollect()
      {
        collect(result);
      }
    }

    private class ParallelTaskWithInputAndResult<TWith, TResult> : IParallelTask
    {
      private readonly Func<TWith> with;
      private readonly Func<IDataSession, TWith, TResult> run;
      private readonly Action<TResult> collect;

      private TWith input;
      private TResult result;

      public ParallelTaskWithInputAndResult(Func<TWith> with, Func<IDataSession, TWith, TResult> run, Action<TResult> collect)
      {
        this.with = with;
        this.run = run;
        this.collect = collect;
      }

      public void ExecuteWith()
      {
        input = with();
      }

      public void ExecuteRun(IDataSession dataSession)
      {
        result = run(dataSession, input);
      }

      public void ExecuteCollect()
      {
        collect(result);
      }
    }
  }
}
