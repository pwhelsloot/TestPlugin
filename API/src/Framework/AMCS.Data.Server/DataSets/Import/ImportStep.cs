using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  [DebuggerDisplay("Table = {Table.DataSet.Name}, IsUpdate = {IsUpdate}, IsFinal = {IsFinal}")]
  internal class ImportStep
  {
    public DataSetTable Table { get; }

    public bool IsUpdate { get; }

    public bool IsFinal { get; }

    public IList<DataSetColumn> Columns { get; } = new List<DataSetColumn>();

    public ImportStep(DataSetTable table, bool isUpdate, bool isFinal)
    {
      Table = table;
      IsFinal = isFinal;
      IsUpdate = isUpdate;
    }

    public void Build(ImportBatchBuilder batchBuilder, ImportIdManager idManager, MessageCollection messages)
    {
      IList<IDataSetRecord> records;
      if (IsUpdate)
        records = Table.Records;
      else
        records = new ImportTableReorderer(Table, messages).GetOrdered();

      foreach (var record in records)
      {
        int recordId = record.GetId();
        if (recordId == 0)
        {
          messages.AddError("Record must have an ID value", Table.DataSet, record);
          continue;
        }

        var id = idManager.Get(Table.DataSet, recordId, record, Table.DataSet.KeyColumn);

        batchBuilder.AddAction(new ImportAction(Table, id, IsUpdate, IsFinal, Columns, record), messages);
      }
    }
  }
}
