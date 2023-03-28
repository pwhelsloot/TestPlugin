using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportQueue
  {
    private static readonly TimeSpan ReportInterval = TimeSpan.FromSeconds(1);

    private readonly IDataSetImportProgress progress;
    private readonly List<ImportBatch> queue;
    private int queueOffset;
    private readonly int actionCount;
    private int actionsProcessed;
    private readonly object syncRoot = new object();
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private int errors;

    public ImportQueue(List<ImportBatch> batches, IDataSetImportProgress progress)
    {
      this.progress = progress;

      actionCount = batches.Sum(p => p.Actions.Count);
      queue = batches.ToList();
    }

    public IImportQueueRequester GetRequester()
    {
      return new ImportQueueRequester(this);
    }

    private ImportBatch GetNextBatch()
    {
      lock (syncRoot)
      {
        if (queueOffset >= queue.Count || progress.CancellationToken.IsCancellationRequested)
          return null;

        var batch = queue[queueOffset++];
        actionsProcessed += batch.Actions.Count;

        if (stopwatch.Elapsed >= ReportInterval)
        {
          stopwatch.Restart();

          var label = $"{actionsProcessed} / {actionCount}";
          if (errors > 0)
            label += $" ({errors} errors)";

          progress.SetProgress(actionsProcessed, actionCount, label);
        }

        return batch;
      }
    }

    private void ReportErrors(int errors)
    {
      lock (syncRoot)
      {
        this.errors += errors;
      }
    }

    private class ImportQueueRequester : IImportQueueRequester
    {
      private readonly ImportQueue queue;

      public ImportQueueRequester(ImportQueue queue)
      {
        this.queue = queue;
      }

      public ImportBatch GetNextBatch()
      {
        return queue.GetNextBatch();
      }

      public void ReportErrors(int errors)
      {
        queue.ReportErrors(errors);
      }
    }
  }
}
