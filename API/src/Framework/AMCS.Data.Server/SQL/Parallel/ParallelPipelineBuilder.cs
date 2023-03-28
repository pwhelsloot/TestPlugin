using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  public partial class ParallelPipelineBuilder
  {
    private readonly IDataSession dataSession;
    private readonly int maxNumberOfThreads;
    private readonly List<IParallelTask> parallelTasks;

    public ParallelPipelineBuilder(IDataSession dataSession, int maxNumberOfThreads)
    {
      this.dataSession = dataSession;
      this.maxNumberOfThreads = maxNumberOfThreads;
      parallelTasks = new List<IParallelTask>();
    }

    public ParallelPipelineBuilder Add(Func<IParallelTaskInitialBuilder, IParallelTask> action)
    {
      parallelTasks.Add(action(new ParallelTaskInitialBuilder()));
      return this;
    }

    public void Run()
    {
      var parallelTaskExecutor = new ParallelTaskExecutor(dataSession, Math.Min(maxNumberOfThreads, parallelTasks.Count), parallelTasks);
      parallelTaskExecutor.Execute();
    }
  }
}
