using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  [DebuggerDisplay("Table = {Table.DataSet.Name}, Id = {Id.Id}, IsUpdate = {IsUpdate}, IsFinal = {IsFinal}")]
  internal class ImportAction
  {
    public DataSetTable Table { get; }

    public IImportId Id { get; }

    public bool IsUpdate { get; }

    public bool IsFinal { get; }

    public IList<DataSetColumn> Columns { get; }

    public IDataSetRecord Record { get; }

    public ImportAction(DataSetTable table, IImportId id, bool isUpdate, bool isFinal, IList<DataSetColumn> columns, IDataSetRecord record)
    {
      Table = table;
      Id = id;
      IsUpdate = isUpdate;
      IsFinal = isFinal;
      Columns = columns;
      Record = record;
    }
  }
}
