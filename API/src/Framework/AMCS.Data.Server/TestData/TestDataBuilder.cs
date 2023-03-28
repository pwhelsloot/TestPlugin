namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Server.DataSets;

  public class TestDataBuilder
  {
    private readonly IDataSetService dataSetService = DataServices.Resolve<IDataSetService>();
    private readonly DataSetImportBuilder dataSetImportBuilder;
    private readonly Random random;

    public TestDataBuilder(string randomSeedParameter)
    {
      dataSetImportBuilder = new DataSetImportBuilder();

      if (!int.TryParse(randomSeedParameter, out int randomSeed))
        randomSeed = randomSeedParameter.GetHashCode();
      random = new Random(randomSeed);
    }

    public int NextRandom() => random.Next();
    public string NextRandomString() => NextRandom().ToString();

    public TestDataBuilder SetMode(DataSetImportMode mode)
    {
      dataSetImportBuilder.SetMode(mode);
      return this;
    }

    public TestDataBuilder AddRecords(IList<IDataSetRecord> records)
    {
      dataSetImportBuilder.AddRecords(records);
      return this;
    }

    public TRecord GetDependantRecord<TRecord>()
    {
      var dataSet = dataSetService.GetDataSet(typeof(TRecord));
      var dataSetTable = dataSetImportBuilder.TableSet.Tables.Single(table => table.DataSet == dataSet);
      return (TRecord)dataSetTable.Records.First();
    }

    public DataSetImport Build()
    {
      return dataSetImportBuilder.Build();
    }
  }
}
