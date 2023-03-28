namespace AMCS.Data.Server.DataSets.Support
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class DataSetImportResultReader
  {
    public DataSetImportResult Result { get; }

    public List<IDataSetRecord> ImportedRecords { get; }

    public List<IDataSetRecord> FailedRecords { get; }

    public DataSetImportResultReader(DataSetImportResult result)
    {
      Result = result;
      ImportedRecords = Result.ImportedSet.Tables.SelectMany(table => table.Records).ToList();
      FailedRecords = Result.FailedSet.Tables.SelectMany(table => table.Records).ToList();
    }
  }
}
