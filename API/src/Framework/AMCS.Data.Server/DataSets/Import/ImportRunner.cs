using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportRunner
  {
    private readonly ImportRetryer retryer = ImportRetryer.CreateDefault();
    private readonly ISessionToken userId;
    private readonly ImportQueue queue;
    private readonly ImportIdManager idManager;
    private readonly int threadCount;
    private readonly DataSetImport import;
    private readonly object syncRoot = new object();
    private readonly MessageCollection messages = new MessageCollection();

    public ImportRunner(ISessionToken userId, ImportQueue queue, ImportIdManager idManager, int threadCount, DataSetImport import)
    {
      this.userId = userId;
      this.queue = queue;
      this.idManager = idManager;
      this.threadCount = threadCount;
      this.import = import;
    }

    public MessageCollection Run()
    {
      var threads = new List<Thread>();

      for (var i = 0; i < threadCount; i++)
      {
        var thread = new Thread(ThreadProc);
        threads.Add(thread);
        thread.Start();
      }

      foreach (var thread in threads)
      {
        thread.Join();
      }

      lock (syncRoot)
      {
        return messages;
      }
    }

    private void ThreadProc()
    {
      try
      {
        DoImport(queue.GetRequester());
      }
      catch (Exception ex)
      {
        lock (syncRoot)
        {
          messages.AddError("Unexpected error: " + ex.Message, exception: MessageException.FromException(ex));
        }
      }
    }

    private void DoImport(IImportQueueRequester requester)
    {
      // During validate we use a transaction per import batch, since we're
      // rolling back everything anyway. This will significantly lower the
      // load on the server, but does lower the chance we'll see stuff
      // like cross record unique index violations.

      while (true)
      {
        var batch = requester.GetNextBatch();
        if (batch == null)
          break;

        retryer.Retry(last =>
        {
          try
          {
            using (var session = BslDataSessionFactory.GetDataSession(userId))
            using (var transaction = session.CreateTransaction())
            {
              batch.Import(userId, idManager, session, throwOnTransientError: !last);

              bool hasErrors = ReportErrors(requester, batch);

              // Only commit on import; on validate we roll back everything.

              if (!hasErrors && import.Mode == DataSetImportMode.Import)
                transaction.Commit();
            }

            return true;
          }
          catch (Exception ex) when (!last && SqlAzureRetriableExceptionDetector.ShouldRetryOn(ex))
          {
            if (ex is SqlException sqlException)
            {
              lock (syncRoot)
              {
                messages.AddWarn(SqlErrorUtils.GetFriendlyErrorMessage(sqlException));
              }
            }
            return false;
          }
        });
      }
    }

    private static bool ReportErrors(IImportQueueRequester requester, ImportBatch batch)
    {
      var errors = batch.Messages.Count(p => p.Type == MessageType.Error);

      foreach (var action in batch.Actions)
      {
        action.Id.IsSuccess = errors == 0;
      }

      if (errors > 0)
      {
        requester.ReportErrors(errors);
        return true;
      }

      return false;
    }
  }
}
