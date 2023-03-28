using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.DataSets.Support;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetService : IDelayedStartup, IDataSetService
  {
    private readonly TypeManager typeManager;
    private readonly List<DataSet> dataSets = new List<DataSet>();
    private readonly Dictionary<Type, DataSet> dataSetsByType = new Dictionary<Type, DataSet>();
    private readonly Dictionary<string, DataSet> dataSetsByName = new Dictionary<string, DataSet>();

    public IList<DataSet> DataSets { get; }

    public IDataSetImportManager ImportManager { get; }

    public IDataSetsConfiguration Configuration { get; }

    public DataSetService(TypeManager typeManager, IDataSetsConfiguration configuration, IJobReader jobReader, IConnectionString blobStorageConnectionString, SchedulerClient schedulerClient)
    {
      this.typeManager = typeManager;

      ImportManager = new DataSetImportManager(configuration, jobReader, blobStorageConnectionString, schedulerClient);
      Configuration = configuration;
      DataSets = new ReadOnlyCollection<DataSet>(dataSets);
    }

    public void Start()
    {
      var builders = new List<DataSetBuilder>();

      // Data sets are built in two phases. First we construct the
      // bare data sets with just its own properties and the columns. When
      // we have all of these, we build restrictions and filters. The reason for
      // this is that (at least) the filters can reference data sets, so these
      // need to be available.

      foreach (var type in typeManager.GetTypes())
      {
        if (typeof(DataSetBuilder).IsAssignableFrom(type) && type.CanConstruct())
        {
          var dataSetBuilder = (DataSetBuilder)Activator.CreateInstance(type);

          dataSets.Add(dataSetBuilder.DataSet);
          dataSetsByType.Add(dataSetBuilder.DataSet.Type, dataSetBuilder.DataSet);
          dataSetsByName.Add(dataSetBuilder.DataSet.Name, dataSetBuilder.DataSet);

          builders.Add(dataSetBuilder);
        }
      }

      foreach (var builder in builders)
      {
        var restrictions = builder.GetRestrictions(this);
        var filters = builder.GetFilters(this);

        builder.DataSet.Complete(restrictions, filters);
      }
    }

    public DataSet GetDataSet(Type type)
    {
      var dataSet = FindDataSet(type);
      if (dataSet == null)
        throw new ArgumentException($"No data set available for type '{type}'");

      return dataSet;
    }

    public DataSet FindDataSet(Type type)
    {
      dataSetsByType.TryGetValue(type, out var result);
      return result;
    }

    public DataSetImportResult Import(ISessionToken userId, DataSetImport import, IDataSetImportProgress progress = null)
    {
      var runner = new DataSetImportRunner(userId, import, progress ?? DataSetImportSinkProgress.Instance);

      return runner.Run();
    }

    public DataSet GetDataSet(string name)
    {
      var dataSet = FindDataSet(name);
      if (dataSet == null)
        throw new ArgumentException($"No data set found named '{name}'");

      return dataSet;
    }

    public DataSet FindDataSet(string name)
    {
      dataSetsByName.TryGetValue(name, out var result);
      return result;
    }
  }
}