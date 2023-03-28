namespace AMCS.Data.Server.DataSets
{
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data;

  public class DataSetImportBuilder
  {
    private readonly IDataSetService service = DataServices.Resolve<IDataSetService>();

    private DataSetImportMode importMode = DataSetImportMode.Import;
    private readonly DataSetTableSet tableSet = new DataSetTableSet();
    public DataSetTableSet TableSet => tableSet;

    public DataSetImportBuilder SetMode(DataSetImportMode mode)
    {
      importMode = mode;
      return this;
    }

    public DataSetImportBuilder AddRecord(IDataSetRecord record)
    {
      var dataSet = service.GetDataSet(record.GetType());
      var table = tableSet.Tables.SingleOrDefault(p => p.DataSet == dataSet);
      if (table == null)
      {
        table = new DataSetTable(dataSet, dataSet.Columns.Where(c => !c.IsReadOnly).ToList());
        tableSet.Tables.Add(table);
      }

      table.Records.Add(record);

      return this;
    }

    public DataSetImportBuilder AddRecords(IList<IDataSetRecord> records)
    {
      foreach (var record in records)
      {
        AddRecord(record);
      }

      return this;
    }

    public DataSetImportBuilder AddTable(string dataSet, params string[] columns)
    {
      var resolvedDataSet = service.GetDataSet(dataSet);

      return AddTable(resolvedDataSet, columns.Select(resolvedDataSet.GetColumn).ToArray());
    }

    private DataSetImportBuilder AddTable(DataSet dataSet, params DataSetColumn[] columns)
    {
      tableSet.Tables.Add(new DataSetTable(dataSet, columns));
      return this;
    }

    public DataSetImport Build()
    {
      return new DataSetImport(importMode, tableSet);
    }
  }
}
