using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace AMCS.Data.Server.SQL.Parallel
{
  internal class ParallelTaskExecutor
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ParallelTaskExecutor));

    private readonly object syncRoot = new object();

    private readonly IDataSession dataSession;
    private readonly int numberOfThreads;
    private readonly Queue<IParallelTask> parallelTasks;
    private int completedThreads;

    public ParallelTaskExecutor(IDataSession dataSession, int numberOfThreads, IEnumerable<IParallelTask> parallelTasks)
    {
      if (numberOfThreads < 1)
        throw new ArgumentException($"{ numberOfThreads } Must be greater than 0.", nameof(numberOfThreads));

      this.dataSession = dataSession;
      this.numberOfThreads = numberOfThreads;
      this.parallelTasks = new Queue<IParallelTask>(parallelTasks);
    }

    public void Execute()
    {
      var availableThreads = new List<Thread>();
      var exceptions = new List<Exception>();

      using (var manualResetEventSlim = numberOfThreads > 1 ? new ManualResetEventSlim(false) : null)
      {
        for (int i = 0; i < numberOfThreads - 1; i++)
        {
          var thread = new Thread(() =>
          {
            try
            {
              ExecuteTaskOnThread(exceptions, manualResetEventSlim);
            }
            catch (Exception ex)
            {
              Logger.Error(ex);
            }
          })
          {
            IsBackground = true,
            Name = $"Parallel data session executor thread {i}"
          };

          availableThreads.Add(thread);
          thread.Start();
        }

        ExecuteTask(dataSession, exceptions, manualResetEventSlim);

        availableThreads.ForEach(thread => thread.Join());

        if (exceptions.Count > 0)
        {
          throw new AggregateException(exceptions);
        }
      }
    }

    public void ExecuteTaskOnThread(IList<Exception> exceptions, ManualResetEventSlim manualResetEventSlim)
    {
      using (var dataSession = BslDataSessionFactory.GetDataSession())
      {
        ExecuteTask(dataSession, exceptions, manualResetEventSlim);
      }
    }

    public void ExecuteTask(IDataSession ds, IList<Exception> exceptions, ManualResetEventSlim manualResetEventSlim)
    {
      using (var transaction = ds.CreateTransaction())
      {
        while (true)
        {
          try
          {
            IParallelTask task;

            lock (syncRoot)
            {
              if (exceptions.Count > 0 || parallelTasks.Count == 0)
                break;

              task = parallelTasks.Dequeue();

              task.ExecuteWith();
            }

            task.ExecuteRun(ds);

            lock (syncRoot)
            {
              task.ExecuteCollect();
            }
          }
          catch (Exception ex)
          {
            lock (syncRoot)
            {
              exceptions.Add(ex);
              break;
            }
          }
        }

        lock (syncRoot)
        {
          completedThreads++;

          if (completedThreads == numberOfThreads)
          {
            manualResetEventSlim?.Set();
          }
        }

        manualResetEventSlim?.Wait();

        lock (syncRoot)
        {
          if (exceptions.Count == 0)
            transaction.Commit();
        }
      }
    }
  }
}
