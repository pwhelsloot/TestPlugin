using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportIdManager : IDataSetDefaultImportIdMapper
  {
    private readonly List<IImportId> ids = new List<IImportId>();
    private readonly Dictionary<(DataSet, int), ImportIdValue> values = new Dictionary<(DataSet, int), ImportIdValue>();
    private readonly Dictionary<(DataSet, DataSetColumn, IDataSetRecord), IImportId> referenceIds = new Dictionary<(DataSet, DataSetColumn, IDataSetRecord), IImportId>();
    private readonly Dictionary<(DataSet, DataSetColumn), DataSetRelationship> relationships;

    public IList<IImportId> Ids { get; }

    public ImportIdManager(IList<DataSet> dataSets)
    {
      Ids = new ReadOnlyCollection<IImportId>(ids);

      var dataSetsSet = new HashSet<DataSet>(dataSets);

      relationships = dataSets
        .SelectMany(p => p.Relationships)
        .Where(p => dataSetsSet.Contains(p.To))
        .ToDictionary(p => (p.From, p.FromColumn), p => p);
    }

    public IImportId Get(DataSet dataSet, int id, IDataSetRecord record, DataSetColumn column)
    {
      if (!values.TryGetValue((dataSet, id), out var value))
      {
        value = new ImportIdValue(id);
        values.Add((dataSet, id), value);
      }

      var importId = new ImportId(dataSet, record, column, value);
      ids.Add(importId);

      return importId;
    }

    public IImportId GetReferenced(DataSet dataSet, int id, IDataSetRecord record, DataSetColumn column)
    {
      var key = (dataSet, column, record);

      if (referenceIds.TryGetValue(key, out var importId))
        return importId;

      if (!relationships.TryGetValue((dataSet, column), out var relationship))
        return null;

      importId = Get(relationship.To, id, record, column);

      referenceIds.Add(key, importId);

      return importId;
    }

    public bool TryMapId(DataSet dataSet, int id, IDataSetRecord record, DataSetColumn column, out int result)
    {
      var importId = GetReferenced(dataSet, id, record, column);
      if (importId != null)
      {
        result = importId.NewId ?? importId.Id;
        return true;
      }

      result = 0;
      return false;
    }

    [DebuggerDisplay("DataSet = {DataSet.Name}, Id = {Id}")]
    private class ImportId : IImportId
    {
      private readonly ImportIdValue value;

      public DataSet DataSet { get; }

      public int Id => value.Id;

      public int? NewId
      {
        get => value.NewId;
        set => this.value.NewId = value;
      }

      public IDataSetRecord Record { get; }

      public DataSetColumn Column { get; }

      public bool IsSuccess { get; set; }

      public ImportId(DataSet dataSet, IDataSetRecord record, DataSetColumn column, ImportIdValue value)
      {
        DataSet = dataSet;
        Record = record;
        Column = column;
        this.value = value;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(this, obj))
          return true;

        return
          obj is ImportId other &&
          value == other.value;
      }

      public override int GetHashCode()
      {
        return value.GetHashCode();
      }
    }

    private class ImportIdValue
    {
      public int Id { get; }
      public int? NewId { get; set; }

      public ImportIdValue(int id)
      {
        Id = id;
      }
    }
  }
}
