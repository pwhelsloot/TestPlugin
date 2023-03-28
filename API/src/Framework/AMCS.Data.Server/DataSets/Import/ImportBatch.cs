using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportBatch
  {
    public List<ImportAction> Actions { get; } = new List<ImportAction>();

    public MessageCollection Messages { get; } = new MessageCollection();

    public void Import(ISessionToken userId, ImportIdManager idManager, IDataSession dataSession, bool throwOnTransientError = false)
    {
      Messages.Clear();

      foreach (var action in Actions)
      {
        var dataSet = action.Table.DataSet;
        var executor = dataSet.ImportExecutorFactory();

        try
        {
          int? recordId = null;
          Mode mode;

          if (action.IsUpdate)
          {
            recordId = action.Id.NewId.Value;
            mode = Mode.Update;
          }
          else if (action.Id.Id > 0)
          {
            recordId = action.Id.Id;
            mode = Mode.Merge;
          }
          else
          {
            mode = Mode.Create;
          }

          int? newId = executor.SaveRecord(userId, recordId, action.Record, action.Table, action.Columns, idManager, Messages, dataSession);

          if (Messages.HasErrors)
            return;

          if (!newId.HasValue && mode == Mode.Merge)
            Messages.AddInfo("No value changed", action);

          if (newId.HasValue)
            recordId = newId;

          action.Id.NewId = recordId;
        }
        catch (Exception ex) when (!(throwOnTransientError && SqlAzureRetriableExceptionDetector.ShouldRetryOn(ex)))
        {
          Messages.AddError($"Error while creating record: {ex.Message}", action, MessageException.FromException(ex));
          return;
        }
      }
    }

    private enum Mode
    {
      Create,
      Update,
      Merge
    }
  }
}
