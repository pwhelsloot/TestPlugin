using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class Importer
  {
    private readonly ISessionToken userId;
    private readonly DataSetImport import;
    private readonly ImportIdManager idManager;
    private DataSetImportResult result;

    public Importer(ISessionToken userId, DataSetImport import)
    {
      this.userId = userId;
      this.import = import;

      idManager = new ImportIdManager(import.TableSet.Tables.Select(p => p.DataSet).ToList());
    }

    public DataSetImportResult Import(IDataSetImportProgress progress)
    {
      result = new DataSetImportResult
      {
        Mode = import.Mode
      };

      DoImport(progress);

      if (progress.CancellationToken.IsCancellationRequested)
        result.Messages.AddError("The import process was canceled");

      return result;
    }

    private void DoImport(IDataSetImportProgress progress)
    {
      try
      {
        progress.SetProgress(0, "Preparing import");

        foreach (var table in import.TableSet.Tables)
        {
          foreach (var column in table.Columns)
          {
            if (column.IsReadOnly)
            {
              result.Messages.AddError($"Column '{column.Label}' is read only. Please remove from import.", table.DataSet);
            }
          }
        }

        if (ReturnOnErrorOrCancellation())
          return;

        var plan = ImportPlan.Build(import.TableSet.Tables, result.Messages);

        if (ReturnOnErrorOrCancellation())
          return;

        var batches = BuildImportBatches(plan);

        if (ReturnOnErrorOrCancellation())
          return;

        var largeBatchesCount = batches.Count(p => p.Actions.Count > 200);
        if (largeBatchesCount > 0)
        {
          var largestBatch = batches.OrderByDescending(p => p.Actions.Count).First();
          var table = largestBatch.Actions.First().Table;
          result.Messages.AddWarn($"You have {largeBatchesCount} batches with more then 200 records, the greatest being {largestBatch.Actions.Count} big", table.DataSet);
        }

        progress.SetProgress(0.2, "Executing import");

        var executeProgress = new DataSetImportProgressDelegate(progress, "Executing import: ", 0.2, 0.8);

        ExecuteImport(batches, executeProgress);
      }
      catch (Exception ex)
      {
        result.Messages.AddError($"An unknown error has occurred: {ex.Message}", exception: MessageException.FromException(ex));
      }
      finally
      {
        if (import.Mode == DataSetImportMode.Import)
          WriteResults(new DataSetImportProgressDelegate(progress, null, 1, 0));
      }

      bool ReturnOnErrorOrCancellation()
      {
        return (result.Messages.HasErrors || progress.CancellationToken.IsCancellationRequested);
      }
    }

    private void ExecuteImport(List<ImportBatch> batches, IDataSetImportProgress progress)
    {
      int parallelImports = DataServices.Resolve<IDataSetService>().ImportManager.Concurrency;
      int threadCount = Math.Min(batches.Count, parallelImports);
      if (batches.Count < threadCount)
        result.Messages.AddWarn($"The number of batches ({batches.Count}) is lower than the number of parallel imports set ({parallelImports}). This will cause slowdown during the import process.");

      var queue = new ImportQueue(batches, progress);
      var runner = new ImportRunner(userId, queue, idManager, threadCount, import);

      result.Messages.AddRange(runner.Run());

      foreach (var batch in batches)
      {
        result.Messages.AddRange(batch.Messages);
      }
    }

    private void WriteResults(IDataSetImportProgress progress)
    {
      progress.SetProgress(0, "Writing results");

      foreach (var id in idManager.Ids)
      {
        if (id.NewId.HasValue)
          id.Column.Property.SetValue(id.Record, id.NewId.Value);
      }

      var successful = new HashSet<IDataSetRecord>(
        idManager.Ids.Where(p => p.IsSuccess).Select(p => p.Record)
      );

      foreach (var table in import.TableSet.Tables)
      {
        var importedTable = new DataSetTable(table.DataSet, table.Columns);
        result.ImportedSet.Tables.Add(importedTable);
        var failedTable = new DataSetTable(table.DataSet, table.Columns);
        result.FailedSet.Tables.Add(failedTable);

        foreach (var record in table.Records)
        {
          if (successful.Contains(record))
            importedTable.Records.Add(record);
          else
            failedTable.Records.Add(record);
        }
      }
    }

    private List<ImportBatch> BuildImportBatches(ImportPlan plan)
    {
      var batchBuilder = new ImportBatchBuilder(idManager);

      foreach (var step in plan.Steps)
      {
        step.Build(batchBuilder, idManager, result.Messages);
      }

      return batchBuilder.GetBatches();
    }
  }
}
