using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportBatchBuilder
  {
    private readonly ImportIdManager idManager;
    private readonly List<Batch> batches = new List<Batch>();
    private readonly Dictionary<IImportId, Batch> idMap = new Dictionary<IImportId, Batch>();

    public ImportBatchBuilder(ImportIdManager idManager)
    {
      this.idManager = idManager;
    }

    public void AddAction(ImportAction action, MessageCollection messages)
    {
      Batch batch = null;
      Batch originalUpdateBatch = null;

      // If this is a delayed update, start with the batch we did the create in.
      if (action.IsUpdate)
        originalUpdateBatch = batch = ResolveBatch(idMap[action.Id]);

      foreach (var column in action.Table.Columns)
      {
        object value = column.Property.GetValue(action.Record);

        // If this value is an ID, and we have it mapped already, add it to a batch.

        IImportId id = null;

        if (value is int intValue)
          id = idManager.GetReferenced(action.Table.DataSet, intValue, action.Record, column);

        if (id != null && idMap.TryGetValue(id, out var idBatch))
        {
          idBatch = ResolveBatch(idBatch);

          // If we've found a new batch, merge it with the current one.

          if (batch != null && idBatch != batch)
          {
            idBatch.Merged = batch;
            batch.Actions.AddRange(idBatch.Actions);
            idBatch.Actions = null;
          }
          else
          {
            batch = idBatch;
          }
        }
      }

      // Create a new batch if we don't yet have one.

      if (batch == null)
      {
        batch = new Batch();
        batches.Add(batch);
      }

      // Add this action to the batch.

      batch.Actions.Add(action);

      // Map this action ID with the batch.

      if (action.IsUpdate)
      {
        if (originalUpdateBatch != batch)
          throw new InvalidOperationException("Expected update batch to be unchanged");
      }
      else
      {
        if (idMap.ContainsKey(action.Id))
          messages.AddError($"Duplicate ID '{action.Id.Id}'", action);
        else
          idMap.Add(action.Id, batch);
      }
    }

    public List<ImportBatch> GetBatches()
    {
      var result = new List<ImportBatch>();

      foreach (var batch in batches)
      {
        if (batch.Merged == null)
        {
          var importBatch = new ImportBatch();
          result.Add(importBatch);
          importBatch.Actions.AddRange(batch.Actions);
        }
      }

      return result;
    }

    private Batch ResolveBatch(Batch batch)
    {
      while (batch.Merged != null)
      {
        batch = batch.Merged;
      }

      return batch;
    }

    private class Batch
    {
      public List<ImportAction> Actions { get; set; } = new List<ImportAction>();
      public Batch Merged { get; set; }
    }
  }
}
